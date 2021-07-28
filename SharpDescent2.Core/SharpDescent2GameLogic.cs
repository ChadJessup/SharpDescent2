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
