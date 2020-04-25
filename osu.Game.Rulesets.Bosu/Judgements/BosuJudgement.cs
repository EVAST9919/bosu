using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Judgements
{
    public class BosuJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.Perfect;

        protected override double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return 0.0006f;

                case HitResult.Miss:
                    return -0.15f;

                default:
                    return 0;
            }
        }

        protected override int NumericResultFor(HitResult result)
        {
            switch (result)
            {
                default:
                    return 0;

                case HitResult.Perfect:
                    return 300;
            }
        }
    }
}
