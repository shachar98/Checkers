namespace GameEnginesCommon
{
    public interface IMoveChooser
    {
        IGameState<T> GetNextMove<T>(IGameState<T> gameState);
    }
}