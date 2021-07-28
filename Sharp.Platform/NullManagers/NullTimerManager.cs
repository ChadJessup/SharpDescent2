using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.NullManagers
{
    public class NullTimerManager : ITimerManager
    {
        public TimeSpan BaseTimeSlice { get; set; }
        public bool IsInitialized { get; }

        public void Dispose()
        {
        }

        public DateTime GetClock()
        {
            return DateTime.MinValue;
        }

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void PauseTime(bool shouldPause)
        {
        }
    }
}
