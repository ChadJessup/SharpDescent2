using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.DataStructures;
using SharpDescent2.Core.Loaders;

using static SharpDescent2.Core.VectorMath;

namespace SharpDescent2.Core.Systems;

public class ObjectSystem : IGamePlatformManager
{
    private readonly ILogger<ObjectSystem> logger;
    private readonly CollisionSystem collision;
    private readonly SegmentSystem segments;
    private readonly PolyObjSystem Polygon;
    private readonly ILibraryManager library;

    public ObjectSystem(
        ILogger<ObjectSystem> logger,
        ILibraryManager library,
        CollisionSystem collision,
        SegmentSystem segmentSystem,
        PolyObjSystem polyObjSystem)
    {
        this.logger = logger;
        this.library = library;
        this.collision = collision;
        this.segments = segmentSystem;
        this.Polygon = polyObjSystem;
    }

    public bool IsInitialized { get; private set; }

    public GameObject[] GameObjects { get; } = new GameObject[MAX.OBJECTS];
    public GameObject ConsoleObject { get; private set; }
    public GameObject Viewer { get; private set; }

    static int[] free_obj_list = new int[MAX.OBJECTS];

    private int num_objects = 0;
    private int Highest_object_index = 0;
    private int Highest_ever_object_index = 0;

    public async ValueTask<bool> Initialize()
    {
        this.IsInitialized = await this.collision.Initialize();

        var ham = (HAMArchive)this.library.GetLibrary("descent2.ham");
        Polygon.models = ham.PolyModels;
        Player.ship = ham.PlayerShip;

        for (int i = 0; i < MAX.OBJECTS; i++)
        {
            free_obj_list[i] = i;
            GameObjects[i] = new GameObject
            {
                type = OBJ.NONE,
                segnum = -1,
            };
        }

        for (int i = 0; i < MAX.SEGMENTS; i++)
        {
            this.segments.Segments[i] = new segment
            {
                objects = -1,
            };
        }

        ConsoleObject = Viewer = GameObjects[0];

        init_player_object();
        obj_link(0, 0);   //put in the world in segment 0

        num_objects = 1;                        //just the player
        Highest_object_index = 0;

        return this.IsInitialized;
    }

    //make object0 the player, setting all relevant fields
    private void init_player_object()
    {
        this.ConsoleObject.type = OBJ.PLAYER;
        ConsoleObject.id = 0;                  //no sub-types for player

        ConsoleObject.signature = 0;           //player has zero, others start at 1

        ConsoleObject.size = Polygon.models[Player.ship.model_num].rad;

        ConsoleObject.control_type = CT.SLEW;          //default is player slewing
        ConsoleObject.movement_type = MT.PHYSICS;      //change this sometime

        ConsoleObject.lifeleft = Game.IMMORTAL_TIME;

        ConsoleObject.attached_obj = -1;

        reset_player_object();
    }

    private void obj_link(int objnum, int segnum)
    {
        GameObject obj = this.GameObjects[objnum];

        // Assert(objnum != -1);
        // Assert(obj->segnum == -1);
        // Assert(segnum >= 0 && segnum <= Highest_segment_index);

        obj.segnum = segnum;

        obj.next = this.segments.Segments[segnum].objects;
        obj.prev = -1;

        this.segments.Segments[segnum].objects = objnum;

        if (obj.next != -1)
        {
            GameObjects[obj.next].prev = objnum;
        }

        //list_seg_objects( segnum );
        //check_duplicate_objects();

        //Assert(Objects[0].next != 0);
        if (GameObjects[0].next == 0)
        {
            GameObjects[0].next = -1;
        }

        //Assert(Objects[0].prev != 0);
        if (GameObjects[0].prev == 0)
        {
            GameObjects[0].prev = -1;
        }
    }


    private void reset_player_object()
    {
        int i;

        //Init physics

        vm_vec_zero(ConsoleObject.mtype.phys_info.velocity);
        vm_vec_zero(ConsoleObject.mtype.phys_info.thrust);
        vm_vec_zero(ConsoleObject.mtype.phys_info.rotvel);
        vm_vec_zero(ConsoleObject.mtype.phys_info.rotthrust);
        ConsoleObject.mtype.phys_info.brakes = ConsoleObject.mtype.phys_info.turnroll = 0;
        ConsoleObject.mtype.phys_info.mass = Player.ship.mass;
        ConsoleObject.mtype.phys_info.drag = Player.ship.drag;
        ConsoleObject.mtype.phys_info.flags |= PF.TURNROLL | PF.LEVELLING | PF.WIGGLE | PF.USES_THRUST;

        //Init render info

        ConsoleObject.render_type = RT.POLYOBJ;

        ConsoleObject.rtype.pobj_info = new();
        ConsoleObject.rtype.pobj_info.model_num = Player.ship.model_num;      //what model is this?
        ConsoleObject.rtype.pobj_info.subobj_flags = 0;        //zero the flags
        ConsoleObject.rtype.pobj_info.tmap_override = -1;      //no tmap override!

        for (i = 0; i < MAX.SUBMODELS; i++)
        {
            vm_angvec_zero(ConsoleObject.rtype.pobj_info.anim_angles[i]);
        }

        // Clear misc

        ConsoleObject.flags = 0;

    }

    public void Dispose()
    {
    }
}

//Movement types
public enum MT
{
    NONE = 0,   //doesn't move
    PHYSICS = 1,    //moves by physics
    SPINNING = 3,   //this object doesn't move, just sits and spins
}

//Control types - what tells this object what do do
public enum CT
{
    NONE = 0,//doesn't move (or change movement)
    AI = 1,//driven by AI
    EXPLOSION = 2,//explosion sequencer
    FLYING = 4,//the player is flying
    SLEW = 5,//slewing
    FLYTHROUGH = 6,//the flythrough system
    WEAPON = 9, //laser, etc.
    REPAIRCEN = 10,//under the control of the repair center
    MORPH = 11,//this object is being morphed
    DEBRIS = 12,//this is a piece of debris
    POWERUP = 13,//animating powerup blob
    LIGHT = 14,//doesn't actually do anything
    REMOTE = 15,//controlled by another net player
    CNTRLCEN = 16,//the control center/main reactor 
}

//Render types
public enum RT
{
    NONE = 0,           //does not render
    POLYOBJ = 1,        //a polygon model
    FIREBALL = 2,       //a fireball
    LASER = 3,          //a laser
    HOSTAGE = 4,        //a hostage
    POWERUP = 5,        //a powerup
    MORPH = 6,          //a robot being morphed
    WEAPON_VCLIP = 7,   //a weapon that renders as a vclip
}

public enum OBJ : int
{
    NONE = 255,     //unused object
    WALL = 0,       //A wall... not really an object, but used for collisions
    FIREBALL = 1,   //a fireball, part of an explosion
    ROBOT = 2,      //an evil enemy
    HOSTAGE = 3,    //a hostage you need to rescue
    PLAYER = 4,     //the player on the console
    WEAPON = 5,     //a laser, missile, etc
    CAMERA = 6,     //a camera to slew around with
    POWERUP = 7,    //a powerup you can pick up
    DEBRIS = 8,     //a piece of robot
    CNTRLCEN = 9,   //the control center
    FLARE = 10,     //a flare
    CLUTTER = 11,   //misc objects
    GHOST = 12,     //what the player turns into when dead
    LIGHT = 13,     //a light source, & not much else
    COOP = 14,      //a cooperative player object.
    MARKER = 15,    //a map marker
}
