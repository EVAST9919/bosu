using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public partial class DrawableInstantCherry : DrawableCherry<InstantCherry>
    {
        protected override bool CanHitPlayer => false;

        public DrawableInstantCherry()
            : this(null)
        {
        }

        public DrawableInstantCherry([CanBeNull] InstantCherry h = null)
            : base(h)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            base.CheckForResult(userTriggered, timeOffset);

            if (timeOffset < 0)
                return;

            ApplyResult(HitResult.IgnoreHit);
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
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
