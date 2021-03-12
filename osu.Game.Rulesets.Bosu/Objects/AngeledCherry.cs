using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Judgements;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class AngeledCherry : Cherry
    {
        public readonly Bindable<float> SpeedMultiplierBindable = new Bindable<float>(1);

        public float SpeedMultiplier
        {
            get => SpeedMultiplierBindable.Value;
            set => SpeedMultiplierBindable.Value = value;
        }

        public readonly Bindable<float> AngleBindable = new Bindable<float>();

        public float Angle
        {
            get => AngleBindable.Value;
            set => AngleBindable.Value = value;
        }

        public double Duration;
        public Vector2 EndPosition;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            EndPosition = getFinalPosition();
            Duration = Vector2.Distance(Position, EndPosition) / SpeedMultiplierBindable.Value * 2.8f;
        }

        public override Judgement CreateJudgement() => new BosuJudgement();

        private Vector2 getFinalPosition()
        {
            var angle = MathExtensions.GetSafeAngle(Angle);

            float finalX = 0;
            float finalY = 0;

            switch (getTargetWall(angle))
            {
                case Wall.Bottom:
                    finalY = BosuPlayfield.BASE_SIZE.Y;
                    finalX = getXPosition(Position, finalY, angle);
                    break;

                case Wall.Top:
                    finalY = 0;
                    finalX = getXPosition(Position, finalY, angle);
                    break;

                case Wall.Left:
                    finalX = 0;
                    finalY = getYPosition(Position, finalX, angle);
                    break;

                case Wall.Right:
                    finalX = BosuPlayfield.BASE_SIZE.X;
                    finalY = getYPosition(Position, finalX, angle);
                    break;
            }

            return new Vector2(finalX, finalY);
        }

        private Wall getTargetWall(float angle)
        {
            // Top/Right
            if (angle <= 90)
            {
                if (angle < getCornerAngle(new Vector2(BosuPlayfield.BASE_SIZE.X, 0)))
                    return Wall.Top;

                return Wall.Right;
            }

            // Right/Bottom
            if (angle <= 180)
            {
                if (angle < getCornerAngle(new Vector2(BosuPlayfield.BASE_SIZE.X, BosuPlayfield.BASE_SIZE.Y)))
                    return Wall.Right;

                return Wall.Bottom;
            }

            // Bottom/Left
            if (angle <= 270)
            {
                if (angle < getCornerAngle(new Vector2(0, BosuPlayfield.BASE_SIZE.Y)))
                    return Wall.Bottom;

                return Wall.Left;
            }

            // Left/Top
            if (angle < getCornerAngle(Vector2.Zero))
                return Wall.Left;

            return Wall.Top;
        }

        private float getCornerAngle(Vector2 cornerPosition)
            => MathExtensions.GetSafeAngle((float)(Math.Atan2(Y - cornerPosition.Y, X - cornerPosition.X) * 180 / Math.PI) - 90);

        private static float getYPosition(Vector2 initialPosition, float finalX, float angle)
            => (float)(initialPosition.Y + ((finalX - initialPosition.X) / -Math.Tan(angle * Math.PI / 180)));

        private static float getXPosition(Vector2 initialPosition, float finalY, float angle)
            => (float)(initialPosition.X + ((finalY - initialPosition.Y) * -Math.Tan(angle * Math.PI / 180)));

        private enum Wall
        {
            Top,
            Right,
            Left,
            Bottom
        }
    }
}
