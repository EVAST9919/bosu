using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public partial class DrawableAngeledCherry : DrawableCherry<AngeledCherry>
    {
        protected override bool CanHitPlayer => true;

        public DrawableAngeledCherry()
            : this(null)
        {
        }

        public DrawableAngeledCherry([CanBeNull] AngeledCherry h = null)
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

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Hit:
                    this.Delay(HitObject.Duration).FadeOut();
                    break;
            }
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            base.CheckForResult(userTriggered, timeOffset);

            if (timeOffset < HitObject.Duration)
                return;

            ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }
    }
}
