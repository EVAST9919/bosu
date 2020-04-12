using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModHidden : ModHidden
    {
        public override string Description => @"Play with fading fruits.";
        public override double ScoreMultiplier => 1.06;

        private const double fade_out_offset_multiplier = 0.7;
        private const double fade_out_duration_multiplier = 0.44;

        protected override void ApplyHiddenState(DrawableHitObject drawable, ArmedState state)
        {
            if (!(drawable is DrawableCherry))
                return;

            ((DrawableCherry)drawable).OnReady += onObjectReady;
        }

        private void onObjectReady(DrawableBosuHitObject cherry)
        {
            var hitObject = cherry.HitObject;

            var offset = hitObject.TimePreempt * fade_out_offset_multiplier;
            var duration = hitObject.TimePreempt * fade_out_duration_multiplier;

            cherry.Delay(offset).FadeOut(duration);
        }
    }
}
