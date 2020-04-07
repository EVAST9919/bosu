using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class GhostCherry : BosuHitObject
    {
        public override Judgement CreateJudgement() => new IgnoreJudgement();
    }
}
