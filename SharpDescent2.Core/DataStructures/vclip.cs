using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures;

// 82
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public unsafe struct vclip
{
    public int play_time;          //total time (in seconds) of clip
    public int num_frames;
    public int frame_time;         //time (in seconds) of each frame
    public int flags;
    public short sound_num;
    public fixed short frames[30];
    public int light_value;
}
