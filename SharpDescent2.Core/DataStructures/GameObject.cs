using SharpDescent2.Core.Systems;

namespace SharpDescent2.Core.DataStructures;

public class GameObject
{
    public int signature;      // Every object ever has a unique signature...
    public OBJ type;             // what type of object this is... robot, weapon, hostage, powerup, fireball
    public byte id;               // which form of object...which powerup, robot, etc.
    public int next, prev;       // id of next and previous connected object in Objects, -1 = no connection
    public CT control_type;  // how this object is controlled
    public MT movement_type; // how this object moves
    public RT render_type;  //	how this object renders
    public byte flags;            // misc flags
    public int segnum;           // segment number containing object
    public short attached_obj; // number of attached fireball object
    public vms_vector pos;             // absolute x,y,z coordinate of center of object
    public vms_matrix orient;          // orientation of object in world
    public int size;               // 3d size of object - for collision detection
    public int shields;        // Starts at maximum, when <0, object dies..
    public vms_vector last_pos;        // where object was last frame
    public byte contains_type; //	Type of object this object contains (eg, spider contains powerup)
    public byte contains_id;   //	ID of object this object contains (eg, id = blue type = key)
    public byte contains_count;// number of objects of type:id this object contains
    public byte matcen_creator;//	Materialization center that created this object, high bit set if matcen-created
    public int lifeleft;       // how long until goes away, or 7fff if immortal
                               // -- Removed, MK, 10/16/95, using lifeleft instead:	int			lightlevel;

    public ctype ctype = new ctype();

    //movement info, determined by MOVEMENT_TYPE
    public mtype mtype = new mtype();

    //control info, determined by CONTROL_TYPE


    //render info, determined by RENDER_TYPE
    public rtype rtype = new rtype();

}
