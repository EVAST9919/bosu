using osuTK;

namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class IWannaExtensions
    {
        // Player size
        public static readonly Vector2 PLAYER_SIZE = new Vector2(11, 21);
        public static readonly float PLAYER_HALF_WIDTH = 5.5f;
        public static readonly float PLAYER_HALF_HEIGHT = 10.5f;

        // Player movement
        public static readonly double PLAYER_MAX_HORIZONTAL_SPEED = 0.15;
        public static readonly double PLAYER_MAX_VERTICAL_SPEED = 9;
        public static readonly double PLAYER_VERTICAL_STOP_SPEED_MULTIPLIER = 0.45;
        public static readonly double PLAYER_JUMP_SPEED = 8.5;
        public static readonly double PLAYER_JUMP2_SPEED = 7;
        public static readonly double PLAYER_GRAVITY_UP = 0.42;
        public static readonly double PLAYER_GRAVITY_DOWN = 0.442;

        public static readonly int BULLET_SIZE = 3;
        public static readonly int BULLET_SPEED = 16;
        public static readonly int TILE_SIZE = 32;
        public static readonly int CHERRY_DIAMETER = 21;
        public static readonly float CHERRY_RADIUS = 10.5f;
        public static readonly Vector2 CHERRY_FULL_SIZE = new Vector2(21, 24);
        public static readonly int CHERRY_ANIMATION_FRAME_TIME = 400;
    }
}
