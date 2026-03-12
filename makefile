CXX       = g++
CXXFLAGS  = -std=c++20 -Wall -Wextra -Igame_logic
TARGET    = game
BUILD_DIR = build

SOURCES   = main.cpp Board.cpp Piece.cpp
HEADERS   = game_logic.h
OBJECTS   = $(addprefix $(BUILD_DIR)/, $(SOURCES:.cpp=.o))

ifeq ($(OS),Windows_NT)
    RMDIR    = rmdir /s /q
    MKDIR    = mkdir
else
    RMDIR    = rm -rf
    MKDIR    = mkdir -p
endif

vpath %.cpp . game_logic
vpath %.h   game_logic

all: $(BUILD_DIR) $(TARGET)

$(BUILD_DIR):
	$(MKDIR) $@

$(TARGET): $(OBJECTS)
	$(CXX) $(CXXFLAGS) -o $(BUILD_DIR)/$@ $^

$(BUILD_DIR)/%.o: %.cpp $(HEADERS) | $(BUILD_DIR)
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	$(RMDIR) $(BUILD_DIR)

run: $(TARGET)
	$(BUILD_DIR)/$(TARGET)

.PHONY: all clean run