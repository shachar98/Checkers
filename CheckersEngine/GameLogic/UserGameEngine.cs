using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public class UserGameEngine
    {
        MovesHandler m_MovesHandler;
        public UserGameEngine(MovesHandler movesHandler)
        {
            m_MovesHandler = movesHandler;
        }

        public bool IsValidMove(BoardCoordinate start, BoardCoordinate end, Piece[,] board, bool isContinuesEating)
        {
            var movingPiece = board[start.Row, start.Col];
            if (!end.IsInBoard(board))
                return false;

            var distance = end.Substract(start);
            int rowDistance = Math.Abs(distance.Row);
            if (rowDistance != Math.Abs(distance.Col))
                return false;

            int maxMovesCount = m_MovesHandler.GetSquaresMoveCount(movingPiece, board);
            if (distance.Row == 0 || rowDistance > maxMovesCount + 1)
                return false;

            var direction = distance.Divide(rowDistance);
            var movesDirections = m_MovesHandler.GetSuspectMovesDirections(movingPiece, isContinuesEating);
            if (!movesDirections.Contains(direction))
                return false;

            if (board[end.Row, end.Col] != null)
                return false;

            if (rowDistance == 1 && !isContinuesEating)
                return true;

            for (int i = 1; i < distance.Row - 1; i++)
            {
                BoardCoordinate middleCoordinate = start.Add(direction.Multiply(i));
                if (board[middleCoordinate.Row, middleCoordinate.Col] != null)
                    return false;
            }

            BoardCoordinate oponentPosition = start.Add(direction.Multiply(rowDistance - 1));
            var possibleOponent = board[oponentPosition.Row, oponentPosition.Col];
            if (possibleOponent == null && !isContinuesEating && rowDistance <= maxMovesCount)
                return true;

            if (possibleOponent != null && possibleOponent.Player != movingPiece.Player)
                return true;

            return false;
        }

        public bool HaveMoreMoves(Piece[,] board, BoardCoordinate position)
        {
            Piece piece = board[position.Row, position.Col];
            int movesCount = m_MovesHandler.GetSquaresMoveCount(piece, board);
            var movesDirections = m_MovesHandler.GetSuspectMovesDirections(piece, true);
            foreach (var item in movesDirections)
            {
                for (int i = 0; i <= movesCount + 1; i++)
                {
                    var newPosition = position.Add(item.Multiply(i));
                    if (IsValidMove(position, newPosition, board, true))
                        return true;
                }
            }

            return false;
        }
    }
}
