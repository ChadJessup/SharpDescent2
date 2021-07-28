using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp.Platform.Interfaces
{
    public interface ITimerManager : IGamePlatformManager
    {
        // Going to try TimeSpan and DateTime's for now...might be too much trouble. If so, move to longs.
        TimeSpan BaseTimeSlice { get; set; }
        void PauseTime(bool shouldPause);
        DateTime GetClock();
    }
}
