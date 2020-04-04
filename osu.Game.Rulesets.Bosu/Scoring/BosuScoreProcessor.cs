using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Scoring
{
    public class BosuScoreProcessor : ScoreProcessor
    {
        public override HitWindows CreateHitWindows() => new BosuHitWindows();
    }
}
