using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDescent2.Core.DataStructures;
using SharpDescent2.Core.Loaders;
using SixLabors.ImageSharp.PixelFormats;

namespace SharpDescent2.Core.Loaders
{
    public class HAMArchive
    {
        public const string HAMFILE_ID = "HAM!";
        public const int HAMFILE_VERSION = 3;

        public string FilePath { get; init; }


        public unsafe static HAMArchive LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var file = File.OpenRead(path);

            // verify header and version
            Span<byte> hamHeader = stackalloc byte[4];
            Span<byte> hamVersionBytes = stackalloc byte[sizeof(int)];
            file.Read(hamHeader);

            if (hamHeader[0] != HAMFILE_ID[0] && hamHeader[1] != HAMFILE_ID[1] && hamHeader[2] != HAMFILE_ID[2] && hamHeader[3] != HAMFILE_ID[3])
            {
                throw new InvalidOperationException($"HAM file doesn't start with {HAMFILE_ID}: {path}");
            }

            file.Read(hamVersionBytes);

            if (hamVersionBytes[0] != HAMFILE_VERSION)
            {
                throw new InvalidOperationException($"HAM Version isn't {HAMFILE_VERSION}");
            }

            Span<byte> numberCountBytes = stackalloc byte[sizeof(int)];
            file.Read(numberCountBytes);

            int numberOfTextures = MemoryMarshal.Read<int>(numberCountBytes);
            var size = Unsafe.SizeOf<BitmapIndex>() * numberOfTextures;
            using var bitmapIndexBuffer = MemoryPool<byte>.Shared.Rent(minBufferSize: size);
            file.Read(bitmapIndexBuffer.Memory.Span.Slice(0, size));

            var bitmapIndexes = MemoryMarshal.Cast<byte, BitmapIndex>(bitmapIndexBuffer.Memory.Span)
                .ToArray();

            size = Unsafe.SizeOf<TextureMapInfo>() * numberOfTextures;
            using var tmiBuffer = MemoryPool<byte>.Shared.Rent(minBufferSize: size);
            file.Read(tmiBuffer.Memory.Span.Slice(0, size));

            var tmi = MemoryMarshal.Cast<byte, TextureMapInfo>(tmiBuffer.Memory.Span)
                .ToArray();

            file.Read(numberCountBytes);
            int numberOfSounds = MemoryMarshal.Read<int>(numberCountBytes);

            var sounds = new byte[numberOfSounds];
            var altSounds = new byte[numberOfSounds];

            file.Read(sounds);
            file.Read(altSounds);

            file.Read(numberCountBytes);
            int numberOfVClips = MemoryMarshal.Read<int>(numberCountBytes);
            var VCLIPS = cfread<vclip>(Unsafe.SizeOf<vclip>(), numberOfVClips, file);

            file.Read(numberCountBytes);
            int numberOfEffects = MemoryMarshal.Read<int>(numberCountBytes);
            var effects = cfread<eclip>(Unsafe.SizeOf<eclip>(), numberOfEffects, file);

            // 42976
            file.Read(numberCountBytes);
            int numberOfWallAnimations = MemoryMarshal.Read<int>(numberCountBytes);
            var wallAnimations = cfread<wclip>(Unsafe.SizeOf<wclip>(), numberOfWallAnimations, file);

            // 49406
            file.Read(numberCountBytes);
            int numberOfRobotInfos = MemoryMarshal.Read<int>(numberCountBytes);
            var robotInfos = ParseRobotInfos(numberOfRobotInfos, file);

            return new HAMArchive
            {
                FilePath = path,
            };
        }

        private static List<robot_info> ParseRobotInfos(int count, Stream stream)
        {
            // 49410
            var robotInfos = new List<robot_info>();
            using var br = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

            for (int i = 0; i < count; i++)
            {
                robot_info ri = new();

                ri.model_num = br.ReadInt32();
                for (int j = 0; j < Robots.MAX_GUNS; j++)
                {
                    Vector3 vector = new();
                    vector.X = br.ReadInt32();
                    vector.Y = br.ReadInt32();
                    vector.Z = br.ReadInt32();
                    ri.gun_points[j] = vector;
                }

                // 49510
                stream.Read(ri.gun_submodels);

                ri.exp1_vclip_num = br.ReadInt16();
                ri.exp1_sound_num = br.ReadInt16();

                ri.exp2_vclip_num = br.ReadInt16();
                ri.exp2_sound_num = br.ReadInt16();

                ri.weapon_type = br.ReadByte();
                ri.weapon_type2 = br.ReadByte();
                ri.n_guns = br.ReadByte();
                ri.contains_id = br.ReadByte();

                ri.contains_count = br.ReadByte();
                ri.contains_prob = br.ReadByte();
                ri.contains_type = br.ReadByte();
                ri.kamikaze = br.ReadByte();

                ri.score_value = br.ReadInt16();
                ri.badass = br.ReadByte();
                ri.energy_drain = br.ReadByte();

                ri.lighting = br.ReadInt32();
                ri.strength = br.ReadInt32();

                ri.mass = br.ReadInt32();
                ri.drag = br.ReadInt32();

                ri.field_of_view = cfread<int>(sizeof(int), Game.NDL, stream);
                ri.firing_wait = cfread<int>(sizeof(int), Game.NDL, stream);
                ri.firing_wait2 = cfread<int>(sizeof(int), Game.NDL, stream);
                ri.turn_time = cfread<int>(sizeof(int), Game.NDL, stream);

                ri.max_speed = cfread<int>(sizeof(int), Game.NDL, stream);
                ri.circle_distance = cfread<int>(sizeof(int), Game.NDL, stream);

                stream.Read(ri.rapidfire_count);
                stream.Read(ri.evade_speed);
                ri.cloak_type = br.ReadByte();
                ri.attack_type = br.ReadByte();

                ri.see_sound = br.ReadByte();
                ri.attack_sound = br.ReadByte();
                ri.claw_sound = br.ReadByte();
                ri.taunt_sound = br.ReadByte();

                ri.boss_flag = br.ReadByte();
                ri.companion = br.ReadByte();
                ri.smart_blobs = br.ReadByte();
                ri.energy_blobs = br.ReadByte();

                ri.thief = br.ReadByte();
                ri.pursuit = br.ReadByte();
                ri.lightcast = br.ReadByte();
                ri.death_roll = br.ReadByte();

                ri.flags = br.ReadByte();
                stream.Read(ri.pad);

                ri.deathroll_sound = br.ReadByte();
                ri.glow = br.ReadByte();
                ri.behavior = br.ReadByte();
                ri.aim = br.ReadByte();

                // 49706 - before jointlist array

                // 180 - next should always be 0xabcd
                stream.Seek(180, SeekOrigin.Current);

                ri.always_0xabcd = br.ReadInt32();

                // 49886
                if (ri.always_0xabcd != 0x0000abcd)
                {

                }

                robotInfos.Add(ri);
            }

            return robotInfos;
        }

        private static T[] cfread<T>(int size, int number, Stream stream)
            where T : struct
        {
            var totalSize = size * number;
            var buffer = MemoryPool<byte>.Shared.Rent(minBufferSize: totalSize);
            stream.Read(buffer.Memory.Span.Slice(0, totalSize));

            return MemoryMarshal.Cast<byte, T>(buffer.Memory.Span.Slice(0, totalSize))
                .ToArray();
        }
    }

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

    //describes a list of joint positions
    public readonly struct jointlist
    {
        public readonly short n_joints;
        public readonly short offset;
    }

}
