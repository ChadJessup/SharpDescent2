using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;

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

    public void bm_pixel(ref grs_bitmap bm, int x, int y, byte color)
    {
        if ((x < 0) || (y < 0) || (x >= bm.bm_w) || (y >= bm.bm_h))
        {
            return;
        }

        switch (bm.bm_type)
        {
            case BM.LINEAR:
                // bm.bm_data.Span[bm.bm_rowsize * y + x] = color;
                return;
            case BM.MODEX:
                x += bm.bm_x;
                y += bm.bm_y;
                // gr_modex_setplane(x & 3);
                // gr_video_memory[(bm.bm_rowsize * y) + (x / 4)] = color;
                return;
            case BM.SVGA:
                // gr_vesa_pixel(color, bm.bm_data.Span[0] + (uint)bm.bm_rowsize * y + x);
                return;
        }
    }
}
