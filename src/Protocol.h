#ifndef Protocol_H
#define Protocol_H

#include <cstdint>
#include <string>
#include <boost/asio.hpp>
#include "GameInfo.h"

namespace Protocol {

bool sendCode(uint8_t code, boost::asio::ip::tcp::socket& socket);
uint8_t recvCode(boost::asio::ip::tcp::socket& socket);

bool sendInt(int value, boost::asio::ip::tcp::socket& socket);
int recvInt(boost::asio::ip::tcp::socket& socket);

bool sendString(const std::string& str, boost::asio::ip::tcp::socket& socket);
std::string recvString(boost::asio::ip::tcp::socket& socket);

bool sendGameInfo(const GameInfo& game, boost::asio::ip::tcp::socket& socket);
GameInfo recvGameInfo(boost::asio::ip::tcp::socket& socket);
GameInfo recvGameInfoForCreate(boost::asio::ip::tcp::socket& socket);

bool sendMove(const Move& move, boost::asio::ip::tcp::socket& socket);
Move recvMove(boost::asio::ip::tcp::socket& socket);

bool sendTime(int time, boost::asio::ip::tcp::socket& socket);

}

#endif