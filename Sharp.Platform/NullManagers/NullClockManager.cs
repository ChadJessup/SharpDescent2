using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullClockManager : IClockManager
    {
        public bool GamePaused { get; set; }
        public bool IsInitialized { get; }

        public void Dispose()
        {
        }

        public long GetClock()
        {
            return long.MinValue;
        }

        public long GetTickCount()
        {
            return long.MinValue;
        }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void PauseGame()
        {
        }

        public void UnPauseGame()
        {
        }

        public void UpdateClock()
        {
        }
    }
}
