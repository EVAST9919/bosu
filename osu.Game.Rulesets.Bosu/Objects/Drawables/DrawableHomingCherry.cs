using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableHomingCherry : DrawableAngledCherry
    {
        public DrawableHomingCherry(HomingCherry h)
            : base(h)
        {
        }

        protected override float GetAngle() => MathExtensions.GetPlayerAngle(Player, HitObject.Position);
    }
}
