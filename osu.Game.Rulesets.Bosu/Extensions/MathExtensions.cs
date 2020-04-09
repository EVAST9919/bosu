namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class MathExtensions
    {
        public static float Map(float value, float lowerCurrent, float upperCurrent, float lowerTarget, float upperTarget)
        {
            return ((value - lowerCurrent) / (upperCurrent - lowerCurrent)) * (upperTarget - lowerTarget) + lowerTarget;
        }
    }
}
