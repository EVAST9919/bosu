using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableHomingCherry : DrawableAngledCherry
    {
        public DrawableHomingCherry(HomingCherry h)
            : base(h)
        {
        }

        protected override float GetAngle()
        {
            var playerPosition = Player.PlayerPosition();
            var angle = (float)(Math.Atan2(HitObject.Y - playerPosition.Y, HitObject.X - playerPosition.X) * 180 / Math.PI) + 270;

            if (angle > 360)
                angle %= 360f;

            return angle;
        }
    }
}
