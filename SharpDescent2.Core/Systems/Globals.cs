namespace SharpDescent2.Core.Systems;

public class Globals
{
    public GM Game_mode { get; set; } = GM.GAME_OVER;
}

[Flags]
public enum GM
{
    //	The following bits define the game modes.
    NORMAL = 0,//	You are in normal play mode, no multiplayer stuff
    EDITOR = 1,//	You came into the game from the editor
    SERIAL = 2,// You are in serial mode
    NETWORK = 4,// You are in network mode
    MULTI_ROBOTS = 8,//	You are in a multiplayer mode with robots.
    MULTI_COOP = 16,//	You are in a multiplayer mode and can't hurt other players.
    MODEM = 32,  // You are in a modem (serial) game
    UNKNOWN = 64,//	You are not in any mode, kind of dangerous...
    GAME_OVER = 128, //	Game has been finished
    TEAM = 256,// Team mode for network play
    CAPTURE = 512,// Capture the flag mode for D2
    HOARD = 1024,        // New hoard mode for D2 Christmas
    MULTI = 38,	//	You are in some type of multiplayer game
}
