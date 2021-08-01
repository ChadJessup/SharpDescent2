using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public unsafe struct wclip
    {
        public int play_time;
        public short num_frames;
        public fixed short frames[50];
        public short open_sound;
        public short close_sound;
        public short flags;
        public fixed byte filename[13];
        public byte pad;
    }
}
