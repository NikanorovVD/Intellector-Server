#include "LogWriter.h"
#include <iostream>
#include <fstream>
#include <chrono>
#include <ctime>
#include <iomanip>
#include <sstream>
#include <thread>
#include <sys/stat.h>
#ifdef _WIN32
#include <direct.h>   
#endif

std::mutex LogWriter::mutex_;

void LogWriter::writeLine(const std::string &msg) {
    std::lock_guard<std::mutex> lock(mutex_);
    auto now = std::chrono::system_clock::now();
    auto in_time_t = std::chrono::system_clock::to_time_t(now);
    std::tm tm;

#ifdef _WIN32
    localtime_s(&tm, &in_time_t);
#else
    localtime_r(&in_time_t, &tm);
#endif

    std::ostringstream oss;
    oss << "[" << std::put_time(&tm, "%Y-%m-%d %H:%M:%S") << "] "
        << "[" << std::this_thread::get_id() << "] " << msg;

    std::cout << oss.str() << std::endl;

#ifdef _WIN32
    _mkdir("logs");
#else
    mkdir("logs", 0755);
#endif

    std::ostringstream filename;
    filename << "logs/" << std::put_time(&tm, "%Y-%m-%d") << ".log";

    std::ofstream file(filename.str(), std::ios::app);
    if (file.is_open()) {
        file << oss.str() << std::endl;
    }
}

void LogWriter::writeLine(const char *msg) {
    writeLine(std::string(msg));
}