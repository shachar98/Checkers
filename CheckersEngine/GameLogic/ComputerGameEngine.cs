using CheckersEngine.Pieces;
using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    // TODO: change infinte cache to LRUCache

    public class GameEngine
    {
        public CheckesMoveState MoveState { get; private set; }
        private AlphaBetaAlgorithem m_AlpahBetaAlgorithem;
        public GameEngine(Level level)
        {
            m_AlpahBetaAlgorithem = InitAlphaBeta(level);

            MoveState = new CheckesMoveState()
            {
                CurrState = InitBoard()
            };
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

        public IGameState<CheckesMoveState> Play(Player player)
        {
            CheckersGameState oldGameState = new CheckersGameState(MoveState.CurrState, player);
            CheckersGameState newGameState = m_AlpahBetaAlgorithem.GetNextMove(oldGameState) as CheckersGameState;
            if (newGameState != null)
                MoveState = newGameState.MoveState;
            return newGameState;
        }

        private Piece[,] InitBoard()
        {
            return new Piece[,]
            {
                {new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null },
                {null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black) },
                {new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null, new RegularPiece(Player.Black), null },
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, new RegularPiece(Player.White), null, new RegularPiece(Player.White), null, new RegularPiece(Player.White), null, new RegularPiece(Player.White) },
                {new RegularPiece(Player.White), null, new RegularPiece(Player.White), null, new RegularPiece(Player.White), null, new RegularPiece(Player.White), null },
                {null, new RegularPiece(Player.White), null, new RegularPiece(Player.White), null, new RegularPiece(Player.White), null, new RegularPiece(Player.White) },
            };
        }
    }
}
