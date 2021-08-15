using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;

namespace SharpDescent2.Core.Systems;

public enum AS
{
    REST = 0,
    ALERT = 1,
    FIRE = 2,
    RECOIL = 3,
    FLINCH = 4,
}

public class AISystem : IGamePlatformManager
{
    private readonly ILogger<AISystem> logger;
    private readonly ILibraryManager library;

    public AISystem(
        ILogger<AISystem> logger,
        ILibraryManager libraryManager)
    {
        this.logger = logger;
        this.library = libraryManager;
    }

    public const int Flinch_scale = 4;
    public const int Attack_scale = 24;
    public AS[] Mike_to_matt_xlate = { AS.REST, AS.REST, AS.ALERT, AS.ALERT, AS.FLINCH, AS.FIRE, AS.RECOIL, AS.REST };

    //	Amount of time since the current robot was last processed for things such as movement.
    //	It is not valid to use FrameTime because robots do not get moved every frame.

    public int Num_boss_teleport_segs;
    public short[] Boss_teleport_segs = new short[MAX.BOSS_TELEPORT_SEGS];
    public int Num_boss_gate_segs;
    public short[] Boss_gate_segs = new short[MAX.BOSS_TELEPORT_SEGS];

    public int N_robot_types { get; private set; }
    public int N_robot_joints { get; private set; }


    public bool IsInitialized { get; }

    public ValueTask<bool> Initialize()
    {
        var ham = (HAMArchive)this.library.GetLibrary("descent2.ham");
        this.N_robot_types = ham.RobotInfos.Count;
        this.N_robot_joints = ham.JointPositions.Length;

        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
