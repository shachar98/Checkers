using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    /// <summary>
    /// This class handle the moving logic and possibilities in checkers game
    /// </summary>
    public class MovesHandler : IMovesHandler
    {
        private MovesHandler()
        {
        }

        public static MovesHandler Instance { get; } = new MovesHandler();

        /// <summary>
        /// This method checks the piece, and reurn the amount of possible moves
        /// </summary>
        /// <param name="piece">The current piece</param>
        /// <param name="board">The current board</param>
        /// <returns>The amount of possible squares the piece can move</returns>
        public int GetSquaresMoveCount(Piece piece, Piece[,] board)
        {
            return piece.PieceType == PieceType.Regular ? 1 : board.GetLength(0);
        }

        /// <summary>
        /// This method check the piece type, player, and is continues eating, and return the possbile directions
        /// </summary>
        /// <param name="piece">The current piece</param>
        /// <param name="isMultipleEating">True if continues eating. Otherwise false</param>
        /// <returns>The possible moving directions</returns>
        public List<BoardCoordinate> GetMovesDirections(Piece piece, bool isMultipleEating)
        {
            // In those cases, we can move to all directions
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

        /// <summary>
        /// Checking if the player can move
        /// This method is similar To GetNextActions, but with better preformence for these case - 
        /// It's similiiar to using any() instead of count() > 0
        /// </summary>
        /// <param name="player">The moving player</param>
        /// <param name="gameState">The current state</param>
        /// <returns>True if the player can move. Otherwise false</returns>
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

        /// <summary>
        /// The method gets a player and the current state, and return the new possible states
        /// We over on all the cells, and check for each one if it has player's piece, and if yes we add all the possible new boards it can create
        /// at the end, we check if any player could eat - if so, we must return a board the eating happened.
        /// </summary>
        /// <param name="player">The moving player</param>
        /// <param name="gameState">The current state</param>
        /// <returns>The new possible states</returns>
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

            // In case of eating, piece count will the different the original. if there are many - we want to return only those states
            var piecesCount = gameState.Board.PiecesCount();
            var eatingStates = newStates.Where(_ => _.Board.PiecesCount() != piecesCount).ToList();
            if (eatingStates.Count != 0)
                return eatingStates;

            return newStates;
        }

        /// <summary>
        /// Get all the options of the moving from the current position
        /// This method is recursive because of multiple eating
        /// </summary>
        /// <param name="currPosition">The position which we check</param>
        /// <param name="player">The player that moves</param>
        /// <param name="isMultipleEating">true if it is continues eationg. false otherwise</param>
        /// <param name="currGameState">Curr state</param>
        /// <returns>All the possible new states from this position</returns>
        private List<CheckersGameState> GetMovesOptions(BoardCoordinate currPosition, Player player, bool isMultipleEating, CheckersGameState currGameState)
        {
            List<CheckersGameState> newBoards = new List<CheckersGameState>();

            Piece currPiece = currGameState.Board[currPosition.Row, currPosition.Col];
            List<BoardCoordinate> possibleMoves = GetMovesDirections(currPiece, isMultipleEating);
            int squareMoveCount = GetSquaresMoveCount(currPiece, currGameState.Board);

            // Over on all possible direction
            foreach (var direction in possibleMoves)
            {
                // Over on the direction by the piece movement capability. This is for handling queens
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

                    // This case means regular movement. can't happened in continues eating
                    if (newPoistionPiece == null && !isMultipleEating)
                    {
                        CheckersGameState newMoveState = MovePiece(currPosition, currGameState, newPosition, direction);
                        newBoards.Add(newMoveState);
                        continue;
                    }

                    newPosition = newPosition.Add(direction);

                    /// Checking if eating is possible
                    if (newPosition.IsInBoard(currGameState.Board) && newPoistionPiece != null && 
                        newPoistionPiece.Player != player && currGameState.Board[newPosition.Row, newPosition.Col] == null)
                    {
                        CheckersGameState newMoveState = MovePiece(currPosition, currGameState, newPosition, direction);

                        // In case of eating, we should check if continues eating aplly
                        newBoards.AddRange(GetMovesOptions(newPosition, player, true, newMoveState));
                    }
                }
            }

            // If new move found, and we are in case of continues eating, we want to return the current state, 
            // because move alredy happened (the first eating). Otherwise, noting hppaened, so we want to return empty list
            if (!newBoards.Any() && isMultipleEating)
                newBoards.Add(currGameState);

            return newBoards;
        }

        /// <summary>
        /// Moving piece from one space to another, and apllying the canges on the board
        /// </summary>
        /// <param name="currPosition">The current piece position</param>
        /// <param name="currGameState">The current Game State</param>
        /// <param name="newPosition">The new piece position</param>
        /// <param name="direction">The moving direction</param>
        /// <returns>The state after the turn</returns>
        private CheckersGameState MovePiece(BoardCoordinate currPosition, CheckersGameState currGameState, BoardCoordinate newPosition, BoardCoordinate direction)
        {
            Piece[,] clonedBoard = currGameState.Board.CloneBoard();
            
            // moving the current piece
            clonedBoard[newPosition.Row, newPosition.Col] = clonedBoard[currPosition.Row, currPosition.Col];
            ChangeToQueenIfNeeded(clonedBoard, newPosition);
            clonedBoard[currPosition.Row, currPosition.Col] = null;

            // Handle eating. if eating didn;t happend, it will do nothing
            var oponentPosition = newPosition.Substract(direction);
            clonedBoard[oponentPosition.Row, oponentPosition.Col] = null;

            // Create and return new state
            var newMoveState = currGameState.CloneWithNewState(clonedBoard);
            return newMoveState;
        }
    }
}
