using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Mods;
using System;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Scoring;
using osu.Game.Users;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModSliders : ModAutoplay<BosuHitObject>, IApplicableToBeatmapConverter
    {
        public override string Name => "Sliders";
        public override string Acronym => "SL";
        public override string Description => "Watch some trippy sliders";

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var bosuBeatmapConverter = (BosuBeatmapConverter)beatmapConverter;
            bosuBeatmapConverter.SlidersOnly = true;
        }

        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new User { Username = "bosu!" } },
            Replay = new BosuAutoGenerator(beatmap).Generate(),
        };

        public override Type[] IncompatibleMods => new[]
        {
            typeof(BosuModAutoplay),
            typeof(BosuModSymmetry),
            typeof(ModFlashlight),
            typeof(ModRelax),
        };
    }
}
