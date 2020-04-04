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
                    Name = @"Bullets Count",
                    Content = HitObjects.Count(s => s is Bullet).ToString(),
                    Icon = FontAwesome.Regular.Circle
                }
            };
        }
    }
}
