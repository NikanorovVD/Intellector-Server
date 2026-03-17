#ifndef GAME_H
#define GAME_H

#include <cstdint>
#include <thread>
#include <atomic>
#include <memory>
#include <boost/asio.hpp>
#include "GameInfo.h"
#include "TimeController.h"

class Game : public std::enable_shared_from_this<Game> {
public:
    Game(uint32_t id,
         boost::asio::ip::tcp::socket whiteSocket,
         boost::asio::ip::tcp::socket blackSocket,
         const GameInfo& info);
    ~Game();

    void start();
    void sendTeams();

private:
    void manageGame(std::shared_ptr<boost::asio::ip::tcp::socket> playerSocket,
                    std::shared_ptr<boost::asio::ip::tcp::socket> opponentSocket,
                    bool team,
                    std::shared_ptr<TimeController> timeController);
    void sendTimeOut(bool team);

    uint32_t id_;
    std::shared_ptr<boost::asio::ip::tcp::socket> whiteSocket_;
    std::shared_ptr<boost::asio::ip::tcp::socket> blackSocket_;
    GameInfo gameInfo_;
    std::shared_ptr<TimeController> timeController_;
    std::thread whiteThread_;
    std::thread blackThread_;
    std::atomic<bool> gameActive_;
};

#endif