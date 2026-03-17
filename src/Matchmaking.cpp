#include "Matchmaking.h"
#include "Protocol.h"
#include "LogWriter.h"
#include <random>
#include <chrono>
#include <thread>

namespace Matchmaking {

std::unordered_map<uint32_t, WaitingGame> waitingGames;
uint32_t nextGameId = 1;
std::mutex gamesMutex;
std::vector<std::shared_ptr<Game>> activeGames;

void sendGamesList(boost::asio::ip::tcp::socket& clientSocket) {
    std::lock_guard<std::mutex> lock(gamesMutex);
    Protocol::sendInt(static_cast<int>(waitingGames.size()), clientSocket);
    for (const auto& pair : waitingGames) { Protocol::sendGameInfo(pair.second.gameInfo, clientSocket); }
}

void waitForGameStart(boost::asio::ip::tcp::socket& clientSocket, uint32_t gameId) {
    const int CHECK_INTERVAL_MS = 500;
    while (true) {
        {
            std::lock_guard<std::mutex> lock(gamesMutex);
            if (waitingGames.find(gameId) == waitingGames.end()) break;
        }
        if (!Protocol::sendCode(SERVER_WAITING_RESPONSE, clientSocket)) break;
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

    switch (waitingGame.gameInfo.colorChoice) {
    case ColorChoice::black:
        std::swap(whiteSocket, blackSocket);
        break;
    case ColorChoice::random: {
        bool whiteFirst = (rand() % 2 == 0);
        if (!whiteFirst) std::swap(whiteSocket, blackSocket);
        break;
    }
    default:
        break;
    }

    auto game = std::make_shared<Game>(
        waitingGame.gameInfo.id, std::move(whiteSocket), std::move(blackSocket), waitingGame.gameInfo);
    game->sendTeams();
    game->start();

    activeGames.push_back(game);
}

bool handleCreateGame(boost::asio::ip::tcp::socket clientSocket, const GameInfo& info) {
    uint32_t newId;
    {
        std::lock_guard<std::mutex> lock(gamesMutex);
        newId = nextGameId++;
        waitingGames.emplace(newId, WaitingGame(newId, info, std::move(clientSocket)));
    }

    boost::asio::ip::tcp::socket* sock = nullptr;
    {
        std::lock_guard<std::mutex> lock(gamesMutex);
        auto it = waitingGames.find(newId);
        if (it != waitingGames.end()) sock = &it->second.clientSocket;
    }
    if (sock) waitForGameStart(*sock, newId);
    return true;
}

bool handleJoinGame(boost::asio::ip::tcp::socket clientSocket, uint8_t wantedId) {
    std::optional<WaitingGame> waitingGame;
    {
        std::lock_guard<std::mutex> lock(gamesMutex);
        auto it = waitingGames.find(wantedId);
        if (it != waitingGames.end()) {
            waitingGame.emplace(std::move(it->second));
            waitingGames.erase(it);
        }
    }
    if (!waitingGame) return false;

    Protocol::sendGameInfo(waitingGame->gameInfo, clientSocket);
    makeGame(*waitingGame, std::move(clientSocket));
    return true;
}
}