namespace SharpDescent2.Core.DataStructures
{
    //stuctures for different kinds of simulation

    public struct laser_info
    {
        short parent_type;      // The type of the parent of this object
        short parent_num;       // The object's parent's number
        int parent_signature;   // The object's parent's signature...
        int creation_time;      //	Absolute time of creation.
        short last_hitobj;      //	For persistent weapons (survive object collision), object it most recently hit.
        short track_goal;           //	Object this object is tracking.
        int multiplier;         //	Power if this is a fusion bolt (or other super weapon to be added).
    }
}
