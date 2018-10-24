using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    // TODO:
    // * Add win-loss
    // * Improve window UI
    // * Improve preformence & game play
    // * Add human player
    // * Add train against impself

    public class CheckersGameState : IGameState<CheckesMoveState>
    {
        private static Dictionary<string, double> s_ScoresDictionary = new Dictionary<string, double>();
        private Player m_currPlayer;
        private Player m_OpossitePlayer;
        public CheckesMoveState MoveState { get; set; }

        public CheckersGameState(Piece[,] board, Player currPlayer)
        {
            this.Board = board;
            m_currPlayer = currPlayer;
            m_OpossitePlayer = m_currPlayer == Player.Black ? Player.White : Player.Black;
        }

        public CheckersGameState(CheckesMoveState moveState, Player currPlayer)
            :this (moveState.CurrState, currPlayer)
        {
            MoveState = moveState;
        }

        public int NumOfAgents => 2;
        public Piece[,] Board { get; }

        public double CalcScore()
        {
            double score = 0;
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var currPiece = Board[row, col];
                    if (currPiece == null)
                        continue;

                    double pieceScore = currPiece.PieceType == PieceType.Queen ? 1000 : 400;
                    if (currPiece.PieceType == PieceType.Regular)
                    {
                        double rowScore = Math.Min(row, Board.GetLength(0) - row);
                        double colScore = Math.Min(col, Board.GetLength(1) - col);
                        pieceScore -= rowScore * rowScore - colScore;
                    }

                    int factor = currPiece.Player == m_currPlayer ? 1 : -1;
                    score += factor * pieceScore;
                }
            }

            return score;
        }

        public IEnumerable<CheckesMoveState> GetNextActions(int agentIndex)
        {
            List<CheckesMoveState> newStates = new List<CheckesMoveState>();
            Player player = GetPlayer(agentIndex);
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var currPiece = Board[row, col];
                    if (currPiece == null || currPiece.Player != player)
                        continue;

                    BoardCoordinate currCoordinate = new BoardCoordinate(row, col);
                    List<CheckesMoveState> newBoards = GetMovesOptions(currCoordinate, player, Board, false, new CheckesMoveState());
                    newStates.AddRange(newBoards);
                }
            }

            return newStates;
        }

        private List<CheckesMoveState> GetMovesOptions(BoardCoordinate currPosition, Player player, Piece[,] originalBoard, bool isMultipleEating, CheckesMoveState currMoveState)
        {
            List<CheckesMoveState> newBoards = new List<CheckesMoveState>();

            Piece currPiece = originalBoard[currPosition.Row, currPosition.Col];
            List<BoardCoordinate> possibleMoves = GetSuspectMovesDirections(currPiece, isMultipleEating);
            int squareMoveCount = GetSquaresMoveCount(currPiece);

            foreach (var direction in possibleMoves)
            {
                bool canContinueMoving = true;
                for (int i = 1; i <= squareMoveCount && canContinueMoving; i++)
                {
                    BoardCoordinate moveCount = direction.Multiply(i);
                    var newPosition = currPosition.Add(moveCount);
                    if (!newPosition.IsInBoard(originalBoard))
                    {
                        canContinueMoving = false;
                        continue;
                    }

                    var newPoistionPiece = originalBoard[newPosition.Row, newPosition.Col];
                    canContinueMoving = newPoistionPiece == null;

                    if (newPoistionPiece == null && !isMultipleEating)
                    {
                        Piece[,] clonedBoard = CloneBoard(originalBoard);
                        clonedBoard[newPosition.Row, newPosition.Col] = currPiece;
                        clonedBoard[currPosition.Row, currPosition.Col] = null;
                        ChangeToQueenIfNeeded(clonedBoard, newPosition);
                        var newMoveState = currMoveState.CloneWithNewState(clonedBoard);
                        newBoards.Add(newMoveState);
                        continue;
                    }

                    var oponentPosition = newPosition;
                    newPosition = oponentPosition.Add(direction);
                    if (!newPosition.IsInBoard(originalBoard))
                        continue;

                    if (newPoistionPiece != null && newPoistionPiece.Player != player && originalBoard[newPosition.Row, newPosition.Col] == null)
                    {
                        Piece[,] clonedBoard = CloneBoard(originalBoard);
                        clonedBoard[newPosition.Row, newPosition.Col] = clonedBoard[currPosition.Row, currPosition.Col];
                        ChangeToQueenIfNeeded(clonedBoard, newPosition);
                        clonedBoard[currPosition.Row, currPosition.Col] = null;
                        clonedBoard[oponentPosition.Row, oponentPosition.Col] = null;
                        var newMoveState = currMoveState.CloneWithNewState(clonedBoard);
                        newBoards.AddRange(GetMovesOptions(newPosition, player, clonedBoard, true, newMoveState));
                    }
                }
            }

            if (!newBoards.Any() && isMultipleEating)
                newBoards.Add(currMoveState);

            return newBoards;
        }

        private void ChangeToQueenIfNeeded(Piece[,] clonedBoard, BoardCoordinate newPosition)
        {
            Piece piece = clonedBoard[newPosition.Row, newPosition.Col];
            if (newPosition.Row == 7 && piece.Player == Player.Black)
            {
                clonedBoard[newPosition.Row, newPosition.Col] = piece.CloneAsQueen();
            }
            else if (newPosition.Row == 0 && piece.Player == Player.White)
            {
                clonedBoard[newPosition.Row, newPosition.Col] = piece.CloneAsQueen();
            }
        }

        private bool IsMoveAvilable(BoardCoordinate currPosition, Player player, Piece[,] originalBoard)
        {
            var currPiece = originalBoard[currPosition.Row, currPosition.Col];
            List<BoardCoordinate> possibleMoves = GetSuspectMovesDirections(currPiece, false);

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

        private Piece[,] CloneBoard(Piece[,] originalBoard)
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

        public IGameState<CheckesMoveState> GetNextState(int agentIndex, CheckesMoveState action)
        {
            return new CheckersGameState(action, m_currPlayer);
        }

        public bool IsLost()
        {
            return IsLost(m_currPlayer);
        }

        public bool IsLost(Player player)
        {
            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    var currPiece = Board[row, col];
                    if (currPiece == null || currPiece.Player != player)
                        continue;

                    BoardCoordinate currCoordinate = new BoardCoordinate(row, col);
                    if (IsMoveAvilable(currCoordinate, player, Board))
                        return false;
                }
            }

            return true;
        }

        public bool IsWin()
        {
            return IsLost(m_OpossitePlayer);
        }

        private Piece[,] Move()
        {
            foreach (var item in Board)
            {

            }

            return null;
        }

        private static List<BoardCoordinate> GetSuspectMovesDirections(Piece piece, bool isMultipleEating)
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

        private Player GetPlayer(int playerIndex)
        {
            return playerIndex == 0 ? m_currPlayer : m_OpossitePlayer;
        }

        private int GetIndex(Player player)
        {
            return player == m_currPlayer ? 0 : 1;
        }

        private int GetSquaresMoveCount(Piece piece)
        {
            return piece.PieceType == PieceType.Regular ? 1 : 8;
        }
    }
}
