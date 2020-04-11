using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;

namespace osu.Game.Rulesets.Bosu.Beatmaps
{
    public class BosuBeatmap : Beatmap<BosuHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            return new[]
            {
                new BeatmapStatistic
                {
                    Name = @"Cherries Count",
                    Content = HitObjects.Count(s => s is Cherry).ToString(),
                    Icon = FontAwesome.Regular.Circle
                }
            };
        }
    }
}
