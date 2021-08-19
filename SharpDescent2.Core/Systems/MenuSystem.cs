using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public class MenuSystem : IGamePlatformManager
{
    private readonly ILogger<MenuSystem> logger;
    private readonly RenderSystem render;

    public MenuSystem(
        ILogger<MenuSystem> logger,
        RenderSystem renderSystem)
    {
        this.logger = logger;
        this.render = renderSystem;
    }

    public bool IsInitialized { get; }

    public ValueTask<bool> Initialize()
    {
        return ValueTask.FromResult(true);
    }

    public void set_detail_level_parameters(DetailLevel detailLevel)
    {

    }

    public void Dispose()
    {
    }
}
