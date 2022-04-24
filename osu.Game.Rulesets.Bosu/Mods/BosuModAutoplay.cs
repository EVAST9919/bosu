using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Rulesets.Mods;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModAutoplay : ModAutoplay
    {
        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            => new ModReplayData(new BosuAutoGenerator(beatmap, mods).Generate(), new ModCreatedUser { Username = "Autoplay" });
    }
}
