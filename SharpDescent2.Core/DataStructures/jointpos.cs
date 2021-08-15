using System.Runtime.InteropServices;

namespace SharpDescent2.Core.DataStructures;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public readonly struct jointpos
{
    public readonly short jointnum;
    public readonly vms_angvec angles;
}
