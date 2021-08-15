namespace Sharp.Platform.Interfaces
{
    public interface IGameLogic : IGamePlatformManager
    {
        Task<int> GameLoop(CancellationToken token);
    }
}
