using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    /// <summary>
    /// Used only for hit-sounds purposes. Will no affect combo/score.
    /// </summary>
    public class SoundHitObject : BosuHitObject
    {
        public override Judgement CreateJudgement() => new NullJudgement();
    }
}
