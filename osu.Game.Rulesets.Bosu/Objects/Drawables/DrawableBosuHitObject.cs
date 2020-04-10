using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableBosuHitObject : DrawableHitObject<BosuHitObject>
    {
        protected BosuPlayer Player;
        protected bool ShouldCheckCollision;

        protected DrawableBosuHitObject(BosuHitObject hitObject)
            : base(hitObject)
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.AddDelayed(OnObjectReady, GetReadyStateOffset());
        }

        protected virtual void OnObjectReady()
        {
            ShouldCheckCollision = true;
        }

        protected virtual float GetReadyStateOffset() => (float)HitObject.TimePreempt;

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

        public void GetPlayerToTrace(BosuPlayer player) => Player = player;

        protected override void Update()
        {
            base.Update();

            if (!ShouldCheckCollision)
                return;

            if (CheckPlayerCollision(Player))
            {
                Player.PlayMissAnimation();
                ApplyResult(r => r.Type = HitResult.Miss);
                ShouldCheckCollision = false;
            }
        }

        protected abstract bool CheckPlayerCollision(BosuPlayer player);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
        }
    }
}
