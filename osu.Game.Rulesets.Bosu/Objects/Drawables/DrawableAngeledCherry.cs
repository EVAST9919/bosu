using JetBrains.Annotations;
using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableAngeledCherry : DrawableConstantMovingCherry<AngeledCherry>
    {
        public readonly IBindable<float> AngleBindable = new Bindable<float>();

        protected override float GetTargetAngle() => AngleBindable.Value;

        public DrawableAngeledCherry()
            : this(null)
        {
        }

        public DrawableAngeledCherry([CanBeNull] AngeledCherry h = null)
            : base(h)
        {
        }

        protected override void OnApply()
        {
            base.OnApply();

            AngleBindable.BindTo(HitObject.AngleBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();

            AngleBindable.UnbindFrom(HitObject.AngleBindable);
        }
    }
}
