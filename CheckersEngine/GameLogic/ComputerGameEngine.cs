using CheckersEngine.Pieces;
using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    /// <summary>
    /// This class represnts the game engine for the computer
    /// </summary>
    public class ComputerGameEngine : IComputerGameEngine
    {
        public CheckersGameState GameState { get; private set; }
        private IMoveChooser m_AlpahBetaAlgorithem;
        public ComputerGameEngine(Level level, Player player)
        {
            m_AlpahBetaAlgorithem = InitAlphaBeta(level);
            GameState = new CheckersGameState(InitBoard(), player);
        }

        private AlphaBetaAlgorithem InitAlphaBeta(Level level)
        {
            if (level == Level.Easy)
                return new AlphaBetaAlgorithem(2);
            else if (level == Level.Medium)
                return new AlphaBetaAlgorithem(3);
            else
                return new AlphaBetaAlgorithem(4);
        }

        /// <summary>
        /// Make the next move
        /// </summary>
        /// <param name="player">The player that moves</param>
        /// <returns>The new state</returns>
        public IGameState<CheckersGameState> Play(Player player)
        {
            CheckersGameState oldGameState = new CheckersGameState(GameState.Board, player);
            CheckersGameState newGameState = m_AlpahBetaAlgorithem.GetNextMove(oldGameState) as CheckersGameState;
            if (newGameState != null)
                GameState = newGameState;
            return newGameState;
        }

        private Piece[,] InitBoard()
        {
            return new Piece[,]
            {
                {null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red) },
                {new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null },
                {null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red), null, new RegularPiece(Player.Red) },
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null },
                {null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue) },
                {new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null, new RegularPiece(Player.Blue), null },
            };
        }
    }
}
