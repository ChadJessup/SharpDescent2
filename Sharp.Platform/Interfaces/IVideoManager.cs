using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Sharp.Platform.Interfaces;

public interface IVideoManager : IGamePlatformManager
{
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static Rgba32 AlphaPixel = new(255, 255, 255, 0);
#pragma warning restore CA2211 // Non-constant fields should not be visible

    public Veldrid.GraphicsDevice GraphicDevice { get; }

    void DrawFrame();
    void RefreshScreen();
    void InvalidateScreen();
    void InvalidateRegion(Rectangle bounds);
    void LineDraw(int v2, int v3, int v4, int v5, Color v6, Image<Rgba32> image);
}
