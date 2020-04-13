using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Beatmaps;
using osu.Game.Rulesets.Mods;
using System;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModSymmetry : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Symmetry";
        public override string Acronym => "SM";
        public override ModType Type => ModType.Fun;
        public override string Description => "Everything is symmetrical";

        public override double ScoreMultiplier => 1.7;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var bosuBeatmapConverter = (BosuBeatmapConverter)beatmapConverter;
            bosuBeatmapConverter.Symmetry = true;
        }

        public override Type[] IncompatibleMods => new[]
        {
            typeof(BosuModSliders),
        };
    }
}
