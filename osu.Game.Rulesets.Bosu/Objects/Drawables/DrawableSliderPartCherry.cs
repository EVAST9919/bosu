using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSliderPartCherry : DrawableCherry
    {
        private readonly Bindable<double> opacityBindable = new Bindable<double>();

        public DrawableSliderPartCherry(SliderPartCherry h)
            : base(h)
        {
        }

        [BackgroundDependencyLoader]
        private void load(BosuRulesetConfigManager config)
        {
            config.BindWith(BosuRulesetSetting.SliderOpacity, opacityBindable);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            opacityBindable.BindValueChanged(opacity => onOpacityChanged(opacity.NewValue), true);
        }

        private void onOpacityChanged(double opacity)
        {
            Content.Alpha = (float)(1 - opacity);
        }

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
