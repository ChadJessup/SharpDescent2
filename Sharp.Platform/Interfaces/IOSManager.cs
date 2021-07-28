using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;

namespace Sharp.Platform.Interfaces
{
    public interface IOSManager : IGamePlatformManager
    {
        ValueTask<bool> Pump(Action gameLoopCallback);
    }
}
