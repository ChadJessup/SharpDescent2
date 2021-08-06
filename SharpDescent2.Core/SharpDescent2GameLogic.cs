﻿using System;
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
        private readonly TextSystem text;

        public bool IsInitialized { get; }

        public SharpDescent2GameLogic(
            ILogger<SharpDescent2GameLogic> logger,
            IConfiguration configuration,
            ILibraryManager libraryManager,
            TextSystem textSystem)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.library = libraryManager;
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

        public ValueTask<bool> Initialize()
        {

            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}
