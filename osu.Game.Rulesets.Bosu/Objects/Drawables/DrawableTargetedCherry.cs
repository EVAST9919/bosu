using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableTargetedCherry : DrawableMovingCherry
    {
        public DrawableTargetedCherry(TargetedCherry h)
            : base(h)
        {
        }

        protected override void OnObjectReady()
        {
            Angle = getAngle() - 90;
            base.OnObjectReady();
        }

        private float getAngle()
        {
            var playerPosition = Player.PlayerPositionInPlayfieldSpace();
            return (float)(Math.Atan2(HitObject.Y - playerPosition.Y, HitObject.X - playerPosition.X) * 180 / Math.PI);
        }
    }
}
