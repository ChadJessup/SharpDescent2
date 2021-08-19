using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public class RenderSystem : IGamePlatformManager
{
    public bool IsInitialized { get; }

    public int Clear_window { get; set; } = 2;			//	1 = Clear whole background window, 2 = clear view portals into rest of world, 0 = no clear

    public ValueTask<bool> Initialize()
    {
        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
