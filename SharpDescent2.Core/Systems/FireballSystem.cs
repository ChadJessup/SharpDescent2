using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.DataStructures;

namespace SharpDescent2.Core.Systems;

public class FireballSystem : IGamePlatformManager
{
    private readonly ILogger<FireballSystem> logger;

    public FireballSystem(
        ILogger<FireballSystem> logger)
    {
        this.logger = logger;
    }

    public bool IsInitialized { get; }

    public expl_wall[] expl_wall_list = new expl_wall[MAX.EXPLODING_WALLS];

    public ValueTask<bool> Initialize()
    {
        for (int i = 0; i < MAX.EXPLODING_WALLS; i++)
        {
            expl_wall_list[i] = new expl_wall
            {
                segnum = -1,
            };
        }

        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
