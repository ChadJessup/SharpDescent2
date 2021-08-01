using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TextureMapInfo
    {
        public readonly TMI flags;          //values defined above
        public readonly byte pad1;          //keep alignment
        public readonly byte pad2;          //keep alignment
        public readonly byte pad3;          //keep alignment
        public readonly int lighting;       //how much light this casts
        public readonly int damage;         //how much damage being against this does (for lava)
        public readonly short eclip_num;    //the eclip that changes this, or -1
        public readonly short destroyed;    //bitmap to show when destroyed, or -1
        public readonly short slide_u;      //slide rates of texture, stored in 8:8 fix
        public readonly short slide_v;
    }

}
