using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct eclip
{
    public readonly vclip vc;               //imbedded vclip
    public int time_left;      //for sequencing
    public readonly int frame_count;    //for sequencing
    public readonly short changing_wall_texture;            //Which element of Textures array to replace.
    public readonly short changing_object_texture;      //Which element of ObjBitmapPtrs array to replace.
    public readonly int flags;          //see above
    public readonly int crit_clip;      //use this clip instead of above one when mine critical
    public readonly int dest_bm_num;    //use this bitmap when monitor destroyed
    public readonly int dest_vclip;     //what vclip to play when exploding
    public readonly int dest_eclip;     //what eclip to play when exploding
    public readonly int dest_size;      //3d size of explosion
    public readonly int sound_num;      //what sound this makes
    public readonly int segnum, sidenum;    //what seg & side, for one-shot clips
}
