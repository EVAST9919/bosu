using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableBosuHitObject : DrawableHitObject<BosuHitObject>
    {
        protected BosuPlayer Player;

        protected DrawableBosuHitObject(BosuHitObject hitObject)
            : base(hitObject)
        {
        }

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

        public void GetPlayerToTrace(BosuPlayer player) => Player = player;

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Idle:
                    // Manually set to reduce the number of future alive objects to a bare minimum.
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
                    break;
            }
        }
    }
}
