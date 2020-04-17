using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class TickCherry : MovingCherry
    {
        public override Judgement CreateJudgement() => new TickJudgement();
    }
}
