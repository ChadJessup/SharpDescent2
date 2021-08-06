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

            file.Read(numberCountBytes);
            int numberOfJoints = MemoryMarshal.Read<int>(numberCountBytes);
            var jointPositions = cfread<jointpos>(Unsafe.SizeOf<jointpos>(), numberOfJoints, file);

            file.Read(numberCountBytes);
            int numberOfWeaponInfos = MemoryMarshal.Read<int>(numberCountBytes);
            var weaponInfos = ParseWeaponInfos(numberOfWeaponInfos, file);

            file.Read(numberCountBytes);
            int numberOfPowerupTypes = MemoryMarshal.Read<int>(numberCountBytes);
            var powerupTypes = cfread<powerup_type_info>(Unsafe.SizeOf<powerup_type_info>(), numberOfPowerupTypes, file);

            file.Read(numberCountBytes);
            int numberOfPolygonModels = MemoryMarshal.Read<int>(numberCountBytes);

            return new HAMArchive
            {
                FilePath = path,
            };
        }

        private static List<weapon_info> ParseWeaponInfos(int numberOfWeaponInfos, Stream stream)
        {
            var weaponInfos = new List<weapon_info>();
            using var br = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

            for (int i = 0; i < numberOfWeaponInfos; i++)
            {
                var wi = new weapon_info
                {
                    render_type = br.ReadByte(),
                    persistent = br.ReadByte(),
                    model_num = br.ReadInt16(),
                    model_num_inner = br.ReadInt16(),

                    flash_vclip = br.ReadByte(),
                    robot_hit_vclip = br.ReadByte(),
                    flash_sound = br.ReadByte(),

                    wall_hit_vclip = br.ReadByte(),
                    fire_count = br.ReadByte(),
                    robot_hit_sound = br.ReadInt16(),

                    ammo_usage = br.ReadByte(),
                    weapon_vclip = br.ReadByte(),
                    wall_hit_sound = br.ReadInt16(),

                    destroyable = br.ReadByte(),
                    matter = br.ReadByte(),
                    bounce = br.ReadByte(),
                    homing_flag = br.ReadByte(),

                    speedvar = br.ReadByte(),
                    flags = br.ReadByte(),
                    flash = br.ReadByte(),
                    afterburner_size = br.ReadByte(),

                    children = br.ReadByte(),

                    energy_usage = br.ReadInt32(),
                    fire_wait = br.ReadInt32(),

                    multi_damage_scale = br.ReadInt32(),

                    bitmap = new BitmapIndex
                    {
                        Index = br.ReadUInt16(),
                    },

                    blob_size = br.ReadInt32(),
                    flash_size = br.ReadInt32(),
                    impact_size = br.ReadInt32(),

                    strength = Enumerable.Range(0, Game.NDL).Select(_ => br.ReadInt32()).ToArray(),
                    speed = Enumerable.Range(0, Game.NDL).Select(_ => br.ReadInt32()).ToArray(),
                    mass = br.ReadInt32(),
                    drag = br.ReadInt32(),
                    thrust = br.ReadInt32(),
                    po_len_to_width_ratio = br.ReadInt32(),
                    light = br.ReadInt32(),
                    lifetime = br.ReadInt32(),
                    damage_radius = br.ReadInt32(),
                    picture = new BitmapIndex
                    {
                        Index = br.ReadUInt16(),
                    },
                    hires_picture = new BitmapIndex
                    {
                        Index = br.ReadUInt16(),
                    },
                };

                weaponInfos.Add(wi);
            }

            return weaponInfos;
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

                // TODO: parse this.

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
            using var buffer = MemoryPool<byte>.Shared.Rent(minBufferSize: totalSize);
            stream.Read(buffer.Memory.Span.Slice(0, totalSize));

            return MemoryMarshal.Cast<byte, T>(buffer.Memory.Span.Slice(0, totalSize))
                .ToArray();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public readonly struct powerup_type_info
    {
        public readonly int vclip_num;
        public readonly int hit_sound;
        public readonly int size;           //3d size of longest dimension
        public readonly int light;      //	amount of light cast by this powerup, set in bitmaps.tbl
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public unsafe struct weapon_info
    {
        public byte render_type;               // How to draw 0=laser, 1=blob, 2=object
        public byte persistent;                    //	0 = dies when it hits something, 1 = continues (eg, fusion cannon)
        public short model_num;                    // Model num if rendertype==2.
        public short model_num_inner;          // Model num of inner part if rendertype==2.

        public byte flash_vclip;               // What vclip to use for muzzle flash
        public byte robot_hit_vclip;           // What vclip for impact with robot
        public short flash_sound;              // What sound to play when fired

        public byte wall_hit_vclip;            // What vclip for impact with wall
        public byte fire_count;                    //	Number of bursts fired from EACH GUN per firing.  For weapons which fire from both sides, 3*fire_count shots will be fired.
        public short robot_hit_sound;          // What sound for impact with robot

        public byte ammo_usage;                    //	How many units of ammunition it uses.
        public byte weapon_vclip;              //	Vclip to render for the weapon, itself.
        public short wall_hit_sound;           // What sound for impact with wall

        public byte destroyable;               //	If !0, this weapon can be destroyed by another weapon.
        public byte matter;                        //	Flag: set if this object is matter (as opposed to energy)
        public byte bounce;                        //	1==always bounces, 2=bounces twice 
        public byte homing_flag;               //	Set if this weapon can home in on a target.

        public byte speedvar;                 //	allowed variance in speed below average, /128: 64 = 50% meaning if speed = 100, can be 50..100

        public byte flags;                        // see values above

        public byte flash;                     //	Flash effect
        public byte afterburner_size;          //	Size of blobs in F1_0/16 units, specify in bitmaps.tbl as floating point.  Player afterburner size = 2.5.

        public byte children;                  //	ID of weapon to drop if this contains children.  -1 means no children.

        public int energy_usage;               //	How much fuel is consumed to fire this weapon.
        public int fire_wait;                  //	Time until this weapon can be fired again.

        public int multi_damage_scale;     //	Scale damage by this amount when applying to player in multiplayer.  F1_0 means no change.

        public BitmapIndex bitmap;                // Pointer to bitmap if rendertype==0 or 1.

        public int blob_size;                  // Size of blob if blob type
        public int flash_size;                 // How big to draw the flash
        public int impact_size;                // How big of an impact
        public int[] strength;              // How much damage it can inflict
        public int[] speed;                 // How fast it can move, difficulty level based.
        public int mass;                           // How much mass it has
        public int drag;                           // How much drag it has
        public int thrust;                     //	How much thrust it has
        public int po_len_to_width_ratio;  // For polyobjects, the ratio of len/width. (10 maybe?)
        public int light;                      //	Amount of light this weapon casts.
        public int lifetime;                   //	Lifetime in seconds of this weapon.
        public int damage_radius;              //	Radius of damage caused by weapon, used for missiles (not lasers) to apply to damage to things it did not hit
                                               //-- unused--	fix	damage_force;				//	Force of damage caused by weapon, used for missiles (not lasers) to apply to damage to things it did not hit
                                               // damage_force was a real mess.  Wasn't Difficulty_level based, and was being applied instead of weapon's actual strength.  Now use 2*strength instead. --MK, 01/19/95
        public BitmapIndex picture;               // a picture of the weapon for the cockpit
        public BitmapIndex hires_picture;     // a hires picture of the above
    }
}
