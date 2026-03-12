#include "game_logic.h"
#include <stdexcept>

using namespace std;

namespace {
    vector<Move> getProgressorMoves(int x, int y, Team team, const vector<vector<Piece>>& board) {
        // TODO
        return vector<Move>();
    }

    vector<Move> getLiberatorMoves(int x, int y, Team team, const vector<vector<Piece>>& board) {
        // TODO
        return vector<Move>();
    }

    vector<Move> getIntellectorMoves(int x, int y, Team team, const vector<vector<Piece>>& board) {
        // TODO
        return vector<Move>();
    }

    vector<Move> getDominatorMoves(int x, int y, Team team, const vector<vector<Piece>>& board) {
        // TODO
        return vector<Move>();
    }

    vector<Move> getDefensorMoves(int x, int y, Team team, const vector<vector<Piece>>& board) {
        // TODO
        return vector<Move>();
    }

    vector<Move> getAgressorMoves(int x, int y, Team team, const vector<vector<Piece>>& board) {
        // TODO
        return vector<Move>();
    }
}

Piece::Piece() : type(PieceType::none), team(Team::white) {}

Piece::Piece(PieceType type, Team team) : type(type), team(team) {}

vector<Move> Piece::getAvailableMoves(int x, int y, const vector<vector<Piece>>& board) const {
    switch (type)
    {
    case PieceType::progressor:
        return getProgressorMoves(x, y, team, board);
    case PieceType::liberator:
        return getLiberatorMoves(x, y, team, board);
    case PieceType::intellector:
        return getIntellectorMoves(x, y, team, board);
    case PieceType::dominator:
        return getDominatorMoves(x, y, team, board);
    case PieceType::defensor:
        return getDefensorMoves(x, y, team, board);
    case PieceType::agressor:
        return getAgressorMoves(x, y, team, board);
    case PieceType::none:
    default:
        throw std::runtime_error("Unknown piece type in getAvailableMoves");
    }
}
