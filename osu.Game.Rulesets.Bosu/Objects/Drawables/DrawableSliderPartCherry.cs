using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSliderPartCherry : DrawableCherry
    {
        public DrawableSliderPartCherry(SliderPartCherry h)
            : base(h)
        {
        }

        protected override bool CollidedWithPlayer(BosuPlayer player) => false;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
                ApplyResult(r => r.Type = HitResult.Meh);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    this.ScaleTo(0, 150).Then().FadeOut();
                    break;
            }
        }
    }
}
