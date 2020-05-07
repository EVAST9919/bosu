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
            var angled = HitObjects.Count(s => s is AngledCherry);
            var sound = HitObjects.Count(s => s is SoundHitObject);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = @"Cherries",
                    Content = angled.ToString(),
                    Icon = FontAwesome.Regular.Circle
                },
                new BeatmapStatistic
                {
                    Name = @"Visuals",
                    Content = (totalCount - angled - sound).ToString(),
                    Icon = FontAwesome.Regular.Circle
                }
            };
        }
    }
}
