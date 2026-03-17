#include "Settings.h"
#include <fstream>
#include <sstream>
#include <iostream>

Settings loadSettingsFromFile(const std::string& filename) {
    Settings settings;
    std::ifstream file(filename);
    if (!file.is_open()) {
        throw std::runtime_error("Fail to open settings file");
    }

    std::string line;
    while (std::getline(file, line)) {
        if (line.empty() || line[0] == '#' || line[0] == ';')
            continue;
        auto pos = line.find('=');
        if (pos == std::string::npos)
            continue;
        std::string key = line.substr(0, pos);
        std::string value = line.substr(pos + 1);

        auto trim = [](std::string& s) {
            s.erase(0, s.find_first_not_of(" \t\r\n"));
            s.erase(s.find_last_not_of(" \t\r\n") + 1);
        };
        trim(key);
        trim(value);

        if (key == "Password") {
            settings.password = value;
        } else if (key == "Version") {
            settings.version = std::stoi(value); 
        }
    }
    return settings;
}