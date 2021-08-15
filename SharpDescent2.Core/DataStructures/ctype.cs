using SharpDescent2.Core.DataStructures.AI;

namespace SharpDescent2.Core.DataStructures;

public struct ctype
{
    public laser_info laser_info;
    public explosion_info expl_info;       //NOTE: debris uses this also
    public ai_static ai_info;
    public light_info light_info;      //why put this here?  Didn't know what else to do with it.
    public powerup_info powerup_info;
}
