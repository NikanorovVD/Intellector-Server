#include "game_logic.h"

using namespace std;

Board::Board(const vector<vector<Piece>>& position, Team currentTurn)
    : position_(position), currentTurn_(currentTurn) {
}

Board::Board() : position_(9), currentTurn_(Team::white) {
    for (size_t i = 0; i < position_.size(); i++) {
        position_[i].resize(7 - (i % 2));
    }
}

vector<Move> Board::getAllAvailableMoves() const {
    //TODO
    return vector<Move>();
}

Team Board::currentTurn() const {
    return currentTurn_;
}

bool Board::makeMove(Move) {
    //TODO
    return false;
}

bool Board::undoMove(Move) {
    //TODO
    return false;
}

void Board::switchTurn() {
    currentTurn_ = (currentTurn_ == Team::white) ? Team::black : Team::white;
}