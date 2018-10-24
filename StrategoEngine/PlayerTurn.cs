using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoEngine
{
    public class PlayerTurn
    {
        public PlayerTurn(Player player, BoardCoordinate originalPosition, BoardCoordinate finalPosition,
            GameCell[,] preTurnBoard, GameCell[,] afterTurnBoard)
        {
            PreTurnBoard = preTurnBoard;
            AfterTurnBoard = afterTurnBoard;
            Player = player;
            OriginalPosition = originalPosition;
            FinalPosition = finalPosition;
        }

        public Player Player { get; set; }
        public BoardCoordinate OriginalPosition { get; }
        public BoardCoordinate FinalPosition { get; }
        public GameCell[,] AfterTurnBoard { get; }
        public GameCell[,] PreTurnBoard { get; }
    }
}
