using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public class MovesHandler
    {
        public int GetSquaresMoveCount(Piece piece, Piece[,] board)
        {
            return piece.PieceType == PieceType.Regular ? 1 : board.GetLength(0);
        }

        public List<BoardCoordinate> GetSuspectMovesDirections(Piece piece, bool isMultipleEating)
        {
            if (isMultipleEating || piece.PieceType == PieceType.Queen)
            {
                return new List<BoardCoordinate>()
                    {
                         new BoardCoordinate(1,1),
                         new BoardCoordinate(1,- 1),
                         new BoardCoordinate(-1,- 1),
                         new BoardCoordinate(-1, 1),
                    };
            }
            // Black goes to max row, white goes to min row
            else if (piece.Player == Player.Black)
            {
                return new List<BoardCoordinate>()
                    {
                         new BoardCoordinate(1,1),
                         new BoardCoordinate(1,- 1),
                    };
            }
            else
            {
                return new List<BoardCoordinate>()
                    {
                         new BoardCoordinate(-1, 1),
                         new BoardCoordinate(-1, -1),
                    };
            }
        }

        public void ChangeToQueenIfNeeded(Piece[,] clonedBoard, BoardCoordinate newPosition)
        {
            Piece piece = clonedBoard[newPosition.Row, newPosition.Col];
            if (newPosition.Row == 7 && piece.Player == Player.Black)
            {
                clonedBoard[newPosition.Row, newPosition.Col] = piece.TurnToQueen();
            }
            else if (newPosition.Row == 0 && piece.Player == Player.White)
            {
                clonedBoard[newPosition.Row, newPosition.Col] = piece.TurnToQueen();
            }
        }
    }
}
