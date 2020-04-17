namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class MathExtensions
    {
        public static float Map(float value, float lowerCurrent, float upperCurrent, float lowerTarget, float upperTarget)
        {
            return (value - lowerCurrent) / (upperCurrent - lowerCurrent) * (upperTarget - lowerTarget) + lowerTarget;
        }

        public static float BulletDistribution(int bulletsPerObject, float angleRange, int index)
        {
            var angle = getAngleBuffer(bulletsPerObject, angleRange) + index * getPerBulletAngle(bulletsPerObject, angleRange);

            if (angle > 360)
                angle %= 360;

            return angle;

            static float getAngleBuffer(int bulletsPerObject, float angleRange) => (360 - angleRange + getPerBulletAngle(bulletsPerObject, angleRange)) / 2f;

            static float getPerBulletAngle(int bulletsPerObject, float angleRange) => angleRange / bulletsPerObject;
        }
    }
}
