using GameEnginesCommon;

namespace CheckersEngine
{
    public interface IComputerGameEngine
    {
        CheckersGameState GameState { get; }

        IGameState<CheckersGameState> Play(Player player);
    }
}