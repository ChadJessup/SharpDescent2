namespace SharpDescent2.Core.DataStructures.AI;

public static class AIConstants
{
    //	Constants defining meaning of flags in ai_state
    public const int MAX_AI_FLAGS = 11; // This MUST cause word (4 bytes) alignment in ai_static, allowing for one byte mode
}

//	This is the stuff that is permanent for an AI object and is therefore saved to disk.
public class ai_static
{
    public byte behavior;                 // 
    public byte[] flags = new byte[AIConstants.MAX_AI_FLAGS];       // various flags, meaning defined by constants
    public short hide_segment;             //	Segment to go to for hiding.
    public short hide_index;                   //	Index in Path_seg_points
    public short path_length;              //	Length of hide path.
    public byte cur_path_index;            //	Current index in path.
    public byte dying_sound_playing;       //	!0 if this robot is playing its dying sound.

    // -- not needed! -- 	short			follow_path_start_seg;	//	Start segment for robot which follows path.
    // -- not needed! -- 	short			follow_path_end_seg;		//	End segment for robot which follows path.

    public short danger_laser_num;
    public int danger_laser_signature;
    public int dying_start_time;           //	Time at which this robot started dying.

    //	byte			extras[28];					//	32 extra bytes for storing stuff so we don't have to change versions on disk
}
