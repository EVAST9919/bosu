﻿using osu.Framework.Utils;
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
        private const int max_visuals_per_slider_span = 200;

        private const int bullets_per_spinner_span = 20;
        private const float spinner_rotation_duration = 2000;
        private const float spinner_ring_distance = 20;

        public static IEnumerable<BosuHitObject> ConvertDefaultCircle(double startTime, Vector2 originalPosition, int indexInCurrentCombo)
            => generateExplosion(
                startTime,
                bullets_per_hitcircle,
                toPlayfieldSpace(originalPosition * new Vector2(1, 0.4f)),
                hitcircle_angle_offset * indexInCurrentCombo);

        public static IEnumerable<BosuHitObject> ConvertImpactCircle(double startTime, Vector2 originalPosition)
            => generatePolygonExplosion(
                startTime,
                4,
                MathExtensions.GetRandomTimedBool(startTime) ? 3 : 4,
                toPlayfieldSpace(originalPosition * new Vector2(1, 0.4f)),
                MathExtensions.GetRandomTimedAngleOffset(startTime));

        public static IEnumerable<BosuHitObject> ConvertDefaultSlider(HitObject obj, Vector2 originalPosition, IBeatmap beatmap, IHasPathWithRepeats curve, double spanDuration)
        {
            List<BosuHitObject> converted = new List<BosuHitObject>();

            var difficulty = beatmap.BeatmapInfo.Difficulty;

            var controlPointInfo = beatmap.ControlPointInfo;
            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(obj.StartTime);

            double scoringDistance = 100 * difficulty.SliderMultiplier;

            var velocity = scoringDistance / timingPoint.BeatLength;
            var tickDistance = scoringDistance / difficulty.SliderTickRate;

            foreach (var e in SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, new CancellationToken()))
            {
                var curvePosition = curve.CurvePositionAt(e.PathProgress / (curve.RepeatCount + 1)) + originalPosition;
                var sliderEventPosition = toPlayfieldSpace(curvePosition * new Vector2(1, 0.4f));

                if (!withinPlayfield(sliderEventPosition))
                    continue;

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

            var difficulty = beatmap.BeatmapInfo.Difficulty;

            var controlPointInfo = beatmap.ControlPointInfo;
            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(obj.StartTime);

            double scoringDistance = 100 * difficulty.SliderMultiplier;

            var velocity = scoringDistance / timingPoint.BeatLength;
            var tickDistance = scoringDistance / difficulty.SliderTickRate;

            var slider = SliderEventGenerator.Generate(obj.StartTime, spanDuration, velocity, tickDistance, curve.Path.Distance, curve.RepeatCount + 1, new CancellationToken());

            var buzzPosition = toPlayfieldSpace(originalPosition * new Vector2(1, 0.4f));
            var repeats = slider.Select(e => e.Type == SliderEventType.Repeat);

            var repeatCount = repeats.Count();

            var repeatsPerSecond = 1000f / (curve.Duration / repeatCount);

            if (repeatsPerSecond > 10)
                repeatsPerSecond = 10;

            var totalRepeats = (int)(repeatsPerSecond * curve.Duration / 1000f);
            var repeatDuration = curve.Duration / totalRepeats;

            foreach (var e in slider)
            {
                switch (e.Type)
                {
                    case SliderEventType.Head:
                        converted.AddRange(generateExplosion(e.Time, bullets_per_slider_reverse, buzzPosition));
                        break;

                    case SliderEventType.Tail:
                        converted.AddRange(generateExplosion(e.Time, bullets_per_slider_reverse, buzzPosition, slider_angle_per_span * (repeatCount + 1)));
                        break;
                }
            }

            for (int i = 0; i < totalRepeats; i++)
                converted.AddRange(generateExplosion(obj.StartTime + (i + 1) * repeatDuration, bullets_per_slider_reverse, buzzPosition, slider_angle_per_span * (i + 1)));

            return converted;
        }

        public static IEnumerable<BosuHitObject> GenerateSliderBody(double startTime, IHasPathWithRepeats curve, Vector2 originalPosition)
        {
            List<InstantCherry> bodyCherries = new List<InstantCherry>();

            var bodyCherriesCount = (int)(curve.Distance * (curve.RepeatCount + 1) / 15);

            Vector2 lastPosition = new Vector2(-10000, -10000);

            for (int i = 0; i < bodyCherriesCount; i++)
            {
                var progress = i / (float)bodyCherriesCount;
                Vector2 position = toPlayfieldSpace((curve.CurvePositionAt(progress) + originalPosition) * new Vector2(1, 0.4f));

                if (i != 0 && i != bodyCherriesCount - 1)
                {
                    if (Vector2.Distance(position, lastPosition) < 5)
                        continue;
                }

                lastPosition = position;

                if (withinPlayfield(position))
                {
                    bodyCherries.Add(new InstantCherry
                    {
                        StartTime = startTime + curve.Duration * progress,
                        Position = position
                    });
                }
            }

            if (!bodyCherries.Any())
            {
                bodyCherries.Add(new InstantCherry
                {
                    StartTime = startTime,
                    Position = Vector2.Zero
                });
            }

            return bodyCherries;
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

                    yield return new AngledCherry
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
                yield return new AngledCherry
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

                yield return new AngledCherry
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
            var newX = Interpolation.ValueAt(Math.Clamp(input.X / osu_playfield_size.X, 0f, 1f), IWannaExtensions.TILE_SIZE, BosuPlayfield.BASE_SIZE.X - IWannaExtensions.TILE_SIZE, 0f, 1f);
            var newY = Interpolation.ValueAt(Math.Clamp(input.Y / osu_playfield_size.Y, 0f, 1f), IWannaExtensions.TILE_SIZE, BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.TILE_SIZE, 0f, 1f);

            return new Vector2(newX, newY);
        }

        private static bool withinPlayfield(Vector2 position)
        {
            return position.X > IWannaExtensions.TILE_SIZE && position.X < BosuPlayfield.BASE_SIZE.X - IWannaExtensions.TILE_SIZE && position.Y > IWannaExtensions.TILE_SIZE && position.Y < BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.TILE_SIZE;
        }
    }
}
