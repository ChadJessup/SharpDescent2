using System;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Veldrid;

namespace Sharp.Platform.NullManagers
{
    public class NullVideoManager : IVideoManager
    {
        public GraphicsDevice GraphicDevice { get; }
        public bool IsInitialized { get; }

        public void Dispose()
        {
        }

        public void DrawFrame()
        {
        }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void InvalidateRegion(SixLabors.ImageSharp.Rectangle bounds)
        {
        }

        public void InvalidateScreen()
        {
        }

        public void LineDraw(int v2, int v3, int v4, int v5, Color v6, Image<Rgba32> image)
        {
        }

        public void RefreshScreen()
        {
        }
    }
}
