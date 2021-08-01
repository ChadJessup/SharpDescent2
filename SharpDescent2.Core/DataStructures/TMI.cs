namespace SharpDescent2.Core.DataStructures
{
    public enum TMI : byte
    {
        UNKNOWN = 0,
        //tmapinfo flags
        VOLATILE = 1, //this material blows up when hit
        WATER = 2, //this material is water
        FORCE_FIELD = 4, //this is force field - flares don't stick
        GOAL_BLUE = 8, //this is used to remap the blue goal
        GOAL_RED = 16, //this is used to remap the red goal
        GOAL_HOARD = 32, //this is used to remap the goals
    }

}
