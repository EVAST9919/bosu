using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.UI;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSpinnerBurstCherry : DrawableAngledCherry
    {
        public static float FULL_ROTATION_DURATION = 2000;
        public static float DISTANCE = 40;

        private readonly float burstAngle;
        private readonly Vector2 initialPosition;

        public DrawableSpinnerBurstCherry(SpinnerBurstCherry h)
            : base(h)
        {
            var rotationsPerSpinner = h.SpinnerDuration / FULL_ROTATION_DURATION;
            burstAngle = (float)(h.Angle + (h.SpinnerProgress * rotationsPerSpinner * 360));

            var originPosition = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y / 4f);

            var rotatedXPos = originPosition.X + (DISTANCE * Math.Sin(burstAngle * Math.PI / 180));
            var rotatedYPos = originPosition.Y + (DISTANCE * -Math.Cos(burstAngle * Math.PI / 180));

            initialPosition = new Vector2((float)rotatedXPos, (float)rotatedYPos);

            Position = initialPosition;
        }

        protected override float GetAngle() => MathExtensions.GetSafeAngle(burstAngle);

        protected override Vector2 UpdatePosition(double currentTime)
        {
            var elapsedTime = currentTime - (HitObject.StartTime - HitObject.TimePreempt);
            var xPosition = initialPosition.X + (elapsedTime * SpeedMultiplier * Math.Sin(burstAngle * Math.PI / 180));
            var yPosition = initialPosition.Y + (elapsedTime * SpeedMultiplier * -Math.Cos(burstAngle * Math.PI / 180));
            return new Vector2((float)xPosition, (float)yPosition);
        }
    }
}
