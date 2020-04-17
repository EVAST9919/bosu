using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModHidden : ModHidden
    {
        public override string Description => @"Play with fading fruits.";
        public override double ScoreMultiplier => 1.06;

        protected override void ApplyHiddenState(DrawableHitObject drawable, ArmedState state)
        {
            if (!(drawable is DrawableMovingCherry))
                return;

            var drawableCherry = (DrawableMovingCherry)drawable;
            var cherry = (MovingCherry)drawableCherry.HitObject;

            using (drawableCherry.BeginAbsoluteSequence(cherry.StartTime))
                drawableCherry.FadeOut(cherry.TimePreempt * 1.5f);
        }
    }
}
