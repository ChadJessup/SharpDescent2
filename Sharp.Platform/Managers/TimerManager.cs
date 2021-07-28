using System;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform
{
    public class TimerManager : ITimerManager
    {
        public TimeSpan BaseTimeSlice { get; set; } = TimeSpan.FromSeconds(10.0);
        public bool IsInitialized { get; private set; }

        public ValueTask<bool> Initialize()
        {
            this.IsInitialized = true;
            return ValueTask.FromResult(true);
        }


        public void PauseTime(bool shouldPause)
        {
        }

        public DateTime GetClock()
        {
            return DateTime.Now;
        }

        public void Dispose()
        {
        }
    }
}
