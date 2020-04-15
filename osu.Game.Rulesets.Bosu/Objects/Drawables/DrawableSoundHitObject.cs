using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSoundHitObject : DrawableBosuHitObject
    {
        public DrawableSoundHitObject(SoundHitObject h)
            : base(h)
        {
        }

        protected override bool CollidedWithPlayer(BosuPlayer player) => false;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
                ApplyResult(r => r.Type = HitResult.Meh);
        }
    }
}
