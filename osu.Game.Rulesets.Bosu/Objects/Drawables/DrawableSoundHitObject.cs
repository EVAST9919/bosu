using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSoundHitObject : DrawableBosuHitObject
    {
        protected override float SamplePlaybackPosition => HitObject.X / BosuPlayfield.BASE_SIZE.X;

        public DrawableSoundHitObject(SoundHitObject h)
            : base(h)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
                ApplyResult(r => r.Type = HitResult.Meh);
        }
    }
}
