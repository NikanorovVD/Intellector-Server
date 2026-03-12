#include <iostream>
#include "game_logic.h"

using namespace std;

int main() {
    Board board;
    cout << "Current turn: " << static_cast<int>(board.currentTurn()) << endl;

    vector<Move> moves = board.getAllAvailableMoves();
    cout << "Available moves: " << moves.size() << endl;

    return 0;
}