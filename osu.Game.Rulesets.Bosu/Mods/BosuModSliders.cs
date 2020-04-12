using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModSliders : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Sliders";
        public override string Acronym => "SL";
        public override ModType Type => ModType.Automation;
        public override string Description => "Relax and watch some sliders";

        public override double ScoreMultiplier => 0;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var bosuBeatmapConverter = (BosuBeatmapConverter)beatmapConverter;
            bosuBeatmapConverter.SlidersOnly = true;
        }
    }
}
