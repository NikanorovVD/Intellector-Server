#include "game_logic.h"

PiecePosition::PiecePosition() : piece(Piece()), x(0), y(0) {}

PiecePosition::PiecePosition(Piece piece, int x, int y) : piece(piece), x(x), y(y)  {}