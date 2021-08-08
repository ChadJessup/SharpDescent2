namespace SharpDescent2.Core.DataStructures
{
    public struct explosion_info
    {
        int spawn_time;         // when lifeleft is < this, spawn another
        int delete_time;        // when to delete object
        short delete_objnum;        // and what object to delete
        short attach_parent;        // explosion is attached to this object
        short prev_attach;      // previous explosion in attach list
        short next_attach;      // next explosion in attach list
    }
}
