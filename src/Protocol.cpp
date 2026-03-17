#include "Protocol.h"
#include "Settings.h"
#include "LogWriter.h"
#include <boost/system/error_code.hpp>
#include <vector>
#include <iomanip>
#include <sstream>


#ifdef ENABLE_NETWORK_LOGGING
#define LOG_NET(msg) LogWriter::writeLine(msg)
#define LOG_HEX(data, len) logHex(data, len)
static void logHex(const void* data, size_t len) {
    const uint8_t* bytes = static_cast<const uint8_t*>(data);
    std::ostringstream oss;
    oss << "Hex: ";
    for (size_t i = 0; i < len; ++i) {
        oss << std::hex << std::setw(2) << std::setfill('0') << (int)bytes[i] << " ";
    }
    LogWriter::writeLine(oss.str());
}
#else
#define LOG_NET(msg) do {} while (0)
#define LOG_HEX(data, len) do {} while (0)
#endif

namespace {

bool recvData(boost::asio::ip::tcp::socket& socket, void* buffer, size_t n) {
    boost::system::error_code ec;
    size_t total = 0;
    char* ptr = static_cast<char*>(buffer);
    while (total < n) {
        size_t received = socket.read_some(boost::asio::buffer(ptr + total, n - total), ec);
        if (ec) {
            LOG_NET("recvData error: " + ec.message());
            return false;
        }
        total += received;
    }
    LOG_HEX(buffer, n);
    return true;
}

bool sendData(boost::asio::ip::tcp::socket& socket, const void* buffer, size_t n) {
    boost::system::error_code ec;
    size_t total = 0;
    const char* ptr = static_cast<const char*>(buffer);
    while (total < n) {
        size_t sent = socket.write_some(boost::asio::buffer(ptr + total, n - total), ec);
        if (ec) {
            LOG_NET("sendData error: " + ec.message());
            return false;
        }
        total += sent;
    }
    LOG_HEX(buffer, n);
    return true;
}

}

bool Protocol::sendCode(uint8_t code, boost::asio::ip::tcp::socket& socket) {
    LOG_NET("sendCode: " + std::to_string(code));
    return sendData(socket, &code, 1);
}

uint8_t Protocol::recvCode(boost::asio::ip::tcp::socket& socket) {
    uint8_t code = 0;
    if (!recvData(socket, &code, 1)) return 0;
    LOG_NET("recvCode: " + std::to_string(code));
    return code;
}

bool Protocol::sendInt(int value, boost::asio::ip::tcp::socket& socket) {
    uint32_t net = static_cast<uint32_t>(value);
    LOG_NET("sendInt: " + std::to_string(value));
    return sendData(socket, &net, 4);
}

int Protocol::recvInt(boost::asio::ip::tcp::socket& socket) {
    uint32_t net = 0;
    if (!recvData(socket, &net, 4)) return 0;
    int value = static_cast<int>(net);
    LOG_NET("recvInt: " + std::to_string(value));
    return value;
}

bool Protocol::sendString(const std::string& str, boost::asio::ip::tcp::socket& socket) {
    uint32_t len = static_cast<uint32_t>(str.size());
    LOG_NET("sendString: length=" + std::to_string(len) + ", content=\"" + str + "\"");
    if (!sendInt(len, socket)) return false;
    if (len > 0 && !sendData(socket, str.data(), len)) return false;
    return true;
}

std::string Protocol::recvString(boost::asio::ip::tcp::socket& socket) {
    uint32_t len = recvInt(socket);
    if (len == 0) return std::string();
    std::vector<char> buf(len);
    if (!recvData(socket, buf.data(), len)) return std::string();
    std::string str(buf.data(), len);
    LOG_NET("recvString: length=" + std::to_string(len) + ", content=\"" + str + "\"");
    return str;
}


bool Protocol::sendGameInfo(const GameInfo& game, boost::asio::ip::tcp::socket& socket) {
    LOG_NET("sendGameInfo: id=" + std::to_string(game.id) + ", name=" + game.name);
    if (!sendInt(game.id, socket)) return false;
    if (!sendString(game.name, socket)) return false;
    if (!sendInt(game.timeControl.maxMinutes, socket)) return false;
    if (!sendInt(game.timeControl.addedSeconds, socket)) return false;
    if (!sendInt(static_cast<int>(game.colorChoice), socket)) return false;
    return true;
}

GameInfo Protocol::recvGameInfo(boost::asio::ip::tcp::socket& socket) {
    GameInfo game;
    game.id = recvInt(socket);
    game.name = recvString(socket);
    game.timeControl.maxMinutes = recvInt(socket);
    game.timeControl.addedSeconds = recvInt(socket);
    game.colorChoice = static_cast<ColorChoice>(recvInt(socket));
    LOG_NET("recvGameInfo: id=" + std::to_string(game.id) + ", name=" + game.name);
    return game;
}

GameInfo Protocol::recvGameInfoForCreate(boost::asio::ip::tcp::socket& socket) {
    GameInfo game;
    game.id = 0;
    game.name = recvString(socket);
    game.timeControl.maxMinutes = recvInt(socket);
    game.timeControl.addedSeconds = recvInt(socket);
    game.colorChoice = static_cast<ColorChoice>(recvInt(socket));
    LOG_NET("recvGameInfoForCreate: name=" + game.name);
    return game;
}


bool Protocol::sendMove(const Move& move, boost::asio::ip::tcp::socket& socket) {
    LOG_NET("sendMove: " + move.toString());
    if (!sendCode(MOVE_CODE, socket)) return false;
    return sendData(socket, move.data, 5);
}

Move Protocol::recvMove(boost::asio::ip::tcp::socket& socket) {
    Move move;
    recvData(socket, move.data, 5);
    LOG_NET("recvMove: " + std::to_string(move[0]) + " " + std::to_string(move[1]) + " " + std::to_string(move[2]) +
            " " + std::to_string(move[3]) + " " + std::to_string(move[4]));
    return move;
}

bool Protocol::sendTime(int time, boost::asio::ip::tcp::socket& socket) {
    LOG_NET("sendTime: " + std::to_string(time));
    if (!sendCode(TIME_CODE, socket)) return false;
    return sendInt(time, socket);
}