using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public static class GameExtensions
    {
        public static Player GetOtherPlayer(this Player player)
        {
            return player == Player.Red ? Player.Blue : Player.Red;
        }

        public static int PiecesCount(this Piece[,] board)
        {
            return board.Cast<Piece>().Count(_ => _ != null);
        }

        public static Piece[,] CloneBoard(this Piece[,] originalBoard)
        {
            Piece[,] newBoard = new Piece[originalBoard.GetLength(0), originalBoard.GetLength(1)];
            for (int row = 0; row < newBoard.GetLength(0); row++)
            {
                for (int col = 0; col < newBoard.GetLength(1); col++)
                {
                    newBoard[row, col] = originalBoard[row, col];
                }
            }

            return newBoard;
        }

        public static bool BoardsEqual(this Piece[,] originalBoard, Piece[,] otherBoard)
        {
            for (int row = 0; row < originalBoard.GetLength(0); row++)
            {
                for (int col = 0; col < originalBoard.GetLength(1); col++)
                {
                    if (!Equals(originalBoard[row, col], originalBoard[row, col]))
                        return false;
                }
            }

            return true;
        }
    }
}
