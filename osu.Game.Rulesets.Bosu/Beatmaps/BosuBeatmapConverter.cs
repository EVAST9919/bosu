using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osuTK;
using osu.Game.Audio;
using osu.Game.Beatmaps.ControlPoints;
using System;

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

        private const int max_visuals_per_slider_span = 100;

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
                        Vector2 sliderEventPosition;

                        // Don't take into account very small sliders. There's a chance that they will contain reverse spam, and offset looks ugly
                        if (spanDuration < 75)
                            sliderEventPosition = objPosition * new Vector2(1, 0.5f);
                        else
                            sliderEventPosition = (curve.CurvePositionAt(e.PathProgress / (curve.RepeatCount + 1)) + objPosition) * new Vector2(1, 0.5f);

                        switch (e.Type)
                        {
                            case SliderEventType.Head:
                                hitObjects.AddRange(generateExplosion(
                                    e.Time,
                                    bullets_per_slider_head,
                                    sliderEventPosition,
                                    comboData,
                                    index));

                                hitObjects.Add(new SoundHitObject
                                {
                                    StartTime = obj.StartTime,
                                    Samples = obj.Samples
                                });

                                break;

                            case SliderEventType.Tick:
                                hitObjects.Add(new TickCherry
                                {
                                    StartTime = e.Time,
                                    Position = sliderEventPosition,
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
                                hitObjects.AddRange(generateExplosion(
                                    obj.StartTime + (e.SpanIndex + 1) * spanDuration,
                                    bullets_per_slider_reverse,
                                    sliderEventPosition,
                                    comboData,
                                    index,
                                    slider_angle_per_span * e.SpanIndex));

                                hitObjects.Add(new SoundHitObject
                                {
                                    StartTime = e.Time,
                                    Samples = obj.Samples
                                });
                                break;

                            case SliderEventType.Tail:
                                hitObjects.AddRange(generateExplosion(
                                    e.Time,
                                    bullets_per_slider_tail,
                                    sliderEventPosition,
                                    comboData,
                                    index));

                                hitObjects.Add(new SoundHitObject
                                {
                                    StartTime = curve.EndTime,
                                    Samples = obj.Samples
                                });
                                break;
                        }
                    }

                    //body

                    var bodyCherriesCount = Math.Min(curve.Distance * (curve.RepeatCount + 1) / 10, max_visuals_per_slider_span * (curve.RepeatCount + 1));

                    for(int i = 0; i < bodyCherriesCount; i++)
                    {
                        var progress = (float)i / bodyCherriesCount;
                        var position = (curve.CurvePositionAt(progress) + objPosition) * new Vector2(1, 0.5f);

                        hitObjects.Add(new SliderPartCherry
                        {
                            StartTime = obj.StartTime + curve.Duration * progress,
                            Position = position,
                            NewCombo = comboData?.NewCombo ?? false,
                            ComboOffset = comboData?.ComboOffset ?? 0,
                            IndexInBeatmap = index
                        });
                    }

                    return hitObjects;

                // Spinner
                case IHasEndTime endTime:
                    var spansPerSpinner = endTime.Duration / spinner_span_delay;

                    for (int i = 0; i < spansPerSpinner; i++)
                    {
                        hitObjects.AddRange(generateExplosion(
                            obj.StartTime + i * spinner_span_delay,
                            bullets_per_spinner_span,
                            objPosition * new Vector2(1, 0.5f),
                            comboData,
                            index,
                            i * spinner_angle_per_span));
                    }

                    return hitObjects;

                // Hitcircle
                default:
                    hitObjects.AddRange(generateExplosion(
                        obj.StartTime,
                        bullets_per_hitcircle,
                        objPosition * new Vector2(1, 0.5f),
                        comboData,
                        index,
                        0,
                        120));

                    hitObjects.Add(new SoundHitObject
                    {
                        StartTime = obj.StartTime,
                        Samples = obj.Samples
                    });

                    return hitObjects;
            }
        }

        private IEnumerable<MovingCherry> generateExplosion(double startTime, int bulletCount, Vector2 position, IHasCombo comboData, int index, float angleOffset = 0, float angleRange = 360f)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                yield return new MovingCherry
                {
                    Angle = getBulletDistribution(bulletCount, angleRange, i) + angleOffset,
                    StartTime = startTime,
                    Position = position,
                    NewCombo = comboData?.NewCombo ?? false,
                    ComboOffset = comboData?.ComboOffset ?? 0,
                    IndexInBeatmap = index
                };
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
