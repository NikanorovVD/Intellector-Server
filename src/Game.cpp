#include "Game.h"
#include "Protocol.h"
#include "LogWriter.h"
#include <random>

Game::Game(uint32_t id,
           boost::asio::ip::tcp::socket whiteSocket,
           boost::asio::ip::tcp::socket blackSocket,
           const GameInfo &info)
    : id_(id), whiteSocket_(std::make_shared<boost::asio::ip::tcp::socket>(std::move(whiteSocket))),
      blackSocket_(std::make_shared<boost::asio::ip::tcp::socket>(std::move(blackSocket))),
      gameInfo_(info),
      gameActive_(true) {
    if (info.timeControl.maxMinutes > 0) {
        timeController_ = std::make_shared<TimeController>(info.timeControl);
        timeController_->setTimeOutCallback([this](bool team) { sendTimeOut(team); });
    }
}

Game::~Game() {
    gameActive_ = false;
    if (whiteThread_.joinable())
        whiteThread_.join();
    if (blackThread_.joinable())
        blackThread_.join();
}

void Game::sendTeams() {
    Protocol::sendCode(0, *whiteSocket_);
    Protocol::sendCode(1, *blackSocket_);
}

void Game::start() {
    auto self = shared_from_this();
    whiteThread_ = std::thread(&Game::manageGame, this, whiteSocket_, blackSocket_, WhiteTeam, timeController_);
    blackThread_ = std::thread(&Game::manageGame, this, blackSocket_, whiteSocket_, BlackTeam, timeController_);
    if (timeController_) {
        timeController_->start();
    }
}

void Game::manageGame(std::shared_ptr<boost::asio::ip::tcp::socket> playerSocket,
                      std::shared_ptr<boost::asio::ip::tcp::socket> opponentSocket,
                      bool team,
                      std::shared_ptr<TimeController> timeController) {
    while (gameActive_) {
        uint8_t code = Protocol::recvCode(*playerSocket);
        if (!gameActive_) break;

        switch (code) {
        case MOVE_CODE: {
            Move move = Protocol::recvMove(*playerSocket);

            if (timeController) {
                if (team == BlackTeam)
                    timeController->blackMakeMove();
                else
                    timeController->whiteMakeMove();
            }

            Protocol::sendMove(move, *opponentSocket);

            if (timeController) {
                int time = team ? timeController->getBlackTime() : timeController->getWhiteTime();
                Protocol::sendTime(time, *playerSocket);
                Protocol::sendTime(time, *opponentSocket);
            }
            break;
        }
        case REMATCH_CODE: {
            Protocol::sendCode(REMATCH_CODE, *opponentSocket);
            team = !team;
            if (timeController)
                timeController->start();
            break;
        }
        case EXIT_CODE: {
            Protocol::sendCode(EXIT_CODE, *opponentSocket);
            if (timeController) timeController->stop();
            gameActive_ = false;
            return;
        }
        default:
            LogWriter::writeLine("Unknown code: " + std::to_string(code));
            gameActive_ = false;
            return;
        }
    }
}

void Game::sendTimeOut(bool team) {
    uint8_t code = (team == BlackTeam) ? BLACK_TIME_OUT_CODE : WHITE_TIME_OUT_CODE;
    Protocol::sendCode(code, *whiteSocket_);
    Protocol::sendCode(code, *blackSocket_);
    gameActive_ = false;
}