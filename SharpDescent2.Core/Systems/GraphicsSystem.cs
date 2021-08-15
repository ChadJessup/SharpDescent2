using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public class GraphicsSystem : IGamePlatformManager
{
    private readonly ILogger<GraphicsSystem> logger;

    public GraphicsSystem(
        ILogger<GraphicsSystem> logger)
    {
        this.logger = logger;
    }

    public bool IsInitialized { get; }

    public ValueTask<bool> Initialize()
    {
        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
