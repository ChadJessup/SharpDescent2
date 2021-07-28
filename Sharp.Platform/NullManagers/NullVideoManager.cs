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
            throw new NotImplementedException();
        }

        public void DrawFrame()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> Initialize()
        {
            throw new NotImplementedException();
        }

        public void InvalidateRegion(SixLabors.ImageSharp.Rectangle bounds)
        {
            throw new NotImplementedException();
        }

        public void InvalidateScreen()
        {
            throw new NotImplementedException();
        }

        public void LineDraw(int v2, int v3, int v4, int v5, Color v6, Image<Rgba32> image)
        {
            throw new NotImplementedException();
        }

        public void RefreshScreen()
        {
            throw new NotImplementedException();
        }
    }
}
