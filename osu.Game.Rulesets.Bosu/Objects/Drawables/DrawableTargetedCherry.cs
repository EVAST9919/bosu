using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableTargetedCherry : DrawableMovingCherry
    {
        public DrawableTargetedCherry(TargetedCherry h)
            : base(h)
        {
            OnReady += _ => Angle = getAngle() - 90;
        }

        private float getAngle()
        {
            var playerPosition = Player.PlayerPositionInPlayfieldSpace();
            return (float)(Math.Atan2(HitObject.Y - playerPosition.Y, HitObject.X - playerPosition.X) * 180 / Math.PI);
        }
    }
}
