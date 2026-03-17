#ifndef MATCHMAKING_H
#define MATCHMAKING_H

#include <cstdint>
#include <unordered_map>
#include <mutex>
#include <memory>
#include <vector>
#include <boost/asio.hpp>
#include "WaitingGame.h"
#include "Game.h"

namespace Matchmaking {

extern std::unordered_map<uint32_t, WaitingGame> waitingGames;
extern uint32_t nextGameId;
extern std::mutex gamesMutex;
extern std::vector<std::shared_ptr<Game>> activeGames;

void sendGamesList(boost::asio::ip::tcp::socket& clientSocket);
void waitForGameStart(boost::asio::ip::tcp::socket& clientSocket, uint32_t gameId);
void makeGame(WaitingGame& waitingGame, boost::asio::ip::tcp::socket&& newClientSocket);

bool handleCreateGame(boost::asio::ip::tcp::socket clientSocket, const GameInfo& info);
bool handleJoinGame(boost::asio::ip::tcp::socket clientSocket, uint8_t wantedId);
}

#endif