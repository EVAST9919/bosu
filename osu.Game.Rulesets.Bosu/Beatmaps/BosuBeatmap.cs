using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;

namespace osu.Game.Rulesets.Bosu.Beatmaps
{
    public class BosuBeatmap : Beatmap<BosuHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            var totalCount = HitObjects.Count();
            var angled = HitObjects.Count(s => s is AngledCherry);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = @"Cherries",
                    Content = angled.ToString(),
                    CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Circles)
                },
                new BeatmapStatistic
                {
                    Name = @"Visuals",
                    Content = (totalCount - angled).ToString(),
                    CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Spinners)
                }
            };
        }
    }
}
