#ifndef SETTINGS_H
#define SETTINGS_H

#include <string>

class Settings {
public:
    std::string password;
    int version;
    int port = 8080;
    Settings() = default;
};

Settings loadSettingsFromFile(const std::string& filename);

#endif