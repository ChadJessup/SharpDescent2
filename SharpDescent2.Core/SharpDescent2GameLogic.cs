using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core
{
    public class SharpDescent2GameLogic : IGameLogic
    {
        public bool IsInitialized { get; }

        public Task<int> GameLoop(CancellationToken token = default)
        {
            return Task.FromResult(0);
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
