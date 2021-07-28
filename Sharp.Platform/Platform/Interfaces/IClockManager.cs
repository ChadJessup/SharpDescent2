using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp.Platform.Interfaces
{
    public interface IClockManager : IGamePlatformManager
    {
        bool GamePaused { get; set; }

        long GetClock();
        long GetTickCount();
        void UpdateClock();
        void UnPauseGame();
        void PauseGame();
    }
}
