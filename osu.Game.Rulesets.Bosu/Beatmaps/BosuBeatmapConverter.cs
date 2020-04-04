using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Beatmaps
{
    public class BosuBeatmapConverter : BeatmapConverter<BosuHitObject>
    {
        private const int bullets_per_hitcircle = 5;
        private const int bullets_per_slider = 15;
        private const int bullets_per_spinner_span = 30;

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

            if (comboData?.NewCombo ?? false)
                index++;

            List<Bullet> bullets = new List<Bullet>();

            switch (obj)
            {
                // Slider
                case IHasCurve _:
                    for (int i = 0; i < bullets_per_slider; i++)
                    {
                        bullets.Add(new Bullet
                        {
                            Angle = getBulletDistribution(bullets_per_slider, 360f, i),
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
                    var bulletsPerSpinner = endTime.Duration / spinner_span_delay;

                    for (int i = 0; i < bulletsPerSpinner; i++)
                    {
                        for (int j = 0; j < bullets_per_spinner_span; j++)
                        {
                            bullets.Add(new Bullet
                            {
                                Angle = getBulletDistribution(bullets_per_spinner_span, 360f, j) + (i * 2),
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
                    for (int i = 0; i < bullets_per_hitcircle; i++)
                    {
                        bullets.Add(new Bullet
                        {
                            Angle = getBulletDistribution(bullets_per_hitcircle, 100, i),
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
    }
}
