using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableTargetedCherry : DrawableMovingCherry
    {
        public DrawableTargetedCherry(TargetedCherry h)
            : base(h)
        {
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            Scheduler.AddDelayed(() => Angle = getAngle() - 90, HitObject.TimePreempt);
        }

        private float getAngle()
        {
            var playerPosition = Player.PlayerPositionInPlayfieldSpace();
            return (float)(Math.Atan2(HitObject.Y - playerPosition.Y, HitObject.X - playerPosition.X) * 180 / Math.PI);
        }
    }
}
