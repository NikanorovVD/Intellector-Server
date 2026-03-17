#define ENABLE_NETWORK_LOGGING

#include <iostream>
#include <thread>
#include <unordered_map>
#include <memory>
#include <csignal>
#include <boost/asio.hpp>
#include "Common.h"
#include "Settings.h"
#include "LogWriter.h"
#include "Protocol.h"
#include "WaitingGame.h"
#include "Game.h"

const std::string settingsFilename = "settings.ini";

std::unordered_map<uint32_t, WaitingGame> waitingGames;
uint32_t nextGameId = 1;
std::mutex gamesMutex;
bool serverRunning = true;
boost::asio::io_context io_context;
Settings settings;

std::vector<std::shared_ptr<Game>> activeGames;

void handleShutdownSignal(int) {
    LogWriter::writeLine("Server stopped...");
    serverRunning = false;
    io_context.stop();
}

bool validateClient(boost::asio::ip::tcp::socket& clientSocket) {
    std::string receivedPass = Protocol::recvString(clientSocket);
    if (receivedPass.empty() || receivedPass != settings.password) {
        LogWriter::writeLine("Wrong password");
        return false;
    }

    int clientVersion = Protocol::recvInt(clientSocket);
    Protocol::sendInt(settings.version, clientSocket);
    if (clientVersion != settings.version) {
        LogWriter::writeLine("Wrong version");
        return false;
    }

    return true;
}

void sendGamesList(boost::asio::ip::tcp::socket& clientSocket) {
    std::lock_guard<std::mutex> lock(gamesMutex);
    int count = static_cast<int>(waitingGames.size());
    Protocol::sendInt(count, clientSocket);

    for (const auto& pair : waitingGames) {
        Protocol::sendGameInfo(pair.second.gameInfo, clientSocket); 
    }
}

void waitForGameStart(boost::asio::ip::tcp::socket& clientSocket, uint32_t gameId) {
    const int CHECK_INTERVAL_MS = 500;
    while (true) {
        {
            std::lock_guard<std::mutex> lock(gamesMutex);
            if (waitingGames.find(gameId) == waitingGames.end()) {
                break;
            }
        }

        if (!Protocol::sendCode(SERVER_WAITING_RESPONSE, clientSocket)) {
            break;
        }

        uint8_t ans = Protocol::recvCode(clientSocket);
        if (ans != CLIENT_WAITING_REQUEST) {
            boost::system::error_code ec;
            clientSocket.close(ec);
            std::lock_guard<std::mutex> lock(gamesMutex);
            waitingGames.erase(gameId);
            return;
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(CHECK_INTERVAL_MS));
    }
}

void makeGame(WaitingGame& waitingGame, boost::asio::ip::tcp::socket&& newClientSocket) {
    boost::asio::ip::tcp::socket whiteSocket = std::move(waitingGame.clientSocket);
    boost::asio::ip::tcp::socket blackSocket = std::move(newClientSocket);

    // Swap if needed based on color choice
    switch (waitingGame.gameInfo.colorChoice) {
    case ColorChoice::black:
        // Already have black = newClient, white = waitingGame.client, so swap
        std::swap(whiteSocket, blackSocket);
        break;
    case ColorChoice::random: {
        bool whiteFirst = (rand() % 2 == 0);
        if (!whiteFirst) {
            std::swap(whiteSocket, blackSocket);
        }
        break;
    }
    // For white and default, keep as is (white = waitingGame.client, black = newClient)
    case ColorChoice::white:
    default:
        break;
    }

    auto game = std::make_shared<Game>(
        waitingGame.gameInfo.id, std::move(whiteSocket), std::move(blackSocket), waitingGame.gameInfo);
    game->sendTeams();
    game->start();

    LogWriter::writeLine("Старт игры " + std::to_string(waitingGame.gameInfo.id));
    activeGames.push_back(game); // сохраняем, чтобы объект жил
}

