using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public class PowerupSystem : IGamePlatformManager
{
    public bool IsInitialized { get; }

    public ValueTask<bool> Initialize()
    {
        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}

