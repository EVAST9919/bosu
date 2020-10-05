using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Judgements
{
    public class TickJudgement : BosuJudgement
    {
        protected override double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return 0.0003f;

                case HitResult.Miss:
                    return -0.075f;
            }

            return base.HealthIncreaseFor(result);
        }
    }
}
