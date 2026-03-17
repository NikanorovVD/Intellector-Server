CXX       = g++
CXXFLAGS  = -std=c++20 -Wall -Wextra -Isrc
TARGET    = server
BUILD_DIR = build
SRC_DIR   = src

ifdef BOOST_ROOT
    CXXFLAGS += -I$(BOOST_ROOT)
endif

SOURCES = \
    $(SRC_DIR)/main.cpp \
    $(SRC_DIR)/Settings.cpp \
    $(SRC_DIR)/LogWriter.cpp \
    $(SRC_DIR)/Protocol.cpp \
    $(SRC_DIR)/TimeController.cpp \
    $(SRC_DIR)/Game.cpp

HEADERS = \
    $(SRC_DIR)/Common.h \
    $(SRC_DIR)/GameInfo.h \
    $(SRC_DIR)/WaitingGame.h \
    $(SRC_DIR)/Settings.h \
    $(SRC_DIR)/LogWriter.h \
    $(SRC_DIR)/Protocol.h \
    $(SRC_DIR)/TimeController.h \
    $(SRC_DIR)/Game.h

OBJECTS = $(addprefix $(BUILD_DIR)/, $(notdir $(SOURCES:.cpp=.o)))

ifeq ($(OS),Windows_NT)
    RMDIR    = rmdir /s /q
    MKDIR    = mkdir
    LDLIBS   = -pthread -lws2_32
else
    RMDIR    = rm -rf
    MKDIR    = mkdir -p
    LDLIBS   = -pthread
endif

all: $(BUILD_DIR) $(TARGET)

$(BUILD_DIR):
	$(MKDIR) $@

$(TARGET): $(OBJECTS)
	$(CXX) $(CXXFLAGS) -o $(BUILD_DIR)/$@ $^ $(LDLIBS)

$(BUILD_DIR)/%.o: $(SRC_DIR)/%.cpp $(HEADERS) | $(BUILD_DIR)
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	$(RMDIR) $(BUILD_DIR)

run: $(TARGET)
	$(BUILD_DIR)/$(TARGET)

.PHONY: all clean run