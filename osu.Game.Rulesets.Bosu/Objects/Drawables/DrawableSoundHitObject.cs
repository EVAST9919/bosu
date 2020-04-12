using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSoundHitObject : DrawableBosuHitObject
    {
        public DrawableSoundHitObject(SoundHitObject h)
            : base(h)
        {
            OnReady += _ => ApplyResult(r => r.Type = HitResult.Meh);
        }

        protected override bool CollidedWithPlayer(BosuPlayer player) => false;
    }
}
