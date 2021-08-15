using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.Loaders;
using Veldrid;

namespace SharpDescent2.Core.Systems;

public enum GaugeBitmaps
{
    //bitmap numbers for gauges

    GAUGE_SHIELDS = 0,  //0..9, in decreasing order (100%,90%...0%)

    GAUGE_INVULNERABLE = 10,		//10..19
    N_INVULNERABLE_FRAMES = 10,

    GAUGE_AFTERBURNER = 20,
    GAUGE_ENERGY_LEFT = 21,
    GAUGE_ENERGY_RIGHT = 22,
    GAUGE_NUMERICAL = 23,

    GAUGE_BLUE_KEY = 24,
    GAUGE_GOLD_KEY = 25,
    GAUGE_RED_KEY = 26,
    GAUGE_BLUE_KEY_OFF = 27,
    GAUGE_GOLD_KEY_OFF = 28,
    GAUGE_RED_KEY_OFF = 29,

    SB_GAUGE_BLUE_KEY = 30,
    SB_GAUGE_GOLD_KEY = 31,
    SB_GAUGE_RED_KEY = 32,
    SB_GAUGE_BLUE_KEY_OFF = 33,
    SB_GAUGE_GOLD_KEY_OFF = 34,
    SB_GAUGE_RED_KEY_OFF = 35,

    SB_GAUGE_ENERGY = 36,

    GAUGE_LIVES = 37,

    GAUGE_SHIPS = 38,
    GAUGE_SHIPS_LAST = 45,

    RETICLE_CROSS = 46,
    RETICLE_PRIMARY = 48,
    RETICLE_SECONDARY = 51,
    RETICLE_LAST = 55,

    GAUGE_HOMING_WARNING_ON = 56,
    GAUGE_HOMING_WARNING_OFF = 57,

    SML_RETICLE_CROSS = 58,
    SML_RETICLE_PRIMARY = 60,
    SML_RETICLE_SECONDARY = 63,
    SML_RETICLE_LAST = 67,

    KEY_ICON_BLUE = 68,
    KEY_ICON_YELLOW = 69,
    KEY_ICON_RED = 70,

    SB_GAUGE_AFTERBURNER = 71,

    FLAG_ICON_RED = 72,
    FLAG_ICON_BLUE = 73,
}

public enum WS
{
    SET = 0,        //in correct state
    FADING_OUT = 1,
    FADING_IN = 2,
}

public enum WBU
{
    WEAPON = 0,     //the weapons display
    MISSILE = 1,        //the missile view
    ESCORT = 2,     //the "buddy bot"
    REAR = 3,       //the rear view
    COOP = 4,       //coop or team member view
    GUIDED = 5,     //the guided missile
    MARKER = 6,     //a dropped marker
    STATIC = 7,		//playing static after missile hits
}

public class GaugeSystem : IGamePlatformManager
{
    private readonly ILogger<GaugeSystem> logger;
    private readonly Globals globals;
    private readonly ILibraryManager library;
    private readonly NewDemoSystem Newdemo;

    public GaugeSystem(
        ILogger<GaugeSystem> logger,
        ILibraryManager libraryManager,
        Globals globals,
        NewDemoSystem newDemo)
    {
        this.logger = logger;
        this.globals = globals;
        this.library = libraryManager;
        this.Newdemo = newDemo;
    }

    public bool IsInitialized { get; }

    public int[] old_score = { -1, -1 };
    public int[] old_energy = { -1, -1 };
    public int[] old_shields = { -1, -1 };
    public int[] old_flags = { -1, -1 };
    public int[,] old_weapon = { { -1, -1 }, { -1, -1 } };
    public int[,] old_ammo_count = { { -1, -1 }, { -1, -1 } };
    public int[] Old_Omega_charge = { -1, -1 };
    public int[] old_laser_level = { -1, -1 };
    public int[] old_cloak = { 0, 0 };
    public int[] old_lives = { -1, -1 };
    public int[] old_afterburner = { -1, -1 };
    public int[] old_bombcount = { 0, 0 };

    public int invulnerable_frame = 0;
    public WS cloak_fade_state;		//0=steady, -1 fading out, 1 fading in 
    public WBU[] weapon_box_user = { WBU.WEAPON, WBU.WEAPON };        //see WBU_ constants in gauges.h
    public int[] weapon_box_states;
    public int[] weapon_box_fade_values;

    public ValueTask<bool> Initialize()
    {
        var ham = (HAMArchive)this.library.GetLibrary("descent2.ham");

        var Game_mode = this.globals.Game_mode;

        for (int i = 0; i < 2; i++)
        {
            if (((Game_mode.HasFlag(GM.MULTI)) && !(Game_mode.HasFlag(GM.MULTI_COOP)))
                || ((Newdemo.state == ND_STATE.PLAYBACK) && (Newdemo.game_mode.HasFlag(GM.MULTI)) && !(Newdemo.game_mode.HasFlag(GM.MULTI_COOP))))
            {
                old_score[i] = -99;
            }
            else
            {
                old_score[i] = -1;
            }

            old_energy[i] = -1;
            old_shields[i] = -1;
            old_flags[i] = -1;
            old_cloak[i] = -1;
            old_lives[i] = -1;
            old_afterburner[i] = -1;
            old_bombcount[i] = 0;
            old_laser_level[i] = 0;

            old_weapon[0, i] = old_weapon[1, i] = -1;
            old_ammo_count[0, i] = old_ammo_count[1, i] = -1;
            Old_Omega_charge[i] = -1;
        }

        cloak_fade_state = 0;

        weapon_box_user[0] = WBU.WEAPON;
        weapon_box_user[1] = WBU.WEAPON;

        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}
