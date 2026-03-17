#ifndef LOGWRITER_H
#define LOGWRITER_H

#include <string>
#include <mutex>

class LogWriter {
public:
    static void writeLine(const std::string& msg);
    static void writeLine(const char* msg);

private:
    static std::mutex mutex_;
};

#endif