using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableEndTimeCherry : DrawableAngledCherry
    {
        private readonly double endTime;

        public DrawableEndTimeCherry(EndTimeCherry h)
            : base(h)
        {
            endTime = h.EndTime;
        }

        private double? finishedXPosition;
        private double? finishedYPosition;
        private float? angle;

        protected override Vector2 UpdatePosition(double currentTime)
        {
            if (currentTime < endTime)
                return base.UpdatePosition(currentTime);

            if (finishedXPosition == null)
                finishedXPosition = Position.X;

            if (finishedYPosition == null)
                finishedYPosition = Position.Y;

            if (angle == null)
                angle = getAngle();

            var elapsedTime = Clock.CurrentTime - endTime;
            var xPosition = finishedXPosition + (elapsedTime * 0.5f * Math.Sin((angle ?? 0) * Math.PI / 180));
            var yPosition = finishedYPosition + (elapsedTime * 0.5f * -Math.Cos((angle ?? 0) * Math.PI / 180));
            return new Vector2((float)xPosition, (float)yPosition);
        }

        private float getAngle() => MathExtensions.GetPlayerReverseAngle(Player, new Vector2((float)(finishedXPosition ?? 0), (float)(finishedYPosition ?? 0)));
    }
}
