using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Bosu.Difficulty
{
    public class BosuDifficultyCalculator : DifficultyCalculator
    {
        public BosuDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
        }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        {
            var objectCount = beatmap.HitObjects.Count(h => h is Cherry);
            var calculatedLength = beatmap.BeatmapInfo.Length / 1000 / clockRate;

            var sr = 0.0;
            if (objectCount != 0 && calculatedLength != 0.0)
                sr = objectCount / 15.0 / calculatedLength;

            return new DifficultyAttributes
            {
                StarRating = sr,
                Mods = mods,
                MaxCombo = beatmap.HitObjects.Count(h => h is Cherry)
            };
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            => Array.Empty<DifficultyHitObject>();

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate) => Array.Empty<Skill>();
    }
}
