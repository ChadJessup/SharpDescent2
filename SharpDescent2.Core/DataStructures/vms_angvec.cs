using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public readonly struct vms_angvec
    {
        public readonly short p;
        public readonly short b;
        public readonly short h;
    }
}
