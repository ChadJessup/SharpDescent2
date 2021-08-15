namespace Sharp.Platform.Interfaces
{
    public interface IOSManager : IGamePlatformManager
    {
        ValueTask<bool> Pump(Action gameLoopCallback);
    }
}
