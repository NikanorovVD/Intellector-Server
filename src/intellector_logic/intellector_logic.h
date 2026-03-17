#pragma once

#include <vector>

enum class PieceType {
    none = 6,
    progressor = 0,
    liberator = 1,
    intellector = 2,
    dominator = 3,
    defensor = 4,
    agressor = 5
};

enum class Team {
    white = 0,
    black = 1
};

struct Piece;
struct Move;

struct Piece {
    PieceType type;
    Team team;

    Piece();
    Piece(PieceType, Team);
    std::vector<Move> getAvailableMoves(int, int, const std::vector<std::vector<Piece>>&) const;
};

struct PiecePosition {
    Piece piece;
    int x;
    int y;

    PiecePosition();
    PiecePosition(Piece, int, int);
};

struct Move {
    PiecePosition s1Before;
    PiecePosition s2Before;
    PiecePosition s1After;
    PiecePosition s2After;
};

class Board {
public:
    Board(const std::vector<std::vector<Piece>>&, Team);
    Board();
    std::vector<Move> getAllAvailableMoves() const;
    Team currentTurn() const;
    bool makeMove(Move);
    bool undoMove(Move);
private:
    std::vector<std::vector<Piece>> position_;
    Team currentTurn_;
    void switchTurn();
};