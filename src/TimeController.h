#ifndef TIMECONTROLLER_H
#define TIMECONTROLLER_H

#include <cstdint>
#include <thread>
#include <atomic>
#include <chrono>
#include <functional>
#include "GameInfo.h"

class TimeController {
public:
    using TimeOutCallback = std::function<void(bool team)>;

    TimeController(const TimeControl& tc);
    ~TimeController();

    void start();
    void stop();

    void whiteMakeMove();
    void blackMakeMove();

    int getWhiteTime() const { return whiteTime_; }
    int getBlackTime() const { return blackTime_; }

    void setTimeOutCallback(TimeOutCallback cb) { timeOutCallback_ = cb; }

private:
    void subtractWhiteTime(int elapsedMs);
    void subtractBlackTime(int elapsedMs);
    void checkTimeLoop();

    int maxTime_;
    int addedTime_;
    int whiteTime_;
    int blackTime_;
    std::chrono::steady_clock::time_point lastMoveTime_;
    std::atomic<bool> turn_;
    std::atomic<bool> timeRunning_;
    std::atomic<bool> gameAlive_;

    std::thread checkerThread_;
    TimeOutCallback timeOutCallback_;
};

#endif