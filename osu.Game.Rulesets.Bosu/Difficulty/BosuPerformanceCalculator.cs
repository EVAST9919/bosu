using osu.Game.Rulesets.Difficulty;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Bosu.Difficulty
{
    public class BosuPerformanceCalculator : PerformanceCalculator
    {
        public BosuPerformanceCalculator(Ruleset ruleset, DifficultyAttributes attributes, ScoreInfo score)
            : base(ruleset, attributes, score)
        {
        }

        public override PerformanceAttributes Calculate()
        {
            double accuracyValue = 0.0;

            return new BosuPerformanceAttributes
            {
                Accuracy = accuracyValue
            };
        }
    }
}
