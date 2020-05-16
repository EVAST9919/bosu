using osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class MathExtensions
    {
        public static float Map(float value, float lowerCurrent, float upperCurrent, float lowerTarget, float upperTarget)
        {
            return (value - lowerCurrent) / (upperCurrent - lowerCurrent) * (upperTarget - lowerTarget) + lowerTarget;
        }

        public static float BulletDistribution(int bulletsPerObject, float angleRange, int index, float angleOffset = 0)
        {
            var angle = getAngleBuffer(bulletsPerObject, angleRange) + index * getPerBulletAngle(bulletsPerObject, angleRange) + angleOffset;

            if (angle < 0)
                angle += 360;

            if (angle > 360)
                angle %= 360;

            return angle;

            static float getAngleBuffer(int bulletsPerObject, float angleRange) => (360 - angleRange + getPerBulletAngle(bulletsPerObject, angleRange)) / 2f;

            static float getPerBulletAngle(int bulletsPerObject, float angleRange) => angleRange / bulletsPerObject;
        }

        public static Vector2 GetRotatedPosition(Vector2 input, Vector2 origin, float angle)
        {
            double newX = origin.X + (input.X - origin.X) * Math.Cos(angle * Math.PI / 180) - ((input.Y - origin.Y) * Math.Sin(angle * Math.PI / 180));
            double newY = origin.Y + (input.Y - origin.Y) * Math.Cos(angle * Math.PI / 180) - ((input.X - origin.X) * Math.Sin(angle * Math.PI / 180));

            return new Vector2((float)newX, (float)newY);
        }

        public static float GetRandomTimedAngleOffset(double time)
        {
            var random = new Random((int)Math.Round(time * 100));
            return (float)random.NextDouble() * 360f;
        }

        public static bool GetRandomTimedBool(double time)
        {
            var random = new Random((int)Math.Round(time * 100));
            return random.NextDouble() > 0.5f;
        }

        public static float GetPlayerAngle(BosuPlayer player, Vector2 input)
        {
            var playerPosition = player.PlayerPosition();
            var angle = (float)(Math.Atan2(input.Y - playerPosition.Y, input.X - playerPosition.X) * 180 / Math.PI) + 270;

            if (angle > 360)
                angle %= 360f;

            return angle;
        }

        public static float GetPlayerReverseAngle(BosuPlayer player, Vector2 input)
        {
            var playerPosition = player.PlayerPosition();
            var angle = (float)(Math.Atan2(input.Y - playerPosition.Y, input.X - playerPosition.X) * 180 / Math.PI) + 90;

            if (angle < 0)
                angle += 360;

            if (angle > 360)
                angle %= 360f;

            return angle;
        }

        public static double Distance(Vector2 input, Vector2 target)
        {
            var x = Math.Abs(input.X - target.X);
            var y = Math.Abs(input.Y - target.Y);

            return Math.Sqrt(Pow(x) + Pow(y));
        }

        public static double Pow(float x)
        {
            return x * x;
        }

        public static float GetSafeAngle(float angle)
        {
            if (angle < 0)
            {
                while (angle < 0)
                    angle += 360;

                return angle;
            }

            if (angle > 360)
            {
                angle %= 360f;
                return angle;
            }

            return angle;
        }

        public static bool Collided(float circleRadius, Vector2 circlePosition, Vector2 rectPosition, Vector2 rectSize)
        {
            var deltaX = circlePosition.X - Math.Max(rectPosition.X, Math.Min(circlePosition.X, rectPosition.X + rectSize.X));
            var deltaY = circlePosition.Y - Math.Max(rectPosition.Y, Math.Min(circlePosition.Y, rectPosition.Y + rectSize.Y));
            return Pow(deltaX) + Pow(deltaY) < Pow(circleRadius);
        }
    }
}
