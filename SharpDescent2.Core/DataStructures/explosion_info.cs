namespace SharpDescent2.Core.DataStructures;

public struct explosion_info
{
    public int spawn_time;         // when lifeleft is < this, spawn another
    public int delete_time;        // when to delete object
    public short delete_objnum;        // and what object to delete
    public short attach_parent;        // explosion is attached to this object
    public short prev_attach;      // previous explosion in attach list
    public short next_attach;      // next explosion in attach list
}
