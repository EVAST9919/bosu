using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableBosuHitObject : DrawableHitObject<BosuHitObject>
    {
        protected BosuPlayer Player;

        private bool isReady;

        protected DrawableBosuHitObject(BosuHitObject hitObject)
            : base(hitObject)
        {
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            Scheduler.AddDelayed(OnObjectReady, GetReadyStateOffset());
        }

        protected virtual void OnObjectReady()
        {
            isReady = true;
        }

        protected virtual float GetReadyStateOffset() => (float)HitObject.TimePreempt;

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

        public void GetPlayerToTrace(BosuPlayer player) => Player = player;

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                case ArmedState.Miss:
                    this.FadeOut();
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!isReady || (Result?.HasResult ?? true))
                return;

            OnObjectUpdate();
        }

        protected virtual void OnObjectUpdate()
        {
            if (CheckPlayerCollision(Player))
            {
                Player.PlayMissAnimation();
                ApplyResult(r => r.Type = HitResult.Miss);
            }
        }

        protected virtual bool CheckPlayerCollision(BosuPlayer player) => false;
    }
}
