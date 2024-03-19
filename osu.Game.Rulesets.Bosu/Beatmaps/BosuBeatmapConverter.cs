using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using System.Threading;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;
using osu.Framework.Utils;
using System;

namespace osu.Game.Rulesets.Bosu.Beatmaps
{
    public class BosuBeatmapConverter : BeatmapConverter<BosuHitObject>
    {
        public BosuBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition);

        private int index = -1;

        protected override IEnumerable<BosuHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            var originalPosition = (obj as IHasPosition)?.Position ?? Vector2.Zero;
            var comboData = obj as IHasCombo;

            bool newCombo = comboData?.NewCombo ?? false;

            if (newCombo)
                index = 0;
            else
                index++;

            switch (obj)
            {
                case IHasPathWithRepeats curve:

                    double spanDuration = curve.Duration / (curve.RepeatCount + 1);
                    bool isBuzz = spanDuration < 75 && curve.RepeatCount > 0;

                    hitObjects.AddRange(ConversionExtensions.GenerateSliderBody(obj.StartTime, curve, originalPosition));

                    if (isBuzz)
                        hitObjects.AddRange(ConversionExtensions.ConvertBuzzSlider(obj, originalPosition, beatmap, curve, spanDuration));
                    else
                        hitObjects.AddRange(ConversionExtensions.ConvertDefaultSlider(obj, originalPosition, beatmap, curve, spanDuration));

                    break;

                case IHasDuration endTime:
                    hitObjects.AddRange(ConversionExtensions.ConvertSpinner(obj.StartTime, endTime, beatmap.ControlPointInfo.TimingPointAt(obj.StartTime).BeatLength));
                    break;

                default:

                    if (newCombo)
                        hitObjects.AddRange(ConversionExtensions.ConvertImpactCircle(obj.StartTime, originalPosition));
                    else
                        hitObjects.AddRange(ConversionExtensions.ConvertDefaultCircle(obj.StartTime, originalPosition, index));

                    break;
            }

            bool first = true;

            foreach (var h in hitObjects)
            {
                if (h is Cherry c)
                {
                    c.NewCombo = first && newCombo;
                    c.ComboOffset = comboData?.ComboOffset ?? 0;
                }

                if (h is AngledCherry m)
                {
                    var bpm = beatmap.ControlPointInfo.TimingPointAt(h.StartTime).BPM;
                    m.SpeedMultiplier *= Interpolation.ValueAt(Math.Clamp(bpm, 0, 300), 0.625f, 1.5f, 0, 300);
                }

                if (first)
                    first = false;
            }

            return hitObjects;
        }

        protected override Beatmap<BosuHitObject> CreateBeatmap() => new BosuBeatmap();
    }
}
