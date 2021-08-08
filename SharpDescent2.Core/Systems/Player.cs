using System.Threading.Tasks;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.DataStructures;

namespace SharpDescent2.Core.Systems
{
    public class player_ship
    {
        public const int N_PLAYER_GUNS = 8;

        public int model_num;
        public int expl_vclip_num;
        public int mass;
        public int drag;
        public int max_thrust;
        public int reverse_thrust;
        public int brakes;     //low_thrust
        public int wiggle;
        public int max_rotthrust;
        public vms_vector[] gun_points = new vms_vector[N_PLAYER_GUNS];
    }

    public class Player : IGamePlatformManager
    {
        public static player_ship ship { get; private set; }

        public bool IsInitialized { get; }
        // Who am I data
        string callsign;    // The callsign of this player, for net purposes.
        byte[] net_address = new byte[6];               // The network address of the player.
        byte connected;                         //	Is the player connected or not?
        int objnum;                         // What object number this player is. (made an int by mk because it's very often referenced)
        int n_packets_got;                  // How many packets we got from them
        int n_packets_sent;             // How many packets we sent to them

        //	-- make sure you're 4 byte aligned now!

        // Game data
        uint flags;                         // Powerup flags, see below...
        int energy;                         // Amount of energy remaining.
        int shields;                            // shields remaining (protection) 
        byte lives;                            // Lives remaining, 0 = game over.
        byte level;                         // Current level player is playing. (must be signed for secret levels)
        byte laser_level;                  //	Current level of the laser.
        byte starting_level;                // What level the player started on.
        short killer_objnum;                    // Who killed me.... (-1 if no one)
        ushort primary_weapon_flags;                    //	bit set indicates the player has this weapon.
        ushort secondary_weapon_flags;                  //	bit set indicates the player has this weapon.
        ushort[] primary_ammo = new ushort[MAX.PRIMARY_WEAPONS];   // How much ammo of each type.
        ushort[] secondary_ammo = new ushort[MAX.SECONDARY_WEAPONS]; // How much ammo of each type.

        ushort pad; // Pad because increased weapon_flags from byte to short -YW 3/22/95

        //	-- make sure you're 4 byte aligned now

        // Statistics...
        int last_score;                 // Score at beginning of current level.
        int score;                          // Current score.
        int time_level;                     // Level time played
        int time_total;                     // Game time played (high word = seconds)

        int cloak_time;                     // Time cloaked
        int invulnerable_time;          // Time invulnerable

        short KillGoalCount;                                  // Num of players killed this level
        short net_killed_total;                               // Number of times killed total
        short net_kills_total;              // Number of net kills total
        short num_kills_level;              // Number of kills this level
        short num_kills_total;              // Number of kills total
        short num_robots_level;             // Number of initial robots this level
        short num_robots_total;             // Number of robots total
        ushort hostages_rescued_total;      // Total number of hostages rescued.
        ushort hostages_total;              // Total number of hostages.
        byte hostages_on_board;            //	Number of hostages on ship.
        byte hostages_level;               // Number of hostages on this level.
        int homing_object_dist;         //	Distance of nearest homing object.
        byte hours_level;                   // Hours played (since time_total can only go up to 9 hours)
        byte hours_total;					// Hours played (since time_total can only go up to 9 hours)

        public ValueTask<bool> Initialize()
        {
            ship = new player_ship
            {

            };

            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}
