using GameEnginesCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoEngine
{
    public class GameEngine
    {
        List<PlayerTurn> allBoards = new List<PlayerTurn>();
        public GameCell[,] Board;
        private readonly Dictionary<PieceType, int> m_PiecesValues = GetPiecesValues();
        private readonly Dictionary<PieceType, int> m_PiecesScores = GetPiecesScores();
        public event EventHandler<GameFinishedEventArgs> GameFinished;
        public bool m_IsGameFinished;

        public GameEngine(int rows = 10, int columns = 10, bool shouldInitializeBoard = true)
        {
            Board = new BoardInitializer().InitGameBoard(rows, columns);
        }

        public void Play(Player player)
        {
            if (m_IsGameFinished)
                return;

            GameCell emptyCell = new GameCell() { IsEmpty = true, IsWater = false, Piece = null };
            
            PlayerTurn bestTurn = null;
            //(BoardCoordinate original, BoardCoordinate newPos) bestMove = (original: null, newPos: null);
            double bestScore = int.MinValue;

            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var currCell = Board[row, col];

                    if (!IsPlayerCell(player, currCell) || !currCell.Piece.IsMoveable)
                        continue;

                    BoardCoordinate currCoordinate = new BoardCoordinate(row, col);
                    IEnumerable<BoardCoordinate> possibleNewPositions = 
                        GetPossibleNewPositions(player, currCell, currCoordinate);

                    foreach (var neighbourCoordinate in possibleNewPositions)
                    {
                        var currNeighbour = Board[neighbourCoordinate.Row, neighbourCoordinate.Col];
                        if (currNeighbour.IsWater ||
                            (currNeighbour.Piece != null && currNeighbour.Piece.PlayerOwner == player))
                            continue;

                        GameCell[,] clonedBoard = GetPossibleNewBoard(emptyCell, currCell, currCoordinate, neighbourCoordinate, currNeighbour);

                        double currScore = CalcScore(clonedBoard, player);
                        if (currScore > bestScore)
                        {
                            bestScore = currScore;
                            bestTurn = new PlayerTurn(player, new BoardCoordinate(row, col), neighbourCoordinate, clonedBoard, Board);
                        }
                    }
                }
            }

            if (bestTurn == null)
            {
                GameFinished?.Invoke(this, new GameFinishedEventArgs(player.GetOtherPlayer()));
            }

            allBoards.Add(bestTurn);
            Board = bestTurn.AfterTurnBoard;

            if (!Board.Cast<GameCell>().Any(_ => _.Piece?.PlayerOwner == player.GetOtherPlayer() && 
                                                 _.Piece?.PieceType == PieceType.Flag))
            {
                GameFinished?.Invoke(this, new GameFinishedEventArgs(player));
            }
        }

        private IEnumerable<BoardCoordinate> GetPossibleNewPositions(Player player, GameCell currCell, BoardCoordinate currCoordinate)
        {
            var possibleNewPositions = currCoordinate.GetBoardNeghibours(Board).ToList();

            foreach (var newCoord in possibleNewPositions.ToList())
            {
                var newCell = Board[newCoord.Row, newCoord.Col];
                if (IsValidPosition(player, newCell))
                    possibleNewPositions.Remove(newCoord);
            }

            if (currCell.Piece.PieceType == PieceType.Two)
            {
                List<BoardCoordinate> newPositions = new List<BoardCoordinate>();
                foreach (var newPos in possibleNewPositions)
                {
                    BoardCoordinate direction = newPos.Substract(currCoordinate);

                    bool seenOponentPiece = false;
                    int distance = 1;
                    while (true)
                    {
                        BoardCoordinate newCoord = currCoordinate.Add(direction.Multiply(distance));
                        if (!newCoord.IsInBoard(Board))
                            break;

                        var newCell = Board[newCoord.Row, newCoord.Col];
                        if (IsValidPosition(player, newCell))
                            break;

                        if (newCell.Piece?.PlayerOwner == player)
                        {
                            if (seenOponentPiece)
                                break;
                            seenOponentPiece = true;
                        }

                        newPositions.Add(newCoord);
                    }
                }

                possibleNewPositions = newPositions;
            }

            return possibleNewPositions;
        }

        private static bool IsValidPosition(Player player, GameCell newCell)
        {
            return newCell.IsWater ||
                                (newCell.Piece != null && newCell.Piece.PlayerOwner == player);
        }

        private GameCell[,] GetPossibleNewBoard(GameCell emptyCell, GameCell currCell, BoardCoordinate currCoordinate, BoardCoordinate neighbourCoordinate, GameCell currNeighbour)
        {
            GameCell[,] clonedBoard = CloneBoard();
            if (currNeighbour.IsEmpty)
            {
                Swap(clonedBoard, neighbourCoordinate, currCoordinate);
                clonedBoard[neighbourCoordinate.Row, neighbourCoordinate.Col].Piece.DidMove = true;
            }
            else
            {
                SimulateAttack(clonedBoard, emptyCell, currCell, currCoordinate, neighbourCoordinate, currNeighbour);
            }

            return clonedBoard;
        }

        private static bool IsPlayerCell(Player player, GameCell currCell)
        {
            return !currCell.IsEmpty && !currCell.IsWater && currCell.Piece.PlayerOwner == player;
        }

        private GameCell[,] CloneBoard()
        {
            GameCell[,] clonedBoard = new GameCell[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    clonedBoard[i, j] = Board[i, j].Clone();
                }
            }

            return clonedBoard;
        }

        private void SimulateAttack(GameCell[,] board, GameCell emptyCell, GameCell currCell, BoardCoordinate currCoordinate, BoardCoordinate neighbourCoordinate, GameCell currNeighbour)
        {
            AttackResults attackResult = SimulateAttack(currCell.Piece.PieceType, currNeighbour.Piece.PieceType);
            board[currCoordinate.Row, currCoordinate.Col] = emptyCell;
            if (attackResult == AttackResults.AttackWon)
            {
                board[neighbourCoordinate.Row, neighbourCoordinate.Col] = currCell;
                board[neighbourCoordinate.Row, neighbourCoordinate.Col].Piece.DidMove = true;
                board[neighbourCoordinate.Row, neighbourCoordinate.Col].Piece.DidExposed = true;
            }
            else if (attackResult == AttackResults.DefenseWon)
            {
                // board[neighbourCoordinate.Row, neighbourCoordinate.Col].Piece.DidMove = true;
                board[neighbourCoordinate.Row, neighbourCoordinate.Col].Piece.DidExposed = true;
            }
            else
            {
                board[neighbourCoordinate.Row, neighbourCoordinate.Col] = emptyCell;
            }
        }

        private double CalcScore(GameCell[,] board, Player player)
        {
            double myScore = 0;
			double oponentScore = 0;
            int minOponentDistance = int.MaxValue;
			Player other = player == Player.Black ? Player.White : Player.Black;
			
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    GameCell currCell = board[row, col];
                    if (currCell.IsEmpty || currCell.IsWater)
                        continue;

                    if (IsPlayerCell(player, currCell))
                    {
                        myScore += GetPieceValue(currCell.Piece);

                        int currDistance = GetDistanceToClosestOponentCoordinate(board, new BoardCoordinate(row, col), player);
                        if (currDistance < minOponentDistance)
                            minOponentDistance = currDistance;
                    }
                    else if (IsPlayerCell(other, currCell))
                    {
                        oponentScore += GetPieceValue(currCell.Piece);
                    }
                }
            }

            return myScore - oponentScore - minOponentDistance / 10.0;
        }

		private double GetPieceValue(Piece piece)
		{
			int pieceValue = m_PiecesScores[piece.PieceType];
			
			double score = pieceValue;
            if (piece.DidExposed)
                score -= Math.Sqrt(pieceValue) / 2;
            else if (piece.DidMove)
                score -= Math.Sqrt(pieceValue) / 100;
			
			return score;
		}
		
        private int GetDistanceToClosestOponentCoordinate(GameCell[,] board, BoardCoordinate currCoordinate, Player player)
        {
            GameCell originalCell = board[currCoordinate.Row, currCoordinate.Col];
            if (!originalCell.Piece.IsMoveable)
                return int.MaxValue;
            Queue<BoardBFS> coordsToVisit = new Queue<BoardBFS>();
            coordsToVisit.Enqueue(new BoardBFS(currCoordinate, 0));
            bool[,] visited = new bool[board.GetLength(0), board.GetLength(1)];
            while (coordsToVisit.Count > 0)
            {
                var nextCoordinate = coordsToVisit.Dequeue();
                if (visited[nextCoordinate.Coordinate.Row, nextCoordinate.Coordinate.Col])
                    continue;

                foreach (var nextNeighbour in nextCoordinate.Coordinate.GetBoardNeghibours(board))
                {
                    GameCell gameCell = board[nextNeighbour.Row, nextNeighbour.Col];
                    if (gameCell.IsWater || gameCell.Piece?.PlayerOwner == player)
                        continue;

                    if (gameCell.IsEmpty)
                        coordsToVisit.Enqueue(new BoardBFS(nextNeighbour, nextCoordinate.Distance + 1));
                    else
                    {
                        if (SimulateAttack(originalCell.Piece.PieceType, gameCell.Piece.PieceType) == AttackResults.AttackWon)
                            return nextCoordinate.Distance + 1;
                    }
                }

                visited[nextCoordinate.Coordinate.Row, nextCoordinate.Coordinate.Col] = true;
            }

            return int.MaxValue;
        }

        private void Swap(GameCell[,] board, BoardCoordinate first, BoardCoordinate second)
        {
            var temp = board[first.Row, first.Col];
            board[first.Row, first.Col] = board[second.Row, second.Col];
            board[second.Row, second.Col] = temp;
        }

        private AttackResults SimulateAttack(PieceType attacker, PieceType defender)
        {
            if (defender == PieceType.Flag)
                return AttackResults.AttackWon;
            else if (defender == PieceType.Bomb)
            {
                if (attacker == PieceType.Three)
                    return AttackResults.AttackWon;
                return AttackResults.DefenseWon;
            }
            else if (attacker == PieceType.One && defender == PieceType.Ten)
                return AttackResults.AttackWon;
            else
            {
                int attackerValue = m_PiecesValues[attacker];
                int defenderValue = m_PiecesValues[defender];
                if (attackerValue > defenderValue)
                    return AttackResults.AttackWon;
                else if (attackerValue < defenderValue)
                    return AttackResults.DefenseWon;
                return AttackResults.Draw;
            }
        }

        private static Dictionary<PieceType, int> GetPiecesValues()
        {
            return new Dictionary<PieceType, int>()
            {
                { PieceType.One, 1 },
                { PieceType.Two, 2 },
                { PieceType.Three, 3 },
                { PieceType.Four, 4 },
                { PieceType.Five, 5 },
                { PieceType.Six, 6 },
                { PieceType.Seven, 7 },
                { PieceType.Eight, 8 },
                { PieceType.Nine, 9 },
                { PieceType.Ten, 10 },
            };
        }

        private static Dictionary<PieceType, int> GetPiecesScores()
        {
            return new Dictionary<PieceType, int>()
            {
                { PieceType.One, 45 },
                { PieceType.Two, 6 },
                { PieceType.Three, 10 },
                { PieceType.Four, 10 },
                { PieceType.Five, 18 },
                { PieceType.Six, 30 },
                { PieceType.Seven, 45 },
                { PieceType.Eight, 70 },
                { PieceType.Nine, 120 },
                { PieceType.Ten, 200 },
                { PieceType.Bomb, 50 },
                { PieceType.Flag, 10000 },
            };
        }
    }

    public class BoardBFS
    {
        public BoardBFS(BoardCoordinate coordinate, int distance)
        {
            Coordinate = coordinate;
            Distance = distance;
        }

        public BoardCoordinate Coordinate { get; set; }
        public int Distance { get; set; }
    }

    public enum AttackResults
    {
        AttackWon,
        DefenseWon,
        Draw
    }
}
