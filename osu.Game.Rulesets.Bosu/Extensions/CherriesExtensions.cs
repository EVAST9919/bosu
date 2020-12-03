using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;
using System;
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class CherriesExtensions
    {
        public static readonly int TILE_SIZE = 32;
        private static readonly Vector2 osu_playfield_size = new Vector2(512, 384);

        private const int bullets_per_hitcircle = 10;
        private const int hitcircle_angle_offset = 5;

        private const int bullets_per_slider_reverse = 5;

        private const float slider_angle_per_span = 2f;
        private const int max_visuals_per_slider_span = 100;

        private const int bullets_per_spinner_span = 20;

        public static List<BosuHitObject> ConvertSlider(HitObject obj, IBeatmap beatmap, IHasPathWithRepeats curve, bool isKiai, int index)
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

            if (indexInCurrentCombo == 0)
                hitObjects.AddRange(convertImpactCircle(obj, isKiai, index));
            else
                hitObjects.AddRange(convertDefaultCircle(obj, isKiai, index, indexInCurrentCombo));

            return hitObjects;
        }

        private static List<BosuHitObject> convertDefaultCircle(HitObject obj, bool isKiai, int index, int indexInCurrentCombo)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var circlePosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            circlePosition = toPlayfieldSpace(circlePosition) * new Vector2(1, 0.4f);
            var comboData = obj as IHasCombo;

            hitObjects.AddRange(generateExplosion(
                obj.StartTime,
                bullets_per_hitcircle,
                circlePosition,
                comboData,
                isKiai,
                index,
                hitcircle_angle_offset * indexInCurrentCombo));

            return hitObjects;
        }

        private static List<BosuHitObject> convertImpactCircle(HitObject obj, bool isKiai, int index)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var circlePosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            circlePosition = toPlayfieldSpace(circlePosition) * new Vector2(1, 0.4f);
            var comboData = obj as IHasCombo;

            var randomBool = MathExtensions.GetRandomTimedBool(obj.StartTime);

            hitObjects.AddRange(generatePolygonExplosion(
                obj.StartTime,
                5,
                randomBool ? 3 : 4,
                circlePosition,
                comboData,
                isKiai,
                index,
                MathExtensions.GetRandomTimedAngleOffset(obj.StartTime)));

            return hitObjects;
        }

        public static List<BosuHitObject> ConvertSpinner(HitObject obj, IHasDuration endTime, double beatLength, bool isKiai, int index)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var comboData = obj as IHasCombo;

            // Fast bpm spinners are almost impossible to pass, nerf them.
            if (beatLength < 400)
            {
                while (beatLength < 400)
                    beatLength *= 2f;
            }

            var spansPerSpinner = endTime.Duration / beatLength;

            for (int j = 0; j < bullets_per_spinner_span; j++)
            {
                hitObjects.Add(new SpinnerCherry
                {
                    EndTime = endTime,
                    InitialAngle = (float)j / bullets_per_spinner_span * 360,
                    StartTime = obj.StartTime,
                    NewCombo = comboData?.NewCombo ?? false,
                    ComboOffset = comboData?.ComboOffset ?? 0,
                    IndexInBeatmap = index,
                    IsKiai = isKiai,
                });
            }

            for (int i = 0; i < spansPerSpinner; i++)
            {
                var spinnerProgress = i / spansPerSpinner;

                for (int j = 0; j < bullets_per_spinner_span; j++)
                {
                    hitObjects.Add(new SpinnerBurstCherry
                    {
                        Angle = (float)j / bullets_per_spinner_span * 360,
                        SpinnerDuration = endTime.Duration,
                        SpinnerProgress = spinnerProgress,
                        StartTime = obj.StartTime + spinnerProgress * endTime.Duration,
                        NewCombo = comboData?.NewCombo ?? false,
                        ComboOffset = comboData?.ComboOffset ?? 0,
                        IndexInBeatmap = index,
                        IsKiai = isKiai,
                    });
                }
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
                var missingAngle = c == 0 ? 0 : Math.Acos((MathExtensions.Pow((float)side) + MathExtensions.Pow((float)length) - MathExtensions.Pow(c)) / (2 * side * length)) * 180 / Math.PI;
                var currentAngle = 180 + (90 - partialAngle) - missingAngle;

                yield return new AngledCherry
                {
                    Angle = (float)currentAngle + additionalOffset,
                    DeltaMultiplier = length / side * 1.2f,
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

        private static List<BosuHitObject> generateDefaultSlider(HitObject obj, IBeatmap beatmap, IHasPathWithRepeats curve, double spanDuration, bool isKiai, int index)
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

            foreach (var e in SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset, new CancellationToken()))
            {
                var curvePosition = curve.CurvePositionAt(e.PathProgress / (curve.RepeatCount + 1)) + objPosition;
                var sliderEventPosition = toPlayfieldSpace(curvePosition) * new Vector2(1, 0.4f);

                switch (e.Type)
                {
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
                        break;
                }
            }

            hitObjects.AddRange(generateSliderBody(obj, curve, isKiai, index));

            return hitObjects;
        }

        private static List<BosuHitObject> generateRepeatSpamSlider(HitObject obj, IBeatmap beatmap, IHasPathWithRepeats curve, double spanDuration, bool isKiai, int index)
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

            foreach (var e in SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, legacyLastTickOffset, new CancellationToken()))
            {
                var sliderEventPosition = toPlayfieldSpace(objPosition) * new Vector2(1, 0.4f);

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
                        break;
                }
            }

            hitObjects.AddRange(generateSliderBody(obj, curve, isKiai, index));

            return hitObjects;
        }

        private static List<SliderPartCherry> generateSliderBody(HitObject obj, IHasPathWithRepeats curve, bool isKiai, int index)
        {
            var objPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;

            List<SliderPartCherry> hitObjects = new List<SliderPartCherry>();

            var bodyCherriesCount = Math.Min(curve.Distance * (curve.RepeatCount + 1) / 10, max_visuals_per_slider_span * (curve.RepeatCount + 1));

            for (int i = 0; i < bodyCherriesCount; i++)
            {
                var progress = (float)i / bodyCherriesCount;
                var position = curve.CurvePositionAt(progress) + objPosition;
                position = toPlayfieldSpace(position) * new Vector2(1, 0.4f);

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

        private static bool positionIsValid(Vector2 position)
        {
            if (position.X > BosuPlayfield.BASE_SIZE.X || position.X < 0 || position.Y < 0 || position.Y > BosuPlayfield.BASE_SIZE.Y)
                return false;

            return true;
        }

        private static Vector2 toPlayfieldSpace(Vector2 input)
        {
            var newX = input.X / osu_playfield_size.X * BosuPlayfield.BASE_SIZE.X;
            var newY = input.Y / osu_playfield_size.Y * BosuPlayfield.BASE_SIZE.Y;
            return new Vector2(newX, newY);
        }
    }
}
