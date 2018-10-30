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

        /// <summary>
        /// Cloning a board with copy the pieces
        /// </summary>
        /// <param name="originalBoard">The board to clone</param>
        /// <returns>The cloned board</returns>
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

        /// <summary>
        /// Checking if two boards are equals, by comparing all the cells
        /// </summary>
        /// <param name="firstBoard">The first board</param>
        /// <param name="secondBoard">The second board</param>
        /// <returns>Truw if the board are euals. Otherwise false</returns>
        public static bool BoardsEqual(this Piece[,] firstBoard, Piece[,] secondBoard)
        {
            for (int row = 0; row < firstBoard.GetLength(0); row++)
            {
                for (int col = 0; col < firstBoard.GetLength(1); col++)
                {
                    if (!Equals(firstBoard[row, col], firstBoard[row, col]))
                        return false;
                }
            }

            return true;
        }
    }
}
