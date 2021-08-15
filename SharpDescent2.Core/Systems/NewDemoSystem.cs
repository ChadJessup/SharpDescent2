using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public enum ND_STATE
{
    NORMAL = 0,
    RECORDING = 1,
    PLAYBACK = 2,
    PAUSED = 3,
    REWINDING = 4,
    FASTFORWARD = 5,
    ONEFRAMEFORWARD = 6,
    ONEFRAMEBACKWARD = 7,
    PRINTSCREEN = 8,
}

public class NewDemoSystem : IGamePlatformManager
{
    private readonly ILogger<NewDemoSystem> logger;

    public NewDemoSystem(
        ILogger<NewDemoSystem> logger)
    {
        this.logger = logger;
    }

    public bool IsInitialized { get; }
    public GM game_mode { get; internal set; }
    public ND_STATE state { get; internal set; }

    public ValueTask<bool> Initialize()
    {
        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
