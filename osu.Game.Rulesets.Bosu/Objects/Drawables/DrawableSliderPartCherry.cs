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
            Scheduler.AddDelayed(() => ApplyResult(r => r.Type = HitResult.Meh), 20);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                case ArmedState.Miss:
                    this.ScaleTo(0, 150).Then().FadeOut();
                    break;
            }
        }
    }
}
