using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;
using System;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModAutoplay : ModAutoplay<BosuHitObject>
    {
        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new User { Username = "bosu!" } },
            Replay = new BosuAutoGenerator(beatmap).Generate(),
        };

        public override string Name => "Visual";

        public override string Acronym => "VS";

        public override string Description => "Watch patterns";

        public override Type[] IncompatibleMods => new[]
        {
            typeof(ModFlashlight)
        };
    }
}
