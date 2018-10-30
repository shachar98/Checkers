using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    /// <summary>
    /// This class handle user movement in checkers game
    /// </summary>
    public class UserGameEngine : IUserGameEngine
    {
        MovesHandler m_MovesHandler;
        public UserGameEngine(MovesHandler movesHandler)
        {
            m_MovesHandler = movesHandler;
        }

        /// <summary>
        /// This method checks if a givven move is valid by the checkers ruless
        /// </summary>
        /// <param name="start">The starting moving position</param>
        /// <param name="end">The finish moving position</param>
        /// <param name="board">The current Board</param>
        /// <returns>True if the move is valid. Otherwise false</returns>
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
            var movesDirections = m_MovesHandler.GetMovesDirections(movingPiece, isContinuesEating);
            if (!movesDirections.Contains(direction))
                return false;

            if (board[end.Row, end.Col] != null)
                return false;

            bool mustEat = m_MovesHandler.GetNextActions(movingPiece.Player, new CheckersGameState(board, movingPiece.Player)).First().Board.PiecesCount() != board.PiecesCount();
            if (rowDistance == 1 && !isContinuesEating && !mustEat)
                return true;

            for (int i = 1; i < distance.Row - 1; i++)
            {
                BoardCoordinate middleCoordinate = start.Add(direction.Multiply(i));
                if (board[middleCoordinate.Row, middleCoordinate.Col] != null)
                    return false;
            }

            BoardCoordinate oponentPosition = start.Add(direction.Multiply(rowDistance - 1));
            var possibleOponent = board[oponentPosition.Row, oponentPosition.Col];
            if (possibleOponent == null && !isContinuesEating && !mustEat && rowDistance <= maxMovesCount)
                return true;

            if (possibleOponent != null && possibleOponent.Player != movingPiece.Player)
                return true;

            return false;
        }

        /// <summary>
        /// The method checks if the piece in the board position can continue eating, by checking all posibilities and using IsValid
        /// </summary>
        /// <param name="board">The current board</param>
        /// <param name="position">The checking piece position</param>
        /// <returns>True if the piece can continue eating. Otherwise false</returns>
        public bool CanContinueEat(Piece[,] board, BoardCoordinate position)
        {
            Piece piece = board[position.Row, position.Col];
            int movesCount = m_MovesHandler.GetSquaresMoveCount(piece, board);
            var movesDirections = m_MovesHandler.GetMovesDirections(piece, true);
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
