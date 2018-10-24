using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacmanEngine
{
    public class PacmanGameState : IBoardGameState<PacmanCell, BoardCoordinate>
    {
        private List<BoardCoordinate> m_AgentsPositions;
        private int m_FoodCount;

        public PacmanGameState()
        {
            Board = InitBoard();
            m_FoodCount = 0;
            m_AgentsPositions = new List<BoardCoordinate>();
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var item = Board[row, col];
                    if (item == PacmanCell.Ghost)
                        m_AgentsPositions.Add(new BoardCoordinate(row, col));

                    if (item == PacmanCell.Pacman)
                    {
                        List<BoardCoordinate> newPositions = new List<BoardCoordinate>
                            { new BoardCoordinate(row, col) };

                        foreach (var currAgent in m_AgentsPositions)
                            newPositions.Add(currAgent);
                        m_AgentsPositions = newPositions;
                    }

                    if (item == PacmanCell.Food)
                        m_FoodCount++;
                }
            }
        }

        public PacmanGameState(PacmanGameState other)
        {
            Board = new PacmanCell[other.Board.GetLength(0), other.Board.GetLength(1)];
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    Board[row, col] = other.Board[row, col];
                }
            }

            m_FoodCount = other.m_FoodCount;
            m_AgentsPositions = new List<BoardCoordinate>();
            foreach (var item in other.m_AgentsPositions)
            {
                m_AgentsPositions.Add(new BoardCoordinate(item));
            }
        }

        private PacmanCell[,] InitBoard()
        {
            return new PacmanCell[,]
            {
                { PacmanCell.Food, PacmanCell.Food, PacmanCell.Wall, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food},
                { PacmanCell.Food, PacmanCell.Wall, PacmanCell.Food, PacmanCell.Wall, PacmanCell.Food, PacmanCell.Food},
                { PacmanCell.Food, PacmanCell.Food, PacmanCell.Pacman, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food },
                { PacmanCell.Food, PacmanCell.Wall, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food},
                { PacmanCell.Food, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food, PacmanCell.Food},
                { PacmanCell.Wall, PacmanCell.Food, PacmanCell.Food, PacmanCell.Wall, PacmanCell.Food, PacmanCell.Food},
            };
        }

        public int NumOfAgents => m_AgentsPositions.Count;

        public PacmanCell[,] Board { get; }

        public double CalcScore()
        {
            if (IsWin())
                return int.MaxValue;

            if (IsLost())
                return int.MinValue;
            int minDistance = GetClosestFood();

            //int foodCount = 0;
            //foreach (var item in Board)
            //{
            //    if (item == PacmanCell.Food)
            //        foodCount++;
            //}

            //if (foodCount != m_FoodCount)
            //    throw new Exception();

            int negativeScore = m_FoodCount * 1000 + minDistance;
            return -negativeScore;
        }

        private int GetClosestFood()
        {
            int minDistance = -1;
            Queue<BoardCoordinate> coordsToVisit = new Queue<BoardCoordinate>();
            coordsToVisit.Enqueue(m_AgentsPositions[0]);
            bool[,] visited = new bool[Board.GetLength(0), Board.GetLength(1)];

            while (coordsToVisit.Count > 0)
            {
                var first = coordsToVisit.Dequeue();
                if (visited[first.Row, first.Col])
                    continue;

                if (Board[first.Row, first.Col] == PacmanCell.Food)
                {
                    minDistance = first.BoardDistance(m_AgentsPositions[0]);
                    break;
                }

                foreach (var item in first.GetBoardNeghibours(Board))
                {
                    if (!IsLegal(item) ||
                        visited[item.Row, item.Col])
                        continue;

                    coordsToVisit.Enqueue(item);
                }

                visited[first.Row, first.Col] = true;
            }

            return minDistance;
        }

        public IEnumerable<BoardCoordinate> GetNextActions(int agentIndex)
        {
            BoardCoordinate coordinate = new BoardCoordinate(m_AgentsPositions[agentIndex]);
            var nextPossibleMoves = coordinate.GetBoardNeghibours(Board);
            foreach (var nextMove in nextPossibleMoves)
            {
                if (!IsLegal(nextMove))
                    continue;

                yield return nextMove;
            }
        }

        public IGameState<BoardCoordinate> GetNextState(int agentIndex, BoardCoordinate nextMove)
        {
            BoardCoordinate coordinate = new BoardCoordinate(m_AgentsPositions[agentIndex]);
            PacmanGameState nextState = new PacmanGameState(this);
            nextState.m_AgentsPositions[agentIndex] = nextMove;
            if (nextState.Board[nextMove.Row, nextMove.Col] == PacmanCell.Food && agentIndex == 0)
                nextState.m_FoodCount--;

            nextState.Board[nextMove.Row, nextMove.Col] = nextState.Board[coordinate.Row, coordinate.Col];
            nextState.Board[coordinate.Row, coordinate.Col] = PacmanCell.Empty;

            return nextState;
        }

        private bool IsLegal(BoardCoordinate coordiante)
        {
            return Board[coordiante.Row, coordiante.Col] != PacmanCell.Wall;
        }

        public bool IsLost()
        {
            if (Board[m_AgentsPositions[0].Row, m_AgentsPositions[0].Col] == PacmanCell.Ghost)
                return true;

            return false;
        }

        public bool IsWin()
        {
            return !IsLost() && m_FoodCount == 0;
        }
    }

    public enum PacmanCell
    {
        Empty,
        Wall,
        Food,
        Ghost,
        WhiteGhost,
        Pacman,
        Magic
    }
}