void handleClient(boost::asio::ip::tcp::socket clientSocket) {
    boost::system::error_code ec;
    auto endpoint = clientSocket.remote_endpoint(ec);
    if (!ec) {
        LogWriter::writeLine("Подключение установлено c " + endpoint.address().to_string() + ":" +
                             std::to_string(endpoint.port()));
    }

    if (!validateClient(clientSocket)) {
        clientSocket.close();
        return;
    }

    bool clientActive = true;
    while (clientActive) {
        uint8_t requestCode = Protocol::recvCode(clientSocket);
        if (requestCode == 0) {
            clientActive = false;
            break;
        }

        switch (requestCode) {
        case GAMES_LIST_REQUEST: {
            LogWriter::writeLine("Запрос списка игр");
            sendGamesList(clientSocket);
            break;
        }
        case CREATE_GAME_REQUEST: {
            LogWriter::writeLine("Запрос создания игры");
            GameInfo info = Protocol::recvGameInfoForCreate(clientSocket);
            uint32_t newId;
            {
                std::lock_guard<std::mutex> lock(gamesMutex);
                newId = nextGameId++;
                waitingGames.emplace(newId, WaitingGame(newId, info, std::move(clientSocket)));
            }
            LogWriter::writeLine("Игра успешно создана: ID=" + std::to_string(newId) + ", Name=" + info.name);

            // Получаем ссылку на сокет без удержания мьютекса во время вызова communicateWithWaitingClient
            boost::asio::ip::tcp::socket* sock = nullptr;
            {
                std::lock_guard<std::mutex> lock(gamesMutex);
                auto it = waitingGames.find(newId);
                if (it != waitingGames.end()) {
                    sock = &it->second.clientSocket;
                }
            }
            if (sock) {
                waitForGameStart(*sock, newId);
            }
            clientActive = false;
            break;
        }
        case JOIN_GAME_REQUEST: {
            LogWriter::writeLine("Запрос присоединения к игре");
            uint8_t wantedId = Protocol::recvCode(clientSocket);

            bool found = false;
            std::optional<WaitingGame> waitingGame; // or use a pointer

            {
                std::lock_guard<std::mutex> lock(gamesMutex);
                auto it = waitingGames.find(wantedId);
                if (it != waitingGames.end()) {
                    // Construct WaitingGame directly from the found element
                    waitingGame.emplace(std::move(it->second));
                    waitingGames.erase(it);
                    found = true;
                }
            }

            if (found) {
                Protocol::sendGameInfo(waitingGame->gameInfo, clientSocket);
                LogWriter::writeLine("Старт игры " + std::to_string(wantedId));
                makeGame(*waitingGame, std::move(clientSocket));
            } else {
                Protocol::sendCode(GAME_NOT_EXIST_RESPONSE, clientSocket);
            }
            clientActive = false;
            break;
        }
        default:
            LogWriter::writeLine("Неизвестный код запроса: " + std::to_string(requestCode));
            clientActive = false;
            break;
        }
    }

    boost::system::error_code ec_close;
    clientSocket.close(ec_close);
    LogWriter::writeLine("Клиент отключён");
}

int main() {
    signal(SIGINT, handleShutdownSignal);
    signal(SIGTERM, handleShutdownSignal);

    // Ignore SIGPIPE only on platforms where it exists
#ifndef _WIN32
    signal(SIGPIPE, SIG_IGN);
#endif
    settings = loadSettingsFromFile(settingsFilename);

    try {
        boost::asio::ip::tcp::acceptor acceptor(io_context,
                                                boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), 8080));

        LogWriter::writeLine("Server Start");

        while (serverRunning) {
            boost::asio::ip::tcp::socket clientSocket(io_context);
            boost::system::error_code ec;
            acceptor.accept(clientSocket, ec);
            if (ec) {
                if (serverRunning) {
                    LogWriter::writeLine("Ошибка accept: " + ec.message());
                }
                continue;
            }

            std::thread clientThread(handleClient, std::move(clientSocket));
            clientThread.detach();
        }
    } catch (std::exception& e) {
        LogWriter::writeLine("Исключение: " + std::string(e.what()));
    }

    LogWriter::writeLine("Server stopped");
    return 0;
}