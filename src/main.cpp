#include <iostream>
#include <thread>
#include <csignal>
#include <boost/asio.hpp>
#include "Common.h"
#include "Settings.h"
#include "LogWriter.h"
#include "Protocol.h"
#include "Matchmaking.h"

const std::string settingsFilename = "settings.ini";

bool serverRunning = true;
boost::asio::io_context io_context;
Settings settings;

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

void handleClient(boost::asio::ip::tcp::socket clientSocket) {
    boost::system::error_code ec;
    auto endpoint = clientSocket.remote_endpoint(ec);
    if (!ec) {
        LogWriter::writeLine("Connect with " + endpoint.address().to_string() + ":" + std::to_string(endpoint.port()));
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
            Matchmaking::sendGamesList(clientSocket);
            break;
        }
        case CREATE_GAME_REQUEST: {
            GameInfo info = Protocol::recvGameInfoForCreate(clientSocket);
            Matchmaking::handleCreateGame(std::move(clientSocket), info);
            clientActive = false;
            break;
        }
        case JOIN_GAME_REQUEST: {
            uint8_t wantedId = Protocol::recvCode(clientSocket);
            Matchmaking::handleJoinGame(std::move(clientSocket), wantedId);
            clientActive = false;
            break;
        }
        default:
            LogWriter::writeLine("Unknown code: " + std::to_string(requestCode));
            clientActive = false;
            break;
        }
    }

    boost::system::error_code ec_close;
    clientSocket.close(ec_close);
}

int main() {
    signal(SIGINT, handleShutdownSignal);
    signal(SIGTERM, handleShutdownSignal);

#ifndef _WIN32
    signal(SIGPIPE, SIG_IGN);
#endif

    try {
        settings = loadSettingsFromFile(settingsFilename);

        boost::asio::ip::tcp::acceptor acceptor(io_context,
                                                boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), settings.port));

        LogWriter::writeLine("Server Start");

        while (serverRunning) {
            boost::asio::ip::tcp::socket clientSocket(io_context);
            boost::system::error_code ec;
            acceptor.accept(clientSocket, ec);
            if (ec) {
                if (serverRunning) { LogWriter::writeLine("accept error: " + ec.message()); }
                continue;
            }

            std::thread clientThread(handleClient, std::move(clientSocket));
            clientThread.detach();
        }
    } catch (std::exception& e) { LogWriter::writeLine("Error: " + std::string(e.what())); }

    return 0;
}