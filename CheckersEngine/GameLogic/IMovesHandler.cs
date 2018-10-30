using System.Collections.Generic;
using GameEnginesCommon;

namespace CheckersEngine
{
    public interface IMovesHandler
    {
        IEnumerable<CheckersGameState> GetNextActions(Player player, CheckersGameState gameState);
        bool HaveActions(Player player, CheckersGameState gameState);
        List<BoardCoordinate> GetMovesDirections(Piece piece, bool isMultipleEating);
        void ChangeToQueenIfNeeded(Piece[,] clonedBoard, BoardCoordinate newPosition);
        int GetSquaresMoveCount(Piece piece, Piece[,] board);
    }
}