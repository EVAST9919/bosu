using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Beatmaps
{
    public class BosuBeatmapConverter : BeatmapConverter<BosuHitObject>
    {
        private const int bullets_per_hitcircle = 10;
        private const int bullets_per_slider = 30;
        private const int bullets_per_spinner_span = 50;

        private const float spinner_span_delay = 250f;

        public BosuBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition);

        private int index = -1;

        protected override IEnumerable<BosuHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap)
        {
            var positionData = obj as IHasPosition;
            var comboData = obj as IHasCombo;
            var difficulty = beatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty;

            if (comboData?.NewCombo ?? false)
                index++;

            List<Cherry> bullets = new List<Cherry>();
            int bulletCount;

            switch (obj)
            {
                // Slider
                case IHasCurve _:
                    bulletCount = getAdjustedObjectCount(bullets_per_slider, difficulty);

                    for (int i = 0; i < bulletCount; i++)
                    {
                        bullets.Add(new Cherry
                        {
                            Angle = getBulletDistribution(bulletCount, 360f, i),
                            StartTime = obj.StartTime,
                            Position = new Vector2(positionData?.X ?? 0, positionData?.Y * 0.5f ?? 0),
                            NewCombo = comboData?.NewCombo ?? false,
                            ComboOffset = comboData?.ComboOffset ?? 0,
                            IndexInBeatmap = index
                        });
                    }

                    return bullets;

                // Spinner
                case IHasEndTime endTime:
                    var spansPerSpinner = endTime.Duration / spinner_span_delay;
                    bulletCount = getAdjustedObjectCount(bullets_per_spinner_span, difficulty);

                    for (int i = 0; i < spansPerSpinner; i++)
                    {
                        for (int j = 0; j < bulletCount; j++)
                        {
                            bullets.Add(new Cherry
                            {
                                Angle = getBulletDistribution(bulletCount, 360f, j) + (i * 2),
                                StartTime = obj.StartTime + i * spinner_span_delay,
                                Position = new Vector2(positionData?.X ?? 0, positionData?.Y * 0.5f ?? 0),
                                NewCombo = comboData?.NewCombo ?? false,
                                ComboOffset = comboData?.ComboOffset ?? 0,
                                IndexInBeatmap = index
                            });
                        }
                    }

                    return bullets;

                default:
                    bulletCount = getAdjustedObjectCount(bullets_per_hitcircle, difficulty);

                    for (int i = 0; i < bulletCount; i++)
                    {
                        bullets.Add(new Cherry
                        {
                            Angle = getBulletDistribution(bulletCount, 100, i),
                            StartTime = obj.StartTime,
                            Position = new Vector2(positionData?.X ?? 0, positionData?.Y * 0.5f ?? 0),
                            NewCombo = comboData?.NewCombo ?? false,
                            ComboOffset = comboData?.ComboOffset ?? 0,
                            IndexInBeatmap = index
                        });
                    }

                    return bullets;
            }
        }

        protected override Beatmap<BosuHitObject> CreateBeatmap() => new BosuBeatmap();

        private float getBulletDistribution(int bulletsPerObject, float angleRange, int index)
        {
            return getAngleBuffer(bulletsPerObject, angleRange) + index * getPerBulletAngle(bulletsPerObject, angleRange);

            static float getAngleBuffer(int bulletsPerObject, float angleRange) => (360 - angleRange + getPerBulletAngle(bulletsPerObject, angleRange)) / 2f;

            static float getPerBulletAngle(int bulletsPerObject, float angleRange) => angleRange / bulletsPerObject;
        }

        private int getAdjustedObjectCount(int baseValue, float difficulty) => (int)Math.Floor(baseValue * Math.Clamp(difficulty, 4, 10) * 0.05f);
    }
}
