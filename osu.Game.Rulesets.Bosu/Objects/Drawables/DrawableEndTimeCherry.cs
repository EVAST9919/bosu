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

        private float getAngle()
        {
            var playerPosition = Player.PlayerPosition();
            var angle = (float)(Math.Atan2((finishedYPosition ?? 0) - playerPosition.Y, (finishedXPosition ?? 0) - playerPosition.X) * 180 / Math.PI) + 90;

            if (angle < 0)
                angle += 360;

            if (angle > 360)
                angle %= 360f;

            return angle;
        }
    }
}
