#include "TimeController.h"
#include "LogWriter.h"
#include <thread>
#include <chrono>

using namespace std::chrono;

TimeController::TimeController(const TimeControl& tc)
    : maxTime_(tc.maxMinutes * 60 * 1000), addedTime_(tc.addedSeconds * 1000), whiteTime_(0), blackTime_(0),
      turn_(WhiteTeam), timeRunning_(false), gameAlive_(true) {}

TimeController::~TimeController() {
    stop();
}

void TimeController::start() {
    whiteTime_ = maxTime_;
    blackTime_ = maxTime_;
    lastMoveTime_ = steady_clock::now();
    turn_ = WhiteTeam;
    timeRunning_ = true;
    if (checkerThread_.joinable())
        checkerThread_.join();
    checkerThread_ = std::thread(&TimeController::checkTimeLoop, this);
}

void TimeController::stop() {
    gameAlive_ = false;
    if (checkerThread_.joinable())
        checkerThread_.join();
}

void TimeController::whiteMakeMove() {
    auto now = steady_clock::now();
    int elapsed = duration_cast<milliseconds>(now - lastMoveTime_).count();
    lastMoveTime_ = now;
    subtractWhiteTime(elapsed);
    turn_ = BlackTeam;
}

void TimeController::blackMakeMove() {
    auto now = steady_clock::now();
    int elapsed = duration_cast<milliseconds>(now - lastMoveTime_).count();
    lastMoveTime_ = now;
    subtractBlackTime(elapsed);
    turn_ = WhiteTeam;
}

void TimeController::subtractWhiteTime(int elapsedMs) {
    whiteTime_ -= elapsedMs;
    if (whiteTime_ >= 0) {
        whiteTime_ += addedTime_;
    } else {
        timeRunning_ = false;
        if (timeOutCallback_)
            timeOutCallback_(WhiteTeam);
    }
}

void TimeController::subtractBlackTime(int elapsedMs) {
    blackTime_ -= elapsedMs;
    if (blackTime_ >= 0) {
        blackTime_ += addedTime_;
    } else {
        timeRunning_ = false;
        if (timeOutCallback_)
            timeOutCallback_(BlackTeam);
    }
}

void TimeController::checkTimeLoop() {
    const int LOOK_INTERVAL_MS = 500;
    while (gameAlive_) {
        if (timeRunning_) {
            auto now = steady_clock::now();
            int elapsed = duration_cast<milliseconds>(now - lastMoveTime_).count();
            if (turn_ == BlackTeam) { 
                if (elapsed >= blackTime_) {
                    timeRunning_ = false;
                    if (timeOutCallback_)
                        timeOutCallback_(true);
                    return;
                }
            } else {
                if (elapsed >= whiteTime_) {
                    timeRunning_ = false;
                    if (timeOutCallback_)
                        timeOutCallback_(false);
                    return;
                }
            }
        }
        std::this_thread::sleep_for(milliseconds(LOOK_INTERVAL_MS));
    }
}