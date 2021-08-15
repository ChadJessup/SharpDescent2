using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using Sharp.Platform.NullManagers;

namespace Sharp.Platform
{
    public class GameContext : IDisposable
    {
        private readonly ILogger<GameContext> logger;

        private bool disposedValue;

        public GameContext(
            ILogger<GameContext> logger,
            IServiceProvider services,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.Services = services;
            this.Configuration = configuration;

            this.logger.LogDebug($"Initialized {nameof(GameContext)}");
        }

        public GameState State { get; set; } = GameState.Unknown;
        public IServiceProvider Services { get; }
        public IConfiguration Configuration { get; }
        public IGameLogic GameLogic { get; set; } = new NullGameLogic();
        public IOSManager OSManager { get; set; } = new NullOSManager();
        public IFileManager FileManager { get; set; } = new NullFileManager();

        public IMusicManager MusicManager { get; set; } = new NullMusicManager();
        public ISoundManager SoundManager { get; set; } = new NullSoundManager();
        public ITimerManager TimerManager { get; set; } = new NullTimerManager();
        public IClockManager ClockManager { get; set; } = new NullClockManager();
        public IVideoManager VideoManager { get; set; } = new NullVideoManager();

        public bool ApplicationActive { get; private set; }

        public async Task<int> StartGameLoop(CancellationToken token)
        {
            if (this.GameLogic is null)
            {
                throw new NullReferenceException("GameLogic must be provided!");
            }

            this.State = GameState.Running;

            var result = await Task.Run(() => this.GameLogic?.GameLoop(token));

            this.State = GameState.Exiting;

            return result;
        }

        public async Task<bool> Initialize()
        {
            this.State = GameState.Initializing;
            var success = true;

            success &= await this.GameLogic.Initialize();
            success &= await this.FileManager.Initialize();
            success &= await this.SoundManager.Initialize();
            success &= await this.TimerManager.Initialize();
            success &= await this.ClockManager.Initialize();

            if (success)
            {
                this.ApplicationActive = true;
                this.State = GameState.Running;

                this.logger.LogDebug("Running Game");
            }

            return success;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.State = GameState.Disposing;

                if (disposing)
                {
                    this.GameLogic?.Dispose();
                    this.FileManager.Dispose();
                    this.SoundManager.Dispose();
                    this.TimerManager.Dispose();
                    this.ClockManager.Dispose();
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
