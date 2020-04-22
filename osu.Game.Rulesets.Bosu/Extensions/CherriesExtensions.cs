using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class CherriesExtensions
    {
        private const int bullets_per_hitcircle = 4;

        private const int bullets_per_slider_reverse = 5;

        private const float slider_angle_per_span = 2f;
        private const int max_visuals_per_slider_span = 100;

        private const int bullets_per_spinner_span = 20;
        private const float spinner_span_delay = 250f;
        private const float spinner_angle_per_span = 8f;

        public static List<BosuHitObject> ConvertSlider(HitObject obj, IBeatmap beatmap, IHasCurve curve, int index)
        {
            double spanDuration = curve.Duration / (curve.RepeatCount + 1);
            bool isRepeatSpam = spanDuration < 75 && curve.RepeatCount > 0;

            if (isRepeatSpam)
                return generateRepeatSpamSlider(obj, beatmap, curve, spanDuration, index);
            else
                return generateDefaultSlider(obj, beatmap, curve, spanDuration, index);
        }

        public static List<BosuHitObject> ConvertHitCircle(HitObject obj, int index)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;

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

        public static List<BosuHitObject> ConvertSpinner(HitObject obj, IHasEndTime endTime, int index, int stageIndex)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;

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
        }

        private static IEnumerable<AngledCherry> generateExplosion(double startTime, int bulletCount, Vector2 position, IHasCombo comboData, int index, float angleOffset = 0, float angleRange = 360f)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                yield return new AngledCherry
                {
                    Angle = MathExtensions.BulletDistribution(bulletCount, angleRange, i, angleOffset),
                    StartTime = startTime,
                    Position = position,
                    NewCombo = comboData?.NewCombo ?? false,
                    ComboOffset = comboData?.ComboOffset ?? 0,
                    IndexInBeatmap = index
                };
            }
        }

        private static List<BosuHitObject> generateDefaultSlider(HitObject obj, IBeatmap beatmap, IHasCurve curve, double spanDuration, int index)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;
            var difficulty = beatmap.BeatmapInfo.BaseDifficulty;

            var controlPointInfo = beatmap.ControlPointInfo;
            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(obj.StartTime);
            DifficultyControlPoint difficultyPoint = controlPointInfo.DifficultyPointAt(obj.StartTime);

            double scoringDistance = 100 * difficulty.SliderMultiplier * difficultyPoint.SpeedMultiplier;

            var velocity = scoringDistance / timingPoint.BeatLength;
            var tickDistance = scoringDistance / difficulty.SliderTickRate;

            double legacyLastTickOffset = (obj as IHasLegacyLastTickOffset)?.LegacyLastTickOffset ?? 0;

            foreach (var e in SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset))
            {
                var sliderEventPosition = (curve.CurvePositionAt(e.PathProgress / (curve.RepeatCount + 1)) + objPosition) * new Vector2(1, 0.5f);

                switch (e.Type)
                {
                    case SliderEventType.Head:

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = obj.StartTime,
                            Samples = obj.Samples
                        });

                        break;

                    case SliderEventType.Tick:

                        if (positionIsValid(sliderEventPosition))
                        {
                            hitObjects.Add(new TickCherry
                            {
                                Angle = 180,
                                StartTime = e.Time,
                                Position = sliderEventPosition,
                                NewCombo = comboData?.NewCombo ?? false,
                                ComboOffset = comboData?.ComboOffset ?? 0,
                                IndexInBeatmap = index
                            });
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = e.Time,
                            Samples = getTickSamples(obj.Samples)
                        });
                        break;

                    case SliderEventType.Repeat:

                        if (positionIsValid(sliderEventPosition))
                        {
                            hitObjects.AddRange(generateExplosion(
                                e.Time,
                                bullets_per_slider_reverse,
                                sliderEventPosition,
                                comboData,
                                index));
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = e.Time,
                            Samples = obj.Samples
                        });
                        break;

                    case SliderEventType.Tail:

                        if (positionIsValid(sliderEventPosition))
                        {
                            hitObjects.AddRange(generateExplosion(
                                e.Time,
                                Math.Clamp((int)curve.Distance / 15, 5, 20),
                                sliderEventPosition,
                                comboData,
                                index));
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = curve.EndTime,
                            Samples = obj.Samples
                        });
                        break;
                }
            }

            hitObjects.AddRange(generateSliderBody(obj, curve, index));

            return hitObjects;
        }

        private static List<BosuHitObject> generateRepeatSpamSlider(HitObject obj, IBeatmap beatmap, IHasCurve curve, double spanDuration, int index)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;
            var difficulty = beatmap.BeatmapInfo.BaseDifficulty;

            var controlPointInfo = beatmap.ControlPointInfo;
            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(obj.StartTime);
            DifficultyControlPoint difficultyPoint = controlPointInfo.DifficultyPointAt(obj.StartTime);

            double scoringDistance = 100 * difficulty.SliderMultiplier * difficultyPoint.SpeedMultiplier;

            var velocity = scoringDistance / timingPoint.BeatLength;
            var tickDistance = scoringDistance / difficulty.SliderTickRate;

            double legacyLastTickOffset = (obj as IHasLegacyLastTickOffset)?.LegacyLastTickOffset ?? 0;

            foreach (var e in SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset))
            {
                var sliderEventPosition = objPosition * new Vector2(1, 0.5f);

                switch (e.Type)
                {
                    case SliderEventType.Head:

                        if (positionIsValid(sliderEventPosition))
                        {
                            hitObjects.AddRange(generateExplosion(
                                e.Time,
                                bullets_per_slider_reverse,
                                sliderEventPosition,
                                comboData,
                                index));
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = obj.StartTime,
                            Samples = obj.Samples
                        });

                        break;

                    case SliderEventType.Repeat:

                        if (positionIsValid(sliderEventPosition))
                        {
                            hitObjects.AddRange(generateExplosion(
                                e.Time,
                                bullets_per_slider_reverse,
                                sliderEventPosition,
                                comboData,
                                index,
                                slider_angle_per_span * (e.SpanIndex + 1)));
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = e.Time,
                            Samples = obj.Samples
                        });
                        break;

                    case SliderEventType.Tail:

                        if (positionIsValid(sliderEventPosition))
                        {
                            hitObjects.AddRange(generateExplosion(
                                e.Time,
                                bullets_per_slider_reverse,
                                sliderEventPosition,
                                comboData,
                                index,
                                slider_angle_per_span * (curve.RepeatCount + 1)));
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = curve.EndTime,
                            Samples = obj.Samples
                        });
                        break;
                }
            }

            hitObjects.AddRange(generateSliderBody(obj, curve, index));

            return hitObjects;
        }

        private static List<SliderPartCherry> generateSliderBody(HitObject obj, IHasCurve curve, int index)
        {
            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;

            List<SliderPartCherry> hitObjects = new List<SliderPartCherry>();

            var bodyCherriesCount = Math.Min(curve.Distance * (curve.RepeatCount + 1) / 10, max_visuals_per_slider_span * (curve.RepeatCount + 1));

            for (int i = 0; i < bodyCherriesCount; i++)
            {
                var progress = (float)i / bodyCherriesCount;
                var position = curve.CurvePositionAt(progress) + objPosition;

                position *= new Vector2(1, 0.5f);

                if (positionIsValid(position))
                {
                    hitObjects.Add(new SliderPartCherry
                    {
                        StartTime = obj.StartTime + curve.Duration * progress,
                        Position = position,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                        IndexInBeatmap = index
                    });
                }
            }

            return hitObjects;
        }

        private static Vector2 getSymmetricalXPosition(Vector2 input) => new Vector2(BosuPlayfield.BASE_SIZE.X - input.X, input.Y);

        private static Vector2 getSymmetricalYPosition(Vector2 input) => new Vector2(input.X, BosuPlayfield.BASE_SIZE.Y - input.Y);

        private static List<HitSampleInfo> getTickSamples(IList<HitSampleInfo> objSamples) => objSamples.Select(s => new HitSampleInfo
        {
            Bank = s.Bank,
            Name = @"slidertick",
            Volume = s.Volume
        }).ToList();

        private static bool positionIsValid(Vector2 position)
        {
            if (position.X > BosuPlayfield.BASE_SIZE.X || position.X < 0 || position.Y < 0 || position.Y > BosuPlayfield.BASE_SIZE.Y)
                return false;

            return true;
        }

        private static float[] getRandomTimedXPosition(double time, int count)
        {
            var random = new Random((int)Math.Round(time));

            float[] randoms = new float[count];

            for (int i = 0; i < count; i++)
                randoms[i] = (float)random.NextDouble() * BosuPlayfield.BASE_SIZE.X;

            return randoms;
        }
    }
}
