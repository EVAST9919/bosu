using osu.Framework.Graphics;
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

        protected override void OnObjectReady()
        {
            base.OnObjectReady();
            ApplyResult(r => r.Type = HitResult.Meh);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                case ArmedState.Miss:
                    this.FadeOut(200);
                    break;
            }
        }
    }
}
