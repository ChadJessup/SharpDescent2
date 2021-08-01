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

}
