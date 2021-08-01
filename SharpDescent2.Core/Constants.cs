namespace SharpDescent2.Core
{
    public static class Game
    {
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

        public const int MAX_GUNS = 8;
    }
}
