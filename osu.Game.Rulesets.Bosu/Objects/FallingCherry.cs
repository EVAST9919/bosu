using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class FallingCherry : Cherry
    {
        public double SpeedMultiplier { get; set; } = 1;

        public override Judgement CreateJudgement() => new BosuJudgement();

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);
            SpeedMultiplier = controlPointInfo.DifficultyPointAt(StartTime).SpeedMultiplier;
        }
    }
}
