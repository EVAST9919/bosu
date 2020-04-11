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
            var totalCount = HitObjects.Count();
            var hitCount = HitObjects.Count(s => s is MovingCherry);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = @"Cherries",
                    Content = hitCount.ToString(),
                    Icon = FontAwesome.Regular.Circle
                },
                new BeatmapStatistic
                {
                    Name = @"Visual objects",
                    Content = (totalCount - hitCount).ToString(),
                    Icon = FontAwesome.Regular.Circle
                }
            };
        }
    }
}
