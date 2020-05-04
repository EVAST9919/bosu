using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModHidden : ModHidden
    {
        public override string Description => @"Play with fading cherries.";
        public override double ScoreMultiplier => 1.06;

        protected override void ApplyHiddenState(DrawableHitObject drawable, ArmedState state)
        {
            if (!(drawable is DrawableCherry))
                return;

            var drawableCherry = (DrawableCherry)drawable;
            drawableCherry.HiddenApplied = true;
        }
    }
}
