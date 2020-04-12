using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableBosuHitObject : DrawableHitObject<BosuHitObject>
    {
        protected BosuPlayer Player;

        public Action<DrawableBosuHitObject> OnReady;

        protected DrawableBosuHitObject(BosuHitObject hitObject)
            : base(hitObject)
        {
        }

        protected virtual float GetReadyStateOffset() => (float)HitObject.TimePreempt;

        protected sealed override double InitialLifetimeOffset => GetReadyStateOffset();

        public void GetPlayerToTrace(BosuPlayer player) => Player = player;

        protected override void Update()
        {
            base.Update();

            if (Judged || Clock.CurrentTime < HitObject.StartTime)
            {
                invoked = false;
                return;
            }

            OnObjectUpdate();
        }

        private bool invoked;

        protected virtual void OnObjectUpdate()
        {
            if (!invoked)
            {
                OnReady?.Invoke(this);
                invoked = true;
            }

            if (CollidedWithPlayer(Player))
            {
                Player.PlayMissAnimation();
                ApplyResult(r => r.Type = HitResult.Miss);
            }
        }

        protected abstract bool CollidedWithPlayer(BosuPlayer player);
    }
}
