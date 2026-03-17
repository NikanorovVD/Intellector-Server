#ifndef GAMEINFO_H
#define GAMEINFO_H

#include <string>
#include "Common.h"

struct TimeControl {
    int maxMinutes;
    int addedSeconds;
    TimeControl(int max = 0, int add = 0) : maxMinutes(max), addedSeconds(add) {}
};

struct GameInfo {
    uint32_t id;
    std::string name;
    TimeControl timeControl;
    ColorChoice colorChoice;
};

#endif