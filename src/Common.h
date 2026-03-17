#ifndef COMMON_H
#define COMMON_H

#include <cstdint>
#include <mutex>
#include <condition_variable>
#include <queue>
#include <unordered_map>
#include <memory>
#include <chrono>
#include <boost/asio.hpp>

// Protocol constants
// Game
const uint8_t MOVE_CODE = 10;
const uint8_t TIME_CODE = 20;
const uint8_t EXIT_CODE = 111;
const uint8_t REMATCH_CODE = 222;
const uint8_t WHITE_TIME_OUT_CODE = 30;
const uint8_t BLACK_TIME_OUT_CODE = 31;
// MatchMaking
const uint8_t JOIN_GAME_REQUEST = 30;
const uint8_t CREATE_GAME_REQUEST = 40;
const uint8_t GAMES_LIST_REQUEST = 100;
const uint8_t CLIENT_WAITING_REQUEST = 1;
const uint8_t SERVER_WAITING_RESPONSE = 123;
const uint8_t GAME_NOT_EXIST_RESPONSE = 99;

enum class ColorChoice : int {
    white = 0,
    black = 1,
    random = 2
};

enum class Color : bool {
    white = 1,
    black = 0,
};

struct Move {
    uint8_t data[5];

    uint8_t& operator[](size_t i) { return data[i]; }
    const uint8_t& operator[](size_t i) const { return data[i]; }

    std::string toString() const {
        std::string result;
        for (int i = 0; i < 5; ++i) {
            if (i > 0) result += ' ';
            result += std::to_string(data[i]);
        }
        return result;
    };
};

#endif