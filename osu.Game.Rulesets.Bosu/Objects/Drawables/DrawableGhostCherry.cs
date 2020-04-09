using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableGhostCherry : DrawableBosuHitObject
    {
        public DrawableGhostCherry(GhostCherry hitObject)
            : base(hitObject)
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.AddDelayed(() => ApplyResult(r => r.Type = HitResult.Perfect), HitObject.TimePreempt);
        }

        protected override bool CheckPlayerCollision(BosuPlayer player) => false;
    }
}
