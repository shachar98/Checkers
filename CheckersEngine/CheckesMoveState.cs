using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public class CheckesMoveState
    {
        public Piece[,] CurrState { get; set; }
        public List<Piece[,]> MidStates { get; set; } = new List<Piece[,]>();

        public CheckesMoveState CloneWithNewState(Piece[,] newState)
        {
            CheckesMoveState newMoveState = new CheckesMoveState();
            newMoveState.CurrState = newState;
            newMoveState.MidStates.AddRange(MidStates);
            if (CurrState != null)
                newMoveState.MidStates.Add(CurrState);
            return newMoveState;
        }
    }
}
