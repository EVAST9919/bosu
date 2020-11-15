using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Extensions;
using System.Threading;

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
        private int objectIndexInCurrentCombo = 0;

        protected override IEnumerable<BosuHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            var comboData = obj as IHasCombo;
            if (comboData?.NewCombo ?? false)
            {
                objectIndexInCurrentCombo = 0;
                index++;
            }

            bool kiai = beatmap.ControlPointInfo.EffectPointAt(obj.StartTime).KiaiMode;

            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            switch (obj)
            {
                case IHasPathWithRepeats curve:
                    hitObjects.AddRange(CherriesExtensions.ConvertSlider(obj, beatmap, curve, kiai, index));
                    break;

                case IHasDuration endTime:
                    hitObjects.AddRange(CherriesExtensions.ConvertSpinner(obj, endTime, beatmap.ControlPointInfo.TimingPointAt(obj.StartTime).BeatLength, kiai, index));
                    break;

                default:
                    hitObjects.AddRange(CherriesExtensions.ConvertHitCircle(obj, kiai, index, objectIndexInCurrentCombo));
                    break;
            }

            objectIndexInCurrentCombo++;

            return hitObjects;
        }

        protected override Beatmap<BosuHitObject> CreateBeatmap() => new BosuBeatmap();
    }
}
