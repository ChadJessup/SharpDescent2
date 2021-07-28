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

        private VideoManagerState guiVideoManagerState;   // VIDEO_ON, VIDEO_OFF, VIDEO_SUSPENDED, VIDEO_SHUTTING_DOWN

        private readonly GameContext context;

        private Sdl2Window window;
        public Sdl2Window Window { get => this.window; }
        public GraphicsDevice GraphicDevice { get; private set; }
        public ResourceFactory Factory { get; private set; }
        protected SpriteRenderer SpriteRenderer { get; private set; }

        private Swapchain mainSwapchain;
        private CommandList commandList;
        private DeviceBuffer _screenSizeBuffer;
        private DeviceBuffer _shiftBuffer;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        private Shader _computeShader;
        private ResourceLayout _computeLayout;
        private Pipeline _computePipeline;
        private ResourceSet _computeResourceSet;
        private Pipeline _graphicsPipeline;
        private ResourceSet _graphicsResourceSet;

        private Texture _computeTargetTexture;
        private TextureView _computeTargetTextureView;
        private ResourceLayout _graphicsLayout;
        private float _ticks;

        private bool _colorSrgb = true;

        private int gusScreenWidth = 640;
        private int gusScreenHeight = 480;
        private int gubScreenPixelDepth;

        private RgbaFloat clearColor = new(1.0f, 0, 0.2f, 1f);
        private Rectangle rcWindow;

        //
        // Direct Draw objects for both the Primary and Backbuffer surfaces
        //

        // private LPDIRectangleDRAW? _gpDirectDrawObject = null;
        // private LPDIRectangleDRAW2 gpDirectDrawObject = null;

        //private Surface? _gpPrimarySurface = null;
        //private Surface2? gpPrimarySurface = null;
        //private Surface2? gpBackBuffer = null

        public bool IsInitialized { get; private set; }

        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;
        private const int PIXEL_DEPTH = 16;

        static bool fShowMouse;
        static Rectangle Region;
        static Point MousePos;
        static bool fFirstTime = true;
        private bool windowResized;

        private Texture backBuffer;
        private Image<Rgba32> gpFrameBuffer;
        private Image<Rgba32> gpPrimarySurface;

        public VeldridVideoManager(
            ILogger<VeldridVideoManager> logger,
            GameContext context)
        {
            this.logger = logger;
            this.context = context;

            this.gpPrimarySurface = new(SCREEN_WIDTH, SCREEN_HEIGHT);
            this.gpFrameBuffer = new(SCREEN_WIDTH, SCREEN_HEIGHT);

            Configuration.Default.MemoryAllocator = new SixLabors.ImageSharp.Memory.SimpleGcMemoryAllocator();
        }

        public ValueTask<bool> Initialize()
        {
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
                debug: false,
                swapchainDepthFormat: null,
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: false,
                swapchainSrgbFormat: this._colorSrgb);

#if DEBUG
            gdOptions.Debug = true;
#endif
            static SDL_WindowFlags GetWindowFlags(WindowState state)
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

            SDL_WindowFlags flags = SDL_WindowFlags.OpenGL
                | SDL_WindowFlags.Resizable
                | GetWindowFlags(windowCI.WindowInitialState);

            if (windowCI.WindowInitialState != WindowState.Hidden)
            {
                flags |= SDL_WindowFlags.Shown;
            }

            this.window = new Sdl2Window(
                windowCI.WindowTitle,
                windowCI.X,
                windowCI.Y,
                windowCI.WindowWidth,
                windowCI.WindowHeight,
                flags,
                threadedProcessing: true);

            this.GraphicDevice = VeldridStartup.CreateGraphicsDevice(
                this.window,
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
    }

    public enum VideoManagerState
    {
        Off = 0x00,
        On = 0x01,
        ShuttingDown = 0x02,
        Suspended = 0x04,
    }
}
