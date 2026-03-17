#ifndef WAITINGGAME_H
#define WAITINGGAME_H

#include <cstdint>
#include <boost/asio.hpp>
#include "GameInfo.h"

struct WaitingGame {
    GameInfo gameInfo;
    boost::asio::ip::tcp::socket clientSocket;

    WaitingGame() = delete;
    WaitingGame(uint32_t id, const GameInfo& info, boost::asio::ip::tcp::socket&& sock)
        : gameInfo(info), clientSocket(std::move(sock)) {
        gameInfo.id = id;
    }
};

#endif