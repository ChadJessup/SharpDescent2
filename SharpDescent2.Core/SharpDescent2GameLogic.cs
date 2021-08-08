using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Systems;

namespace SharpDescent2.Core
{
    public class SharpDescent2GameLogic : IGameLogic
    {
        private readonly ILogger<SharpDescent2GameLogic> logger;
        private readonly IConfiguration configuration;
        private readonly ILibraryManager library;
        private readonly ObjectSystem objects;
        private readonly TextSystem text;

        public bool IsInitialized { get; private set; }

        public SharpDescent2GameLogic(
            ILogger<SharpDescent2GameLogic> logger,
            IConfiguration configuration,
            ILibraryManager libraryManager,
            TextSystem textSystem,
            ObjectSystem objectSystem)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.library = libraryManager;
            this.objects = objectSystem;
            this.text = textSystem;
        }

        public async Task<int> GameLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100.0), CancellationToken.None);
            }

            return 0;
        }

        public async ValueTask<bool> Initialize()
        {
            this.IsInitialized = await this.objects.Initialize();

            return this.IsInitialized;
        }

        public void Dispose()
        {
        }
    }
}
