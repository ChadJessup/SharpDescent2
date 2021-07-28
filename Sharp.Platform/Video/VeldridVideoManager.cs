using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sharp.Platform;
using Sharp.Platform.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using Point = SixLabors.ImageSharp.Point;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace Sharp.Platform.Video
{
    public class VeldridVideoManager : IVideoManager
    {
        private readonly ILogger<VeldridVideoManager> logger;
        private readonly VideoManagerState guiVideoManagerState;   // VIDEO_ON, VIDEO_OFF, VIDEO_SUSPENDED, VIDEO_SHUTTING_DOWN
        private Swapchain mainSwapchain;
        private CommandList commandList;
        private readonly DeviceBuffer _screenSizeBuffer;
        private readonly DeviceBuffer _shiftBuffer;
        private readonly DeviceBuffer _vertexBuffer;
        private readonly DeviceBuffer _indexBuffer;
        private readonly Shader _computeShader;
        private readonly ResourceLayout _computeLayout;
        private readonly Pipeline _computePipeline;
        private readonly ResourceSet _computeResourceSet;
        private readonly Pipeline _graphicsPipeline;
        private readonly ResourceSet _graphicsResourceSet;

        private readonly Texture _computeTargetTexture;
        private readonly TextureView _computeTargetTextureView;
        private readonly ResourceLayout _graphicsLayout;
        private readonly float _ticks;

        private readonly bool _colorSrgb = true;

        private readonly int gusScreenWidth = 640;
        private readonly int gusScreenHeight = 480;
        private readonly int gubScreenPixelDepth;

        private readonly RgbaFloat clearColor = new(1.0f, 0, 0.2f, 1f);
        private Rectangle rcWindow;

        public bool IsInitialized { get; private set; }

        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;
        private const int PIXEL_DEPTH = 16;
        private static readonly bool fShowMouse;
        private static Rectangle Region;
        private static Point MousePos;
        private static readonly bool fFirstTime = true;
        private bool windowResized;

        private Texture backBuffer;
        private readonly Image<Rgba32> gpFrameBuffer;
        private readonly Image<Rgba32> gpPrimarySurface;

        public VeldridVideoManager(
            ILogger<VeldridVideoManager> logger,
            GameContext context)
        {
            this.logger = logger;

            this.gpPrimarySurface = new(SCREEN_WIDTH, SCREEN_HEIGHT);
            this.gpFrameBuffer = new(SCREEN_WIDTH, SCREEN_HEIGHT);

            Configuration.Default.MemoryAllocator = new SixLabors.ImageSharp.Memory.SimpleGcMemoryAllocator();
        }

        public Sdl2Window Window { get; private set; }
        public GraphicsDevice GraphicDevice { get; private set; }
        public ResourceFactory Factory { get; private set; }
        protected SpriteRenderer SpriteRenderer { get; private set; }

        public ValueTask<bool> Initialize()
        {
            // TODO: pass these in by configuration
            WindowCreateInfo windowCI = new()
            {
                X = 50,
                Y = 50,
                WindowWidth = 640,
                WindowHeight = 480,
                WindowInitialState = WindowState.Normal,
                WindowTitle = "SharpDescent2",
            };

            GraphicsDeviceOptions gdOptions = new(
                debug: true,
                swapchainDepthFormat: null,
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: false,
                swapchainSrgbFormat: this._colorSrgb);

            SDL_WindowFlags flags = SDL_WindowFlags.OpenGL
                | SDL_WindowFlags.Resizable
                | this.GetWindowFlags(windowCI.WindowInitialState);

            if (windowCI.WindowInitialState != WindowState.Hidden)
            {
                flags |= SDL_WindowFlags.Shown;
            }

            this.Window = new Sdl2Window(
                windowCI.WindowTitle,
                windowCI.X,
                windowCI.Y,
                windowCI.WindowWidth,
                windowCI.WindowHeight,
                flags,
                threadedProcessing: true);

            this.GraphicDevice = VeldridStartup.CreateGraphicsDevice(
                this.Window,
                gdOptions);

            this.Window.Resized += () => this.windowResized = true;

            //this.Window.PollIntervalInMs = 1000 / 30;
            this.Factory = new DisposeCollectorResourceFactory(this.GraphicDevice.ResourceFactory);
            this.mainSwapchain = this.GraphicDevice.MainSwapchain;
            this.commandList = this.GraphicDevice.ResourceFactory.CreateCommandList();

            this.SpriteRenderer = new SpriteRenderer(this.GraphicDevice);

            this.backBuffer = new ImageSharpTexture(new Image<Rgba32>(SCREEN_WIDTH, SCREEN_HEIGHT), mipmap: false)
                .CreateDeviceTexture(this.GraphicDevice, this.GraphicDevice.ResourceFactory);

            return ValueTask.FromResult(this.IsInitialized);
        }

        public void DrawFrame()
        {
            this.commandList.Begin();

            this.commandList.SetFramebuffer(this.mainSwapchain.Framebuffer);

            if (this.clearScreen)
            {
                this.commandList.ClearColorTarget(0, this.clearColor);
                this.clearScreen = false;
            }

            this.commandList.ClearColorTarget(0, this.clearColor);

            // Everything above writes to this SpriteRenderer, so draw it now.
            this.SpriteRenderer.Draw(this.GraphicDevice, this.commandList);

            //this.SpriteRenderer.RenderText(this.GraphicDevice, this.commandList, this.fonts.TextRenderer.TextureView, new(0, 0));
            this.commandList.End();

            //this.fonts.TextRenderer.RenderAllText();
            this.GraphicDevice.SubmitCommands(this.commandList);
            this.GraphicDevice.SwapBuffers(this.mainSwapchain);
        }

        public static byte[] ReadEmbeddedAssetBytes(string name)
        {
            var names = typeof(VeldridVideoManager).Assembly.GetManifestResourceNames();

            var foundName = names
                .FirstOrDefault(n => n.Contains(name, StringComparison.InvariantCultureIgnoreCase));

            // TODO: Real exceptions that make sense.
            if (foundName is null)
            {
                throw new FileNotFoundException(name);
            }

            using Stream stream = OpenEmbeddedAssetStream(foundName);

            byte[] bytes = new byte[stream.Length];
            using MemoryStream ms = new(bytes);

            stream.CopyTo(ms);
            return bytes;
        }

        public static Stream OpenEmbeddedAssetStream(string name)
            => typeof(VeldridVideoManager).Assembly.GetManifestResourceStream(name)!;

        public void RefreshScreen()
        {

        }

        private void DrawRegion(
            Texture destinationTexture,
            int destinationPointX,
            int destinationPointY,
            Rectangle sourceRegion,
            Image<Rgba32> sourceTexture)
            => this.BlitRegion(
                destinationTexture,
                new Point(destinationPointX, destinationPointY),
                sourceRegion,
                sourceTexture);

        private bool clearScreen;

        private void BlitRegion(
            Texture texture,
            Point destinationPoint,
            Rectangle sourceRegion,
            Image<Rgba32> srcImage)
        {
            srcImage.Mutate(ctx => ctx.Crop(sourceRegion));

            var newTexture = new ImageSharpTexture(srcImage)
                .CreateDeviceTexture(this.GraphicDevice, this.GraphicDevice.ResourceFactory);

            var finalRect = new Rectangle(
                new Point(destinationPoint.X, destinationPoint.Y),
                new Size(sourceRegion.Width, sourceRegion.Height));

            this.SpriteRenderer.AddSprite(finalRect, newTexture, srcImage.GetHashCode().ToString());
        }

        public void LineDraw(int v2, int v3, int v4, int v5, Color color, Image<Rgba32> image)
        {
        }

        public void ClearElements()
        {
        }

        public void InvalidateScreen()
        {
        }

        public void InvalidateRegion(Rectangle bounds)
        {
        }

        public void Dispose()
        {
            this.GraphicDevice.WaitForIdle();
            (this.Factory as DisposeCollectorResourceFactory)!.DisposeCollector.DisposeAll();
            this.GraphicDevice.Dispose();

            GC.SuppressFinalize(this);
        }

        private SDL_WindowFlags GetWindowFlags(WindowState state)
            => state switch
            {
                WindowState.Normal => 0,
                WindowState.FullScreen => SDL_WindowFlags.Fullscreen,
                WindowState.Maximized => SDL_WindowFlags.Maximized,
                WindowState.Minimized => SDL_WindowFlags.Minimized,
                WindowState.BorderlessFullScreen => SDL_WindowFlags.FullScreenDesktop,
                WindowState.Hidden => SDL_WindowFlags.Hidden,
                _ => throw new VeldridException("Invalid WindowState: " + state),
            };
    }

    public enum VideoManagerState
    {
        Off = 0x00,
        On = 0x01,
        ShuttingDown = 0x02,
        Suspended = 0x04,
    }
}
