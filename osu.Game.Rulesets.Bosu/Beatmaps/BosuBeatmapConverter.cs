using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osuTK;
using osu.Game.Audio;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Rulesets.Bosu.Beatmaps
{
    public class BosuBeatmapConverter : BeatmapConverter<BosuHitObject>
    {
        private const int bullets_per_hitcircle = 4;
        private const int bullets_per_slider_head = 7;
        private const int bullets_per_slider_reverse = 5;
        private const int bullets_per_slider_tail = 5;
        private const int bullets_per_spinner_span = 20;

        private const float spinner_span_delay = 250f;
        private const float spinner_angle_per_span = 8f;

        private const float slider_angle_per_span = 2f;

        public BosuBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition);

        private int index = -1;

        protected override IEnumerable<BosuHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap)
        {
            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;
            var difficulty = beatmap.BeatmapInfo.BaseDifficulty;

            if (comboData?.NewCombo ?? false)
                index++;

            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            switch (obj)
            {
                // Slider
                case IHasCurve curve:

                    // head
                    generateExplosion(hitObjects, obj.StartTime,
                        bullets_per_slider_head, 360, 0,
                        objPosition * new Vector2(1, 0.5f),
                        comboData, index);

                    hitObjects.Add(new SoundHitObject
                    {
                        StartTime = obj.StartTime,
                        Samples = obj.Samples
                    });

                    // ticks & reverse arrows

                    var controlPointInfo = beatmap.ControlPointInfo;
                    TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(obj.StartTime);
                    DifficultyControlPoint difficultyPoint = controlPointInfo.DifficultyPointAt(obj.StartTime);

                    double scoringDistance = 100 * difficulty.SliderMultiplier * difficultyPoint.SpeedMultiplier;

                    var velocity = scoringDistance / timingPoint.BeatLength;
                    var tickDistance = scoringDistance / difficulty.SliderTickRate;

                    double spanDuration = curve.Duration / (curve.RepeatCount + 1);
                    double legacyLastTickOffset = (obj as IHasLegacyLastTickOffset)?.LegacyLastTickOffset ?? 0;

                    foreach (var e in SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset))
                    {
                        var tickPosition = curve.CurvePositionAt(e.PathProgress);
                        var position = (tickPosition + objPosition)
                            * new Vector2(1, 0.5f);

                        switch (e.Type)
                        {
                            case SliderEventType.Tick:
                                hitObjects.Add(new TickCherry
                                {
                                    StartTime = e.Time,
                                    Position = position,
                                    NewCombo = comboData?.NewCombo ?? false,
                                    ComboOffset = comboData?.ComboOffset ?? 0,
                                    IndexInBeatmap = index
                                });

                                hitObjects.Add(new SoundHitObject
                                {
                                    StartTime = e.Time,
                                    Samples = getTickSamples(obj.Samples)
                                });
                                break;

                            case SliderEventType.Repeat:
                                generateExplosion(hitObjects,
                                    obj.StartTime + (e.SpanIndex + 1) * spanDuration,
                                    bullets_per_slider_reverse, 360,
                                    slider_angle_per_span * e.SpanIndex,
                                    position, comboData, index);

                                hitObjects.Add(new SoundHitObject
                                {
                                    StartTime = obj.StartTime + (e.SpanIndex + 1) * spanDuration,
                                    Samples = obj.Samples
                                });
                                break;
                        }
                    }

                    //body

                    var bodyCherriesCount = curve.Distance / 10;

                    for(int i = 0; i < bodyCherriesCount; i++)
                    {
                        var progress = (float)i / bodyCherriesCount;
                        var position = curve.CurvePositionAt(curve.ProgressAt(progress));

                        hitObjects.Add(new SliderPartCherry
                        {
                            StartTime = obj.StartTime + curve.Duration * progress,
                            Position = new Vector2(position.X + objPosition.X, (position.Y + objPosition.Y) * 0.5f),
                            NewCombo = comboData?.NewCombo ?? false,
                            ComboOffset = comboData?.ComboOffset ?? 0,
                            IndexInBeatmap = index
                        });
                    }

                    // tail
                    var tailPosition = curve.CurvePositionAt(curve.ProgressAt(1));

                    generateExplosion(hitObjects, curve.EndTime,
                        bullets_per_slider_tail, 360, 0,
                        (tailPosition + objPosition) * new Vector2(1, 0.5f),
                        comboData, index);

                    hitObjects.Add(new SoundHitObject
                    {
                        StartTime = curve.EndTime,
                        Samples = obj.Samples
                    });

                    return hitObjects;

                // Spinner
                case IHasEndTime endTime:
                    var spansPerSpinner = endTime.Duration / spinner_span_delay;

                    for (int i = 0; i < spansPerSpinner; i++)
                    {
                        generateExplosion(hitObjects,
                            obj.StartTime + i * spinner_span_delay,
                            bullets_per_spinner_span, 360,
                            i * spinner_angle_per_span,
                            objPosition * new Vector2(1, 0.5f),
                            comboData, index);
                    }

                    return hitObjects;

                // Hitcircle
                default:
                    generateExplosion(hitObjects, obj.StartTime,
                        bullets_per_hitcircle, 120, 0,
                        objPosition * new Vector2(1, 0.5f),
                        comboData, index);

                    hitObjects.Add(new SoundHitObject
                    {
                        StartTime = obj.StartTime,
                        Samples = obj.Samples
                    });

                    return hitObjects;
            }
        }

        private static void generateExplosion(List<BosuHitObject> hitObjects,
            double startTime, int numBullets, float angleRange, float offset,
            Vector2 position, IHasCombo comboData, int index)
        {
            for (int i = 0; i < numBullets; i++)
            {
                hitObjects.Add(new MovingCherry
                {
                    Angle = getBulletDistribution(numBullets, angleRange, i)
                        + offset,
                    StartTime = startTime,
                    Position = position,
                    NewCombo = comboData?.NewCombo ?? false,
                    ComboOffset = comboData?.ComboOffset ?? 0,
                    IndexInBeatmap = index
                });
            }
        }

        protected override Beatmap<BosuHitObject> CreateBeatmap() => new BosuBeatmap();

        private static float getBulletDistribution(int bulletsPerObject, float angleRange, int index)
        {
            return getAngleBuffer(bulletsPerObject, angleRange) + index * getPerBulletAngle(bulletsPerObject, angleRange);

            static float getAngleBuffer(int bulletsPerObject, float angleRange) => (360 - angleRange + getPerBulletAngle(bulletsPerObject, angleRange)) / 2f;

            static float getPerBulletAngle(int bulletsPerObject, float angleRange) => angleRange / bulletsPerObject;
        }

        private List<HitSampleInfo> getTickSamples(IList<HitSampleInfo> objSamples) => objSamples.Select(s => new HitSampleInfo
        {
            Bank = s.Bank,
            Name = @"slidertick",
            Volume = s.Volume
        }).ToList();
    }
}
