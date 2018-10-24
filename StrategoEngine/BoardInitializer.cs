using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoEngine
{
    public class BoardInitializer
    {
        Random m_Random = new Random();
        public GameCell[,] InitGameBoard(int rows = 10, int columns = 10)
        {
            if (rows != 10 || columns != 10)
                throw new NotSupportedException();

            GameCell[,] gameBoard = new GameCell[rows, columns];
            InitNoPlayerCells(gameBoard);

            InitGameBoard(gameBoard, Player.White);

            FlipGameBoard(gameBoard);

            InitGameBoard(gameBoard, Player.Black);

            return gameBoard;
        }

        private void FlipGameBoard(GameCell[,] gameBoard)
        {
            var lastRow = gameBoard.GetLength(0) - 1;
            for (int row = 0; row <= lastRow / 2; row++)
            {
                for (int column = 0; column < gameBoard.GetLength(1); column++)
                {
                    var temp = gameBoard[row, column];
                    gameBoard[row, column] = gameBoard[lastRow - row, column];
                    gameBoard[lastRow - row, column] = temp;
                }
            }
        }

        private static void InitNoPlayerCells(GameCell[,] gameBoard)
        {
            for (int row = 4; row <= 5; row++)
            {
                for (int column = 0; column < gameBoard.GetLength(1); column++)
                {
                    if (column == 2 || column == 3 || column == 6 || column == 7)
                        gameBoard[row, column] = new GameCell() { IsWater = true };
                    else
                        gameBoard[row, column] = new GameCell() { IsWater = false, IsEmpty = true };
                }
            }
        }

        private void InitGameBoard(GameCell[,] gameBoard, Player player)
        {
            Dictionary<PieceType, int> playerPieces = InitPiecesCounts();
            List<int> possibleRows = new List<int>() { 0, 1, 2, 3 };
            InitFlag(gameBoard, player, playerPieces);
            AddTenNineAndOne(gameBoard, player, playerPieces);
            AddLeftPieces(gameBoard, player, playerPieces);
        }

        private void AddLeftPieces(GameCell[,] gameBoard, Player player, Dictionary<PieceType, int> playerPieces)
        {
            List<PieceType> pieces = new List<PieceType>();
            foreach (var item in playerPieces)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    pieces.Add(item.Key);
                }
            }

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    BoardCoordinate boardCoordinate = new BoardCoordinate(row, col);
                    if (!CellEmpty(gameBoard, boardCoordinate))
                        continue;

                    var newPiece = GetRandomObject(pieces.ToArray());
                    pieces.Remove(newPiece);
                    AddToBoard(gameBoard, playerPieces, player, boardCoordinate, newPiece);
                }
            }
        }

        private int GetCount(GameCell[,] board, PieceType type)
        {
            return board.OfType<GameCell>().Count(_ => _ != null && _.Piece?.PieceType == type);
        }

        private void AddTenNineAndOne(GameCell[,] gameBoard, Player player, Dictionary<PieceType, int> playerPieces)
        {
            int tenColumn = GetRandomObject(1, 2, 3, 4, 5, 6, 7, 8);
            int tenRow = GetRandomObject(1, 2);
            BoardCoordinate tenCoordinate = new BoardCoordinate(tenRow, tenColumn);
            AddToBoard(gameBoard, playerPieces, player, tenCoordinate, PieceType.Ten);

            bool PutNine = false;
            while (!PutNine)
            {
                var possibleNinePositions = 
                    tenCoordinate.GetNighboursWithinExacDistance(gameBoard, 4).Concat(
                     tenCoordinate.GetNighboursWithinExacDistance(gameBoard, 5)).Concat(
                     tenCoordinate.GetNighboursWithinExacDistance(gameBoard, 6)).
                        Where(_ => _.Row == 1 || _.Row == 2).Where(_ => CellEmpty(gameBoard, _)).ToArray();

                if (possibleNinePositions.Length == 0)
                    throw new Exception();

                while (!PutNine)
                {
                    var nineCoordinate = GetRandomObject(possibleNinePositions);
                    int oneDistance = GetRandomObject(1, 2, 2, 2, 3, 3);
                    var posiibleOnePositions = nineCoordinate.GetNighboursWithinExacDistance(gameBoard, oneDistance).Where(_ => _.Row <= nineCoordinate.Row).ToArray();
                    if (posiibleOnePositions.Length == 0)
                        continue;

                    var oneCoordinate = GetRandomObject(posiibleOnePositions);
                    if (!CellEmpty(gameBoard, oneCoordinate))
                        continue;

                    AddToBoard(gameBoard, playerPieces, player, nineCoordinate, PieceType.Nine);
                    AddToBoard(gameBoard, playerPieces, player, oneCoordinate, PieceType.One);
                    PutNine = true;
                }
            }
        }

        private bool CellEmpty(GameCell[,] gameBoard, BoardCoordinate nineCoordinate)
        {
            return gameBoard[nineCoordinate.Row, nineCoordinate.Col] == null ||
                gameBoard[nineCoordinate.Row, nineCoordinate.Col].IsEmpty;
        }

        private T GetRandomObject<T>(params T[] possibleNumbers)
        {
            return possibleNumbers[m_Random.Next(0, possibleNumbers.Length)];
        }

        private void InitFlag(GameCell[,] gameBoard, Player player, Dictionary<PieceType, int> playerPieces)
        {
            int flagColumn = GetRandomObject(0, 1, 2, 7, 8, 9);
            BoardCoordinate flagCoordinate = new BoardCoordinate(0, flagColumn);
            AddToBoard(gameBoard, playerPieces, player, flagCoordinate, PieceType.Flag);

            foreach (var currCoordinate in flagCoordinate.GetBoardNeghibours(gameBoard))
            {
                AddToBoard(gameBoard, playerPieces, player, currCoordinate, PieceType.Bomb);
            }
        }

        private void AddToBoard(GameCell[,] gameBoard, Dictionary<PieceType, int> playerPieces, Player player, BoardCoordinate coordinate, PieceType piece)
        {
            gameBoard[coordinate.Row, coordinate.Col] = new GameCell()
            {
                IsWater = false,
                IsEmpty = false,
                Piece = new Piece()
                {
                    DidExposed = false,
                    DidMove = false,
                    PlayerOwner = player,
                    IsMoveable = piece != PieceType.Bomb && piece != PieceType.Flag,
                    PieceType = piece
                }
            };

            playerPieces[piece]--;
            if (playerPieces[piece] == 0)
                playerPieces.Remove(piece);
        }

        private static Dictionary<PieceType, int> InitPiecesCounts()
        {
            Dictionary<PieceType, int> playerPieces = new Dictionary<PieceType, int>();
            playerPieces.Add(PieceType.Bomb, 6);
            playerPieces.Add(PieceType.Flag, 1);
            playerPieces.Add(PieceType.Ten, 1);
            playerPieces.Add(PieceType.Nine, 1);
            playerPieces.Add(PieceType.Eight, 2);
            playerPieces.Add(PieceType.Seven, 3);
            playerPieces.Add(PieceType.Six, 4);
            playerPieces.Add(PieceType.Five, 4);
            playerPieces.Add(PieceType.Four, 4);
            playerPieces.Add(PieceType.Three, 5);
            playerPieces.Add(PieceType.Two, 8);
            playerPieces.Add(PieceType.One, 1);

            return playerPieces;
        }
    }
}
