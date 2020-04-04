using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class Bullet : BosuHitObject
    {
        public float Angle { get; set; }

        public override Judgement CreateJudgement() => new BosuJudgement();
    }
}
