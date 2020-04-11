using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSoundHitObject : DrawableBosuHitObject
    {
        public DrawableSoundHitObject(SoundHitObject h)
            : base(h)
        {
        }

        protected override void OnObjectReady()
        {
            base.OnObjectReady();
            ApplyResult(r => r.Type = HitResult.Meh);
        }
    }
}
