using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEnginesCommon
{
    public class AlphaBetaAlgorithem : IMoveChooser
    {
        private int m_Depth;
        public AlphaBetaAlgorithem(int depth)
        {
            m_Depth = depth;
        }

        public IGameState<T> GetNextMove<T>(IGameState<T> gameState)
        {
            double highestScore = int.MinValue;
            IGameState<T> bestMove = null;
            foreach (var nextAction in gameState.GetNextActions(0))
            {
                var nextState = gameState.GetNextState(0, nextAction);
                double score = GetScore(0, 0, nextState, int.MinValue, int.MaxValue);
                if (score >= highestScore)
                {
                    bestMove = nextState;
                    highestScore = score;
                }
            }

            return bestMove;
        }

        private double Min<T>(int depth, int agentIndex, IGameState<T> state, double alpha, double betta)
        {
            if (state.IsWin() || state.IsLost() || m_Depth == depth)
                return state.CalcScore();

            double lowestScore = int.MaxValue;
            foreach (var nextAction in state.GetNextActions(agentIndex))
            {
                var nextState = state.GetNextState(agentIndex, nextAction);
                double score = GetScore(depth, agentIndex, nextState, alpha, betta);
                lowestScore = Math.Min(lowestScore, score);
                betta = Math.Min(lowestScore, betta);
                if (betta <= alpha)
                    break;
            }

            return lowestScore;
        }

        private double Max<T>(int depth, int agentIndex, IGameState<T> state, double alpha, double betta)
        {
            if (state.IsWin() || state.IsLost() || m_Depth == depth)
                return state.CalcScore();

            double highestScore = int.MinValue;
            foreach (var nextAction in state.GetNextActions(agentIndex))
            {
                var nextState = state.GetNextState(agentIndex, nextAction);
                double score = GetScore(depth, agentIndex, nextState, alpha, betta);
                highestScore = Math.Max(highestScore, score);
                alpha = Math.Max(highestScore, alpha);
                if (betta <= alpha)
                    break;
            }

            return highestScore;
        }

        private double GetScore<T>(int depth, int agentIndex, IGameState<T> state, double alpha, double betta)
        {
            int nextAgent = (agentIndex + 1) % state.NumOfAgents;
            if (nextAgent == 0)
                return Max(depth + 1, nextAgent, state, alpha, betta);
            else
                return Min(depth, nextAgent, state, alpha, betta);
        }
    }
}
