using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullGameLogic : IGameLogic
    {
        public bool IsInitialized { get; }

        public void Dispose()
        {
        }

        public Task<int> GameLoop(CancellationToken token = default)
        {
            return Task.FromResult(0);
        }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }
    }
}
