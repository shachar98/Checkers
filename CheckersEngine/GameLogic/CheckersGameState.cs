using CheckersEngine.GameLogic;
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
        private Player m_currPlayer;
        public MovesHandler MovesHandler { get; set; }
        public CheckesMoveState MoveState { get; set; }
        private WinningChecker m_WinningChecker;

        public CheckersGameState(Piece[,] board, Player currPlayer)
        {
            this.Board = board;
            m_currPlayer = currPlayer;
            MovesHandler = new MovesHandler();
            m_WinningChecker = new WinningChecker(MovesHandler);
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
            List<BoardCoordinate> possibleMoves = MovesHandler.GetSuspectMovesDirections(currPiece, isMultipleEating);
            int squareMoveCount = MovesHandler.GetSquaresMoveCount(currPiece, Board);

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
                        MovesHandler.ChangeToQueenIfNeeded(clonedBoard, newPosition);
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
                        MovesHandler.ChangeToQueenIfNeeded(clonedBoard, newPosition);
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
            return m_WinningChecker.IsLost(m_currPlayer, Board);
        }

        public bool IsWin()
        {
            return m_WinningChecker.IsLost(m_currPlayer.GetOtherPlayer(), Board);
        }

        private Player GetPlayer(int playerIndex)
        {
            return playerIndex == 0 ? m_currPlayer : m_currPlayer.GetOtherPlayer();
        }

        private int GetIndex(Player player)
        {
            return player == m_currPlayer ? 0 : 1;
        }
    }
}
