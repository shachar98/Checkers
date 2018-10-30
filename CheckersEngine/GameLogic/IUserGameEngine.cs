using GameEnginesCommon;

namespace CheckersEngine
{
    public interface IUserGameEngine
    {
        bool CanContinueEat(Piece[,] board, BoardCoordinate position);
        bool IsValidMove(BoardCoordinate start, BoardCoordinate end, Piece[,] board);
    }
}