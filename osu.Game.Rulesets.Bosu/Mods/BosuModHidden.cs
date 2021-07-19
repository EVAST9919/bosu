using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModHidden : ModHidden
    {
        public override double ScoreMultiplier => 1.06;

        public override string Description => "Cherries will become invisible near you.";

        public override void ApplyToDrawableHitObject(DrawableHitObject dho)
        {
            base.ApplyToDrawableHitObject(dho);

            if (dho is DrawableAngeledCherry c)
                c.HiddenApplied = true;
        }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }
    }
}
