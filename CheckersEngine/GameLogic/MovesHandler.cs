using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public class MovesHandler : IMovesHandler
    {
        private MovesHandler()
        {
        }

        public static MovesHandler Instance { get; } = new MovesHandler();

        public int GetSquaresMoveCount(Piece piece, Piece[,] board)
        {
            return piece.PieceType == PieceType.Regular ? 1 : board.GetLength(0);
        }

        public List<BoardCoordinate> GetMovesDirections(Piece piece, bool isMultipleEating)
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
            // Red goes to max row, blue goes to min row
            else if (piece.Player == Player.Red)
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
            if (newPosition.Row == 7 && piece.Player == Player.Red)
            {
                clonedBoard[newPosition.Row, newPosition.Col] = piece.TurnToQueen();
            }
            else if (newPosition.Row == 0 && piece.Player == Player.Blue)
            {
                clonedBoard[newPosition.Row, newPosition.Col] = piece.TurnToQueen();
            }
        }

        public bool HaveActions(Player player, CheckersGameState gameState)
        {
            List<CheckersGameState> newStates = new List<CheckersGameState>();
            for (int row = 0; row < gameState.Board.GetLength(0); row++)
            {
                for (int col = 0; col < gameState.Board.GetLength(1); col++)
                {
                    var currPiece = gameState.Board[row, col];
                    if (currPiece == null || currPiece.Player != player)
                        continue;

                    BoardCoordinate currCoordinate = new BoardCoordinate(row, col);
                    if (GetMovesOptions(currCoordinate, player, false, gameState).Any())
                        return true;
                }
            }

            return false;
        }

        public IEnumerable<CheckersGameState> GetNextActions(Player player, CheckersGameState gameState)
        {
            List<CheckersGameState> newStates = new List<CheckersGameState>();
            for (int row = 0; row < gameState.Board.GetLength(0); row++)
            {
                for (int col = 0; col < gameState.Board.GetLength(1); col++)
                {
                    var currPiece = gameState.Board[row, col];
                    if (currPiece == null || currPiece.Player != player)
                        continue;

                    BoardCoordinate currCoordinate = new BoardCoordinate(row, col);
                    List<CheckersGameState> newBoards = GetMovesOptions(currCoordinate, player, false, gameState);
                    newStates.AddRange(newBoards);
                }
            }

            var piecesCount = gameState.Board.PiecesCount();
            var eatingStates = newStates.Where(_ => _.Board.PiecesCount() != piecesCount).ToList();
            if (eatingStates.Count != 0)
                return eatingStates;

            return newStates;
        }

        private List<CheckersGameState> GetMovesOptions(BoardCoordinate currPosition, Player player, bool isMultipleEating, CheckersGameState currGameState)
        {
            List<CheckersGameState> newBoards = new List<CheckersGameState>();

            Piece currPiece = currGameState.Board[currPosition.Row, currPosition.Col];
            List<BoardCoordinate> possibleMoves = GetMovesDirections(currPiece, isMultipleEating);
            int squareMoveCount = GetSquaresMoveCount(currPiece, currGameState.Board);

            foreach (var direction in possibleMoves)
            {
                bool canContinueMoving = true;
                for (int i = 1; i <= squareMoveCount && canContinueMoving; i++)
                {
                    BoardCoordinate moveCount = direction.Multiply(i);
                    var newPosition = currPosition.Add(moveCount);
                    if (!newPosition.IsInBoard(currGameState.Board))
                    {
                        canContinueMoving = false;
                        continue;
                    }

                    var newPoistionPiece = currGameState.Board[newPosition.Row, newPosition.Col];
                    canContinueMoving = newPoistionPiece == null;

                    if (newPoistionPiece == null && !isMultipleEating)
                    {
                        CheckersGameState newMoveState = MovePiece(currPosition, currGameState, newPosition, direction);
                        newBoards.Add(newMoveState);
                        continue;
                    }

                    newPosition = newPosition.Add(direction);
                    if (!newPosition.IsInBoard(currGameState.Board))
                        continue;

                    if (newPoistionPiece != null && newPoistionPiece.Player != player && currGameState.Board[newPosition.Row, newPosition.Col] == null)
                    {
                        CheckersGameState newMoveState = MovePiece(currPosition, currGameState, newPosition, direction);
                        newBoards.AddRange(GetMovesOptions(newPosition, player, true, newMoveState));
                    }
                }
            }

            if (!newBoards.Any() && isMultipleEating)
                newBoards.Add(currGameState);

            return newBoards;
        }

        private CheckersGameState MovePiece(BoardCoordinate currPosition, CheckersGameState currGameState, BoardCoordinate newPosition, BoardCoordinate direction)
        {
            Piece[,] clonedBoard = currGameState.Board.CloneBoard();
            clonedBoard[newPosition.Row, newPosition.Col] = clonedBoard[currPosition.Row, currPosition.Col];
            ChangeToQueenIfNeeded(clonedBoard, newPosition);
            clonedBoard[currPosition.Row, currPosition.Col] = null;
            var oponentPosition = newPosition.Substract(direction);
            clonedBoard[oponentPosition.Row, oponentPosition.Col] = null;
            var newMoveState = currGameState.CloneWithNewState(clonedBoard);
            return newMoveState;
        }
    }
}
