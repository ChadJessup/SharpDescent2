using System;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Managers
{
    public class TimerManager : ITimerManager
    {
        public TimeSpan BaseTimeSlice { get; set; } = TimeSpan.FromSeconds(10.0);
        public bool IsInitialized { get; private set; }

        public DateTime GameSystemTime { get; private set; }
        public DateTime GameStartTime { get; private set; }

        public ValueTask<bool> Initialize()
        {
            this.GameStartTime = DateTime.Now;
            this.GameSystemTime = DateTime.MinValue;

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
