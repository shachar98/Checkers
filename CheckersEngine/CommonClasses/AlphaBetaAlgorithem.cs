using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEnginesCommon
{
    /// <summary>
    /// This class represents an alpha - beta algorithem, that search untill the given depth
    /// </summary>
    public class AlphaBetaAlgorithem : IMoveChooser
    {
        private int m_Depth;
        public AlphaBetaAlgorithem(int depth)
        {
            m_Depth = depth;
        }

        /// <summary>
        /// Get the next move the alpa-beta search
        /// </summary>
        /// <typeparam name="T">The action</typeparam>
        /// <param name="gameState">The current state</param>
        /// <returns>The new state</returns>
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
            // In this case, we finished
            if (state.IsWin() || state.IsLost() || m_Depth == depth)
                return state.CalcScore();

            double lowestScore = int.MaxValue;
            foreach (var nextAction in state.GetNextActions(agentIndex))
            {
                var nextState = state.GetNextState(agentIndex, nextAction);
                double score = GetScore(depth, agentIndex, nextState, alpha, betta);
                lowestScore = Math.Min(lowestScore, score);
                betta = Math.Min(lowestScore, betta);

                // Checking if we can stop searching, because we can't reach better score
                if (betta <= alpha)
                    break;
            }

            return lowestScore;
        }

        private double Max<T>(int depth, int agentIndex, IGameState<T> state, double alpha, double betta)
        {
            // In this case, we finished
            if (state.IsWin() || state.IsLost() || m_Depth == depth)
                return state.CalcScore();

            double highestScore = int.MinValue;
            foreach (var nextAction in state.GetNextActions(agentIndex))
            {
                var nextState = state.GetNextState(agentIndex, nextAction);
                double score = GetScore(depth, agentIndex, nextState, alpha, betta);
                highestScore = Math.Max(highestScore, score);
                alpha = Math.Max(highestScore, alpha);

                // Checking if we can stop searching, because we can't reach better score
                if (betta <= alpha)
                    break;
            }

            return highestScore;
        }

        private double GetScore<T>(int depth, int agentIndex, IGameState<T> state, double alpha, double betta)
        {
            // This algorithm supports several enemies.
            // One turn is defined as ont turn of the player, and one turn of all the enemies
            int nextAgent = (agentIndex + 1) % state.NumOfAgents;
            if (nextAgent == 0)
                return Max(depth + 1, nextAgent, state, alpha, betta);
            else
                return Min(depth, nextAgent, state, alpha, betta);
        }
    }
}
