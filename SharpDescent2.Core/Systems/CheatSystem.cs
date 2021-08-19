using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public class CheatSystem : IGamePlatformManager
{
    public bool IsInitialized { get; }
    public bool john_head_on { get; internal set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> Initialize()
    {
        throw new NotImplementedException();
    }
}
