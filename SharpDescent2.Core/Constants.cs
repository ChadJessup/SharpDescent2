namespace SharpDescent2.Core
{
    public static class Game
    {
        public const int IMMORTAL_TIME = 0x3fffffff;    // Time assigned to immortal objects, about 32768 seconds, or about 9 hours.
        public const int ONE_FRAME_TIME = 0x3ffffffe;   // Objects with this lifeleft will live for exactly one frame

        public const int NDL = 5;       //	Number of difficulty levels.
        public const int NUM_DETAIL_LEVELS = 6;
    }

    public static class Robots
    {
        //Animation states
        public const int AS_REST = 0;
        public const int AS_ALERT = 1;
        public const int AS_FIRE = 2;
        public const int AS_RECOIL = 3;
        public const int AS_FLINCH = 4;
        public const int N_ANIM_STATES = 5;

        public const int RI_CLOAKED_NEVER = 0;
        public const int RI_CLOAKED_ALWAYS = 1;
        public const int RI_CLOAKED_EXCEPT_FIRING = 2;

    }

    public static class MAX
    {
        public const int CONTROLCEN_GUNS = 8;
        public const int PRIMARY_WEAPONS = 10;
        public const int SECONDARY_WEAPONS = 10;

        public const int POLYGON_MODELS = 200;
        public const int GUNS = 8;

        public const int SEGMENTS = 900;
        public const int SEGMENT_VERTICES = 3600;

        public const int SUBMODELS = 10;			//how many animating sub-objects per model

        // Set maximum values for segment and face data structures.
        public const int VERTICES_PER_SEGMENT = 8;
        public const int SIDES_PER_SEGMENT = 6;
        public const int VERTICES_PER_POLY = 4;

        public const int OBJECTS = 350;		//increased on 01/24/95 for multiplayer. --MK;  total number of objects in world
        public const int OBJECT_TYPES = 16;

        public const int REACTORS = 7;
    }

    public static class SegmentConstants
    {
        public const int SIDE_IS_QUAD = 1;          // render side as quadrilateral
        public const int SIDE_IS_TRI_02 = 2;            // render side as two triangles, triangulated along edge from 0 to 2
        public const int SIDE_IS_TRI_13 = 3;            // render side as two triangles, triangulated along edge from 1 to 3

        public const int WLEFT = 0;
        public const int WTOP = 1;
        public const int WRIGHT = 2;
        public const int WBOTTOM = 3;
        public const int WBACK = 4;
        public const int WFRONT = 5;
    }

    public static class Objects
    {
        //	This is specific to the shortpos extraction routines in gameseg.c.
        public const int RELPOS_PRECISION = 10;
        public const int MATRIX_PRECISION = 9;
        public const int MATRIX_MAX = 0x7f;		//	This is based on MATRIX_PRECISION, 9 => 0x7f

        public const int PF_SPAT_BY_PLAYER = 1;		//this powerup was spat by the player   
    }
}
