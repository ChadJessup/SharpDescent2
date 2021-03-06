namespace Sharp.Platform.Interfaces
{
    public interface IGamePlatformManager : IDisposable
    {
        ValueTask<bool> Initialize();
        bool IsInitialized { get; }
    }
}
