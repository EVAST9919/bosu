using JetBrains.Annotations;
using osu.Framework.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public partial class DrawableAngledCherry : DrawableCherry<AngledCherry>
    {
        protected override bool CanHitPlayer => true;

        public DrawableAngledCherry()
            : this(null)
        {
        }

        public DrawableAngledCherry([CanBeNull] AngledCherry h = null)
            : base(h)
        {
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            using (BeginDelayedSequence(HitObject.TimePreempt))
            {
                this.MoveTo(HitObject.EndPosition, HitObject.Duration);
            }
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            base.CheckForResult(userTriggered, timeOffset);

            if (timeOffset < HitObject.Duration)
                return;

            ApplyMaxResult();
        }
    }
}
