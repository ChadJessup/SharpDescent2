using System.Numerics;

namespace SharpDescent2.Core.DataStructures
{
    //	Robot information - size 478 on disk
    public class robot_info
    {
        public int model_num;                          // which polygon model?
        public Vector3[] gun_points = new Vector3[Robots.MAX_GUNS];            // where each gun model is
        public byte[] gun_submodels = new byte[Robots.MAX_GUNS];      // which submodel is each gun in?
        public short exp1_vclip_num;
        public short exp1_sound_num;
        public short exp2_vclip_num;
        public short exp2_sound_num;
        public byte weapon_type;
        public byte weapon_type2;                      //	Secondary weapon number, -1 means none, otherwise gun #0 fires this weapon.
        public byte n_guns;                                // how many different gun positions
        public byte contains_id;                       //	ID of powerup this robot can contain.
        public byte contains_count;                    //	Max number of things this instance can contain.
        public byte contains_prob;                     //	Probability that this instance will contain something in N/16
        public byte contains_type;                     //	Type of thing contained, robot or powerup, in bitmaps.tbl, !0=robot, 0=powerup
        public byte kamikaze;                          //	!0 means commits suicide when hits you, strength thereof. 0 means no.
        public short score_value;                      //	Score from this robot.
        public byte badass;                                //	Dies with badass explosion, and strength thereof, 0 means NO.
        public byte energy_drain;                      //	Points of energy drained at each collision.
        public int lighting;                           // should this be here or with polygon model?
        public int strength;                           // Initial shields of robot
        public int mass;                                       // how heavy is this thing?
        public int drag;                                       // how much drag does it have?

        public int[] field_of_view = new int[Game.NDL];                 // compare this value with forward_vector.dot.vector_to_player, if field_of_view <, then robot can see player
        public int[] firing_wait = new int[Game.NDL];                       //	time in seconds between shots
        public int[] firing_wait2 = new int[Game.NDL];                  //	time in seconds between shots
        public int[] turn_time = new int[Game.NDL];                     // time in seconds to rotate 360 degrees in a dimension
                                                                        // -- unused, mk, 05/25/95	fix		fire_power[NDL];						//	damage done by a hit from this robot
                                                                        // -- unused, mk, 05/25/95	fix		shield[NDL];							//	shield strength of this robot
        public int[] max_speed = new int[Game.NDL];                     //	maximum speed attainable by this robot
        public int[] circle_distance = new int[Game.NDL];               //	distance at which robot circles player

        public byte[] rapidfire_count = new byte[Game.NDL];              //	number of shots fired rapidly
        public byte[] evade_speed = new byte[Game.NDL];                      //	rate at which robot can evade shots, 0=none, 4=very fast
        public byte cloak_type;                                //	0=never, 1=always, 2=except-when-firing
        public byte attack_type;                           //	0=firing, 1=charge (like green guy)

        public byte see_sound;                                //	sound robot makes when it first sees the player
        public byte attack_sound;                         //	sound robot makes when it attacks the player
        public byte claw_sound;                               //	sound robot makes as it claws you (attack_type should be 1)
        public byte taunt_sound;                          //	sound robot makes after you die

        public byte boss_flag;                             //	0 = not boss, 1 = boss.  Is that surprising?
        public byte companion;                             //	Companion robot, leads you to things.
        public byte smart_blobs;                           //	how many smart blobs are emitted when this guy dies!
        public byte energy_blobs;                          //	how many smart blobs are emitted when this guy gets hit by energy weapon!

        public byte thief;                                 //	!0 means this guy can steal when he collides with you!
        public byte pursuit;                                   //	!0 means pursues player after he goes around a corner.  4 = 4/2 pursue up to 4/2 seconds after becoming invisible if up to 4 segments away
        public byte lightcast;                             //	Amount of light cast. 1 is default.  10 is very large.
        public byte death_roll;                                //	0 = dies without death roll. !0 means does death roll, larger = faster and louder

        //boss_flag, companion, thief, & pursuit probably should also be bits in the flags byte.
        public byte flags;                                    // misc properties
        public byte[] pad = new byte[3];                                   // alignment

        public byte deathroll_sound;                      // if has deathroll, what sound?
        public byte glow;                                     // apply this light to robot itself. stored as 4:4 fixed-point
        public byte behavior;                             //	Default behavior.
        public byte aim;                                      //	255 = perfect, less = more likely to miss.  0 != random, would look stupid.  0=45 degree spread.  Specify in bitmaps.tbl in range 0.0..1.0

        //animation info
        public jointlist[,] anim_states = new jointlist[Robots.MAX_GUNS + 1, Robots.N_ANIM_STATES];

        public int always_0xabcd;                          // debugging
    }

}
