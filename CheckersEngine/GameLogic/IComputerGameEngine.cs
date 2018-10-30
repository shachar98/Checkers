using GameEnginesCommon;

namespace CheckersEngine
{
    public interface IComputerGameEngine
    {
        /// <summary>
        /// Returns The current state
        /// </summary>
        CheckersGameState GameState { get; }

        /// <summary>
        /// Make the next move
        /// </summary>
        /// <param name="player">The player who makes the move</param>
        /// <returns>The new move</returns>
        IGameState<CheckersGameState> Play(Player player);
    }
}