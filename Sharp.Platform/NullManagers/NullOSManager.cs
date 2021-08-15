using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullOSManager : IOSManager
    {
        public bool IsInitialized { get; }

        public void Dispose()
        {
        }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public ValueTask<bool> Pump(Action gameLoopCallback)
        {
            return ValueTask.FromResult(true);
        }
    }
}
