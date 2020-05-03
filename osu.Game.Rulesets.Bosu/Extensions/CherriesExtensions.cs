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
        private const int bullets_per_hitcircle = 10;
        private const int hitcircle_angle_offset = 5;

        private const int bullets_per_slider_reverse = 5;

        private const float slider_angle_per_span = 2f;
        private const int max_visuals_per_slider_span = 100;

        private const int bullets_per_spinner_span = 20;
        private const float spinner_angle_per_span = 8f;

        public static List<BosuHitObject> ConvertSlider(HitObject obj, IBeatmap beatmap, IHasCurve curve, bool isKiai, int index)
        {
            double spanDuration = curve.Duration / (curve.RepeatCount + 1);
            bool isRepeatSpam = spanDuration < 75 && curve.RepeatCount > 0;

            if (isRepeatSpam)
                return generateRepeatSpamSlider(obj, beatmap, curve, spanDuration, isKiai, index);
            else
                return generateDefaultSlider(obj, beatmap, curve, spanDuration, isKiai, index);
        }

        public static List<BosuHitObject> ConvertHitCircle(HitObject obj, bool isKiai, int index, int indexInCurrentCombo)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var circlePosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            circlePosition *= new Vector2(1, 0.5f);
            var comboData = obj as IHasCombo;

            if (indexInCurrentCombo == 0)
            {
                hitObjects.AddRange(generatePolygonExplosion(
                    obj.StartTime,
                    5,
                    3,
                    circlePosition,
                    comboData,
                    isKiai,
                    index,
                    MathExtensions.GetRandomTimedAngleOffset(obj.StartTime)));
            }
            else
            {
                hitObjects.AddRange(generateExplosion(
                    obj.StartTime,
                    bullets_per_hitcircle,
                    circlePosition,
                    comboData,
                    isKiai,
                    index,
                    hitcircle_angle_offset * indexInCurrentCombo));
            }

            hitObjects.Add(new SoundHitObject
            {
                StartTime = obj.StartTime,
                Samples = obj.Samples,
                Position = circlePosition
            });

            return hitObjects;
        }

        public static List<BosuHitObject> ConvertSpinner(HitObject obj, IHasEndTime endTime, double beatLength, bool isKiai, int index, int stageIndex)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;

            // Fast bpm spinners are almost impossible to pass, nerf them.
            if (beatLength < 400)
            {
                while (beatLength < 400)
                    beatLength *= 2f;
            }

            var spansPerSpinner = endTime.Duration / beatLength;

            for (int i = 0; i < spansPerSpinner; i++)
            {
                hitObjects.AddRange(generateExplosion(
                    obj.StartTime + i * beatLength,
                    bullets_per_spinner_span,
                    objPosition * new Vector2(1, 0.5f),
                    comboData,
                    isKiai,
                    index,
                    i * spinner_angle_per_span));
            }

            return hitObjects;
        }

        private static IEnumerable<AngledCherry> generateExplosion(double startTime, int bulletCount, Vector2 position, IHasCombo comboData, bool isKiai, int index, float angleOffset = 0, float angleRange = 360f)
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
                    IndexInBeatmap = index,
                    IsKiai = isKiai
                };
            }
        }

        private static IEnumerable<BosuHitObject> generatePolygonExplosion(double startTime, int bullets_per_side, int verticesCount, Vector2 position, IHasCombo comboData, bool isKiai, int index, float angleOffset = 0)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            for (int i = 0; i < verticesCount; i++)
                hitObjects.AddRange(generatePolygonLine(startTime, bullets_per_side, verticesCount, position, comboData, isKiai, index, i * (360f / verticesCount) + angleOffset));

            return hitObjects;
        }

        private static IEnumerable<AngledCherry> generatePolygonLine(double startTime, int bullets_per_side, int verticesCount, Vector2 position, IHasCombo comboData, bool isKiai, int index, float additionalOffset = 0)
        {
            var s = 1.0;
            var side = s / (2 * Math.Sin(360.0 / (2 * verticesCount) * Math.PI / 180));
            var partDistance = s / bullets_per_side;
            var partialAngle = 180 * (verticesCount - 2) / (2 * verticesCount);

            for (int i = 0; i < bullets_per_side; i++)
            {
                var c = (float)partDistance * i;
                var length = Math.Sqrt(MathExtensions.Pow((float)side) + MathExtensions.Pow(c) - (2 * side * c * Math.Cos(partialAngle * Math.PI / 180)));
                var missingAngle = Math.Acos((MathExtensions.Pow((float)side) + MathExtensions.Pow((float)length) - MathExtensions.Pow(c)) / (2 * side * length)) * 180 / Math.PI;
                var currentAngle = 180 + (90 - partialAngle) - missingAngle;

                yield return new AngledCherry
                {
                    Angle = (float)currentAngle + additionalOffset,
                    DeltaMultiplier = length / side,
                    StartTime = startTime,
                    Position = position,
                    NewCombo = comboData?.NewCombo ?? false,
                    ComboOffset = comboData?.ComboOffset ?? 0,
                    IndexInBeatmap = index,
                    IsKiai = isKiai,
                };
            }
        }

        private static IEnumerable<EndTimeCherry> generateEndTimeExplosion(double startTime, double endTime, int bulletCount, Vector2 position, IHasCombo comboData, bool isKiai, int index, float angleOffset = 0, float angleRange = 360f)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                yield return new EndTimeCherry
                {
                    Angle = MathExtensions.BulletDistribution(bulletCount, angleRange, i, angleOffset),
                    StartTime = startTime,
                    EndTime = endTime,
                    Position = position,
                    NewCombo = comboData?.NewCombo ?? false,
                    ComboOffset = comboData?.ComboOffset ?? 0,
                    IndexInBeatmap = index,
                    IsKiai = isKiai
                };
            }
        }

        private static List<BosuHitObject> generateDefaultSlider(HitObject obj, IBeatmap beatmap, IHasCurve curve, double spanDuration, bool isKiai, int index)
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
                            Samples = obj.Samples,
                            Position = sliderEventPosition
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
                                IndexInBeatmap = index,
                                IsKiai = isKiai
                            });
                        }

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = e.Time,
                            Samples = getTickSamples(obj.Samples),
                            Position = sliderEventPosition
                        });
                        break;

                    case SliderEventType.Repeat:

                        hitObjects.AddRange(generateExplosion(
                            e.Time,
                            Math.Clamp((int)curve.Distance / 15, 3, 15),
                            sliderEventPosition,
                            comboData,
                            isKiai,
                            index,
                            MathExtensions.GetRandomTimedAngleOffset(e.Time)));

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = e.Time,
                            Samples = obj.Samples,
                            Position = sliderEventPosition
                        });
                        break;

                    case SliderEventType.Tail:

                        hitObjects.AddRange(generateExplosion(
                            e.Time,
                            Math.Clamp((int)curve.Distance * (curve.RepeatCount + 1) / 15, 5, 20),
                            sliderEventPosition,
                            comboData,
                            isKiai,
                            index,
                            MathExtensions.GetRandomTimedAngleOffset(e.Time)));

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = curve.EndTime,
                            Samples = obj.Samples,
                            Position = sliderEventPosition
                        });
                        break;
                }
            }

            hitObjects.AddRange(generateSliderBody(obj, curve, isKiai, index));

            return hitObjects;
        }

        private static List<BosuHitObject> generateRepeatSpamSlider(HitObject obj, IBeatmap beatmap, IHasCurve curve, double spanDuration, bool isKiai, int index)
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

                        hitObjects.AddRange(generateExplosion(
                            e.Time,
                            bullets_per_slider_reverse,
                            sliderEventPosition,
                            comboData,
                            isKiai,
                            index));

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = obj.StartTime,
                            Samples = obj.Samples,
                            Position = sliderEventPosition
                        });

                        break;

                    case SliderEventType.Repeat:

                        hitObjects.AddRange(generateExplosion(
                            e.Time,
                            bullets_per_slider_reverse,
                            sliderEventPosition,
                            comboData,
                            isKiai,
                            index,
                            slider_angle_per_span * (e.SpanIndex + 1)));

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = e.Time,
                            Samples = obj.Samples,
                            Position = sliderEventPosition
                        });
                        break;

                    case SliderEventType.Tail:

                        hitObjects.AddRange(generateExplosion(
                            e.Time,
                            bullets_per_slider_reverse,
                            sliderEventPosition,
                            comboData,
                            isKiai,
                            index,
                            slider_angle_per_span * (curve.RepeatCount + 1)));

                        hitObjects.Add(new SoundHitObject
                        {
                            StartTime = curve.EndTime,
                            Samples = obj.Samples,
                            Position = sliderEventPosition
                        });
                        break;
                }
            }

            hitObjects.AddRange(generateSliderBody(obj, curve, isKiai, index));

            return hitObjects;
        }

        private static List<SliderPartCherry> generateSliderBody(HitObject obj, IHasCurve curve, bool isKiai, int index)
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
                        IndexInBeatmap = index,
                        IsKiai = isKiai,
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
    }
}
