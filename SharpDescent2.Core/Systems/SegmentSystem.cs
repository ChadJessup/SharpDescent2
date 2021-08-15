using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.DataStructures;

namespace SharpDescent2.Core.Systems;

public class SegmentSystem : IGamePlatformManager
{
    private readonly ILogger<SegmentSystem> logger;

    public SegmentSystem(ILogger<SegmentSystem> logger)
    {
        this.logger = logger;
    }

    public bool IsInitialized { get; }
    public segment[] Segments { get; } = new segment[MAX.SEGMENTS];
    public segment2[] Segment2s { get; } = new segment2[MAX.SEGMENTS];

    public ValueTask<bool> Initialize()
    {
        return ValueTask.FromResult(true);
    }

    public void Dispose()
    {
    }
}

public class segment
{
    public side[] sides = new side[MAX.SIDES_PER_SEGMENT];  // 6 sides
    public short[] children = new short[MAX.SIDES_PER_SEGMENT];  // indices of 6 children segments, front, left, top, right, bottom, back
    public short[] verts = new short[MAX.VERTICES_PER_SEGMENT];  // vertex ids of 4 front and 4 back vertices
    public int objects;                                // pointer to objects in this segment
}

public class segment2
{
    public byte special;
    public byte matcen_num;
    public byte value;
    public byte s2_flags;
    public int static_light;
}

public class side
{
    public byte type;                                  // replaces num_faces and tri_edge, 1 = quad, 2 = 0:2 triangulation, 3 = 1:3 triangulation
    public byte pad;                                  //keep us longword alligned
    public short wall_num;
    public short tmap_num;
    public short tmap_num2;
    public uvl[] uvls = new uvl[4];
    public vms_vector[] normals = new vms_vector[2];                      // 2 normals, if quadrilateral, both the same.
}

public struct uvl
{
    public int u, v, l;
}
