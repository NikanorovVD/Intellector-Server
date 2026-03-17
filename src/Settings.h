#ifndef SETTINGS_H
#define SETTINGS_H

#include <string>

class Settings {
public:
    std::string password;
    int version;
    Settings() = default;
};

Settings loadSettingsFromFile(const std::string& filename);

#endif