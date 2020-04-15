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

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

        public void GetPlayerToTrace(BosuPlayer player) => Player = player;

        protected override void Update()
        {
            base.Update();

            if (Judged || Time.Current < HitObject.StartTime)
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
        }

        protected abstract bool CollidedWithPlayer(BosuPlayer player);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
            {
                if (CollidedWithPlayer(Player))
                {
                    Player.PlayMissAnimation();
                    ApplyResult(r => r.Type = HitResult.Miss);
                }
            }
        }
    }
}
