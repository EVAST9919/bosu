using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    /// <summary>
    /// Used only to play hitsounds.
    /// </summary>
    public class GhostCherry : BosuHitObject
    {
        public override Judgement CreateJudgement() => new IgnoreJudgement();
    }
}
