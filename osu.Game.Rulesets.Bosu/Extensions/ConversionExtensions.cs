﻿using osu.Game.Beatmaps;
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
    public static class ConversionExtensions
    {
        private static readonly Vector2 osu_playfield_size = new Vector2(512, 384);

        private const int bullets_per_hitcircle = 8;
        private const int hitcircle_angle_offset = 5;
        private const float slider_angle_per_span = 2f;
        private const int bullets_per_slider_reverse = 5;
        private const int max_visuals_per_slider_span = 100;

        private const int bullets_per_spinner_span = 20;
        private const float spinner_rotation_duration = 2000;
        private const float spinner_ring_distance = 20;

        public static IEnumerable<BosuHitObject> ConvertDefaultCircle(double startTime, Vector2 originalPosition, int indexInCurrentCombo)
            => generateExplosion(
                startTime,
                bullets_per_hitcircle,
                toPlayfieldSpace(originalPosition) * new Vector2(1, 0.4f),
                hitcircle_angle_offset * indexInCurrentCombo);

        public static IEnumerable<BosuHitObject> ConvertImpactCircle(double startTime, Vector2 originalPosition)
            => generatePolygonExplosion(
                startTime,
                4,
                MathExtensions.GetRandomTimedBool(startTime) ? 3 : 4,
                toPlayfieldSpace(originalPosition) * new Vector2(1, 0.4f),
                MathExtensions.GetRandomTimedAngleOffset(startTime));

        public static IEnumerable<BosuHitObject> ConvertDefaultSlider(HitObject obj, Vector2 originalPosition, IBeatmap beatmap, IHasPathWithRepeats curve, double spanDuration)
        {
            List<BosuHitObject> converted = new List<BosuHitObject>();

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
                var curvePosition = curve.CurvePositionAt(e.PathProgress / (curve.RepeatCount + 1)) + originalPosition;
                var sliderEventPosition = toPlayfieldSpace(curvePosition) * new Vector2(1, 0.4f);

                switch (e.Type)
                {
                    case SliderEventType.Repeat:
                        converted.AddRange(generateExplosion(e.Time, Math.Clamp((int)curve.Distance / 15, 3, 15), sliderEventPosition, MathExtensions.GetRandomTimedAngleOffset(e.Time)));
                        break;

                    case SliderEventType.Tail:
                        converted.AddRange(generateExplosion(e.Time, Math.Clamp((int)curve.Distance * (curve.RepeatCount + 1) / 15, 5, 20), sliderEventPosition, MathExtensions.GetRandomTimedAngleOffset(e.Time)));
                        break;
                }
            }

            return converted;
        }

        public static IEnumerable<BosuHitObject> ConvertBuzzSlider(HitObject obj, Vector2 originalPosition, IBeatmap beatmap, IHasPathWithRepeats curve, double spanDuration)
        {
            List<BosuHitObject> converted = new List<BosuHitObject>();

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
                var sliderEventPosition = toPlayfieldSpace(originalPosition) * new Vector2(1, 0.4f);

                switch (e.Type)
                {
                    case SliderEventType.Head:
                        converted.AddRange(generateExplosion(e.Time, bullets_per_slider_reverse, sliderEventPosition));
                        break;

                    case SliderEventType.Repeat:
                        converted.AddRange(generateExplosion(e.Time, bullets_per_slider_reverse, sliderEventPosition, slider_angle_per_span * (e.SpanIndex + 1)));
                        break;

                    case SliderEventType.Tail:
                        converted.AddRange(generateExplosion(e.Time, bullets_per_slider_reverse, sliderEventPosition, slider_angle_per_span * (curve.RepeatCount + 1)));
                        break;
                }
            }

            return converted;
        }

        public static IEnumerable<BosuHitObject> GenerateSliderBody(double startTime, IHasPathWithRepeats curve, Vector2 originalPosition)
        {
            var bodyCherriesCount = Math.Min(curve.Distance * (curve.RepeatCount + 1) / 10, max_visuals_per_slider_span * (curve.RepeatCount + 1));

            for (int i = 0; i < bodyCherriesCount; i++)
            {
                var progress = (float)i / bodyCherriesCount;
                var position = curve.CurvePositionAt(progress) + originalPosition;
                position = toPlayfieldSpace(position) * new Vector2(1, 0.4f);

                yield return new InstantCherry
                {
                    StartTime = startTime + curve.Duration * progress,
                    Position = position
                };
            }
        }

        public static IEnumerable<BosuHitObject> ConvertSpinner(double startTime, IHasDuration endTime, double beatLength)
        {
            // Fast bpm spinners are almost impossible to pass, nerf them.
            if (beatLength < 400)
            {
                while (beatLength < 400)
                    beatLength *= 2f;
            }

            var spansPerSpinner = endTime.Duration / beatLength;

            for (int i = 0; i < spansPerSpinner; i++)
            {
                var spinnerProgress = i / spansPerSpinner;

                for (int j = 0; j < bullets_per_spinner_span; j++)
                {
                    var rotationsPerSpinner = endTime.Duration / spinner_rotation_duration;
                    var angle = (float)(((float)j / bullets_per_spinner_span * 360) + (spinnerProgress * rotationsPerSpinner * 360));
                    var originPosition = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y / 4f);

                    var rotatedXPos = originPosition.X + (spinner_ring_distance * Math.Sin(angle * Math.PI / 180));
                    var rotatedYPos = originPosition.Y + (spinner_ring_distance * -Math.Cos(angle * Math.PI / 180));

                    yield return new AngeledCherry
                    {
                        Angle = angle,
                        StartTime = startTime + spinnerProgress * endTime.Duration,
                        Position = new Vector2((float)rotatedXPos, (float)rotatedYPos),
                    };
                }
            }
        }

        private static IEnumerable<BosuHitObject> generateExplosion(double startTime, int bulletCount, Vector2 position, float angleOffset = 0, float angleRange = 360f)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                yield return new AngeledCherry
                {
                    Angle = MathExtensions.BulletDistribution(bulletCount, angleRange, i, angleOffset),
                    StartTime = startTime,
                    Position = position,
                };
            }
        }

        private static IEnumerable<BosuHitObject> generatePolygonExplosion(double startTime, int bullets_per_side, int verticesCount, Vector2 position, float angleOffset = 0)
        {
            for (int i = 0; i < verticesCount; i++)
            {
                foreach (var h in generatePolygonLine(startTime, bullets_per_side, verticesCount, position, i * (360f / verticesCount) + angleOffset))
                    yield return h;
            }
        }

        private static IEnumerable<BosuHitObject> generatePolygonLine(double startTime, int bullets_per_side, int verticesCount, Vector2 position, float additionalOffset = 0)
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

                yield return new AngeledCherry
                {
                    Angle = (float)currentAngle + additionalOffset,
                    SpeedMultiplier = (float)(length / side * 1.2f),
                    StartTime = startTime,
                    Position = position
                };
            }
        }

        private static Vector2 toPlayfieldSpace(Vector2 input)
        {
            var newX = input.X / osu_playfield_size.X * BosuPlayfield.BASE_SIZE.X;
            var newY = input.Y / osu_playfield_size.Y * BosuPlayfield.BASE_SIZE.Y + IWannaExtensions.TILE_SIZE;
            return new Vector2(newX, newY);
        }
    }
}