using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEnginesCommon
{

    public class BoardCoordinate
    {
        public BoardCoordinate()
        { }

        public BoardCoordinate(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public BoardCoordinate(BoardCoordinate other)
        {
            this.Row = other.Row;
            this.Col = other.Col;
        }

        public int Row { get; set; }
        public int Col { get; set; }

        public override bool Equals(object obj)
        {
            BoardCoordinate other = obj as BoardCoordinate;
            if (other == null)
                return false;
            return Row == other.Row && Col == other.Col;
        }

        public override int GetHashCode()
        {
            return (Row * 31 + Col) * 37;
        }

        public override string ToString()
        {
            return $"{Row},{Col}";
        }

        public int BoardDistance(BoardCoordinate other)
        {
            return Math.Abs(Row - other.Row) + Math.Abs(Col - other.Col);
        }

        public IEnumerable<BoardCoordinate> GetBoardNeghibours<T>(T[,] board)
        {
            List<BoardCoordinate> suspectNeighbours = new List<BoardCoordinate>()
            {
             new BoardCoordinate(Row + 1, Col),
             new BoardCoordinate(Row - 1, Col),
             new BoardCoordinate(Row, Col + 1),
             new BoardCoordinate(Row, Col - 1),
            };

            return suspectNeighbours.Where(_ => _.IsInBoard(board));
        }

        public bool IsInBoard<T>(T[,] board)
        {
            return Row >= 0 &&
                Col >= 0 &&
                Row < board.GetLength(0) &&
                Col < board.GetLength(1);
        }

        public IEnumerable<BoardCoordinate> GetNighboursWithinExacDistance<T>(T[,] board, int distance)
        {
            for (int i = 0; i <= distance; i++)
            {
                List<BoardCoordinate> coordinates = new List<BoardCoordinate>()
                {
                 new BoardCoordinate(Row + i, Col + distance - i),
                 new BoardCoordinate(Row - i, Col + distance - i),
                 new BoardCoordinate(Row + i, Col - distance + i),
                 new BoardCoordinate(Row - i, Col - distance + i),
                 };

                foreach (var cord in coordinates.Where(_ => _.IsInBoard(board)))
                    yield return cord;
            }
        }

        public IEnumerable<BoardCoordinate> GetNighboursWithinMaxDistance<T>(T[,] board, int maxDistance)
        {
            for (int i = -maxDistance; i <= maxDistance; i++)
            {
                for (int j = -maxDistance + Math.Abs(i); j <= maxDistance - Math.Abs(i); j++)
                {
                    BoardCoordinate newCoordinate = new BoardCoordinate(Row + i, Col + j);
                    if (!newCoordinate.IsInBoard(board))
                        continue;

                    yield return newCoordinate;
                }
            }
        }

        public BoardCoordinate Substract(BoardCoordinate other)
        {
            return new BoardCoordinate(Row - other.Row, Col - other.Col);
        }

        public BoardCoordinate Add(BoardCoordinate other)
        {
            return new BoardCoordinate(Row + other.Row, Col + other.Col);
        }

        public BoardCoordinate Multiply(int mul)
        {
            return new BoardCoordinate(Row * mul, Col * mul);
        }
    }
}
