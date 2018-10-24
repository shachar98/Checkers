using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.GameLogic
{
    public class WinningChecker
    {
        private MovesHandler m_MovesHandler;

        public WinningChecker(MovesHandler movesHandler)
        {
            m_MovesHandler = movesHandler;
        }

        public bool IsLost(Player player, Piece[,] board)
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    var currPiece = board[row, col];
                    if (currPiece == null || currPiece.Player != player)
                        continue;

                    BoardCoordinate currCoordinate = new BoardCoordinate(row, col);
                    if (IsMoveAvilable(currCoordinate, player, board))
                        return false;
                }
            }

            return true;
        }


        private bool IsMoveAvilable(BoardCoordinate currPosition, Player player, Piece[,] originalBoard)
        {
            var currPiece = originalBoard[currPosition.Row, currPosition.Col];
            List<BoardCoordinate> possibleMoves = m_MovesHandler.GetSuspectMovesDirections(currPiece, false);

            foreach (var item in possibleMoves)
            {
                var oponentPosition = item.Add(currPosition);
                if (!oponentPosition.IsInBoard(originalBoard))
                    continue;

                var oponent = originalBoard[oponentPosition.Row, oponentPosition.Col];
                if (oponent == null)
                {
                    return true;
                }

                var newPosition = oponentPosition.Add(item);
                if (!newPosition.IsInBoard(originalBoard))
                    continue;

                else if (oponent != null && oponent.Player != player && originalBoard[newPosition.Row, newPosition.Col] == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}