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
        private AlphaBetaAlgorithem alg = new AlphaBetaAlgorithem(3);
        public GameEngine()
        {
            MoveState = new CheckesMoveState();
            MoveState.CurrState = InitBoard();
        }

        public IGameState<CheckesMoveState> Play(Player player)
        {
            CheckersGameState state = alg.GetNextMove(new CheckersGameState(MoveState.CurrState, player)) as CheckersGameState;
            if (state != null)
                MoveState = state.MoveState;
            return state;
        }

        private Piece[,] InitBoard()
        {
            return new Piece[,]
            {
                {new Piece(Player.Black), null, new Piece(Player.Black), null, new Piece(Player.Black), null, new Piece(Player.Black), null },
                {null, new Piece(Player.Black), null, new Piece(Player.Black), null, new Piece(Player.Black), null, new Piece(Player.Black) },
                {new Piece(Player.Black), null, new Piece(Player.Black), null, new Piece(Player.Black), null, new Piece(Player.Black), null },
                {null, null, null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null},
                {null, new Piece(Player.White), null, new Piece(Player.White), null, new Piece(Player.White), null, new Piece(Player.White) },
                {new Piece(Player.White), null, new Piece(Player.White), null, new Piece(Player.White), null, new Piece(Player.White), null },
                {null, new Piece(Player.White), null, new Piece(Player.White), null, new Piece(Player.White), null, new Piece(Player.White) },
            };
        }
    }
}
