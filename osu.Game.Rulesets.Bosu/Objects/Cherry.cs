using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osuTK.Graphics;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class Cherry : BosuHitObject, IHasComboInformation
    {
        public float CircleSize { get; set; } = 1;

        public bool IsKiai { get; set; }

        public override Judgement CreateJudgement() => new IgnoreJudgement();

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);
            CircleSize = difficulty.CircleSize;
        }

        Color4 IHasComboInformation.GetComboColour(IReadOnlyList<Color4> comboColours) => comboColours[(IndexInBeatmap + 1) % comboColours.Count];
    }
}
