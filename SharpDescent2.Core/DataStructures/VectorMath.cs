using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures
{
    //Angle vector.  Used to store orientations
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct vms_angvec
    {
        public short p;
        public short b;
        public short h;
    }

    public struct vms_vector
    {
        public int x;
        public int y;
        public int z;
    }

    public struct vms_vector_array
    {
        public int[] xyz;//[3];
    }

    //Short vector, used for pre-rotation points. 
    //Access elements by name or position
    public struct vms_svec
    {
        public short sv_x, sv_y, sv_z;
    }

    //A 3x3 rotation matrix.  Sorry about the numbering starting with one.
    //Ordering is across then down, so <m1,m2,m3> is the first row
    public struct vms_matrix
    {
        vms_vector rvec, uvec, fvec;
    }
}
