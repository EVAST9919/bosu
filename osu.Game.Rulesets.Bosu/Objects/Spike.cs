using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class Spike : BosuHitObject
    {
        public IHasCurve Curve { get; set; }

        public override Judgement CreateJudgement() => new BosuJudgement();
    }
}
