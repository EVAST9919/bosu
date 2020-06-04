using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Beatmaps.Timing;

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

        protected override IEnumerable<BosuHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap)
        {
            var comboData = obj as IHasCombo;
            if (comboData?.NewCombo ?? false)
            {
                objectIndexInCurrentCombo = 0;
                index++;
            }

            var beatmapStageIndex = getBeatmapStageIndex(beatmap, obj.StartTime);

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

        private static int getBeatmapStageIndex(IBeatmap beatmap, double time)
        {
            if (beatmap.Breaks.Count == 0)
                return 1;

            BreakPeriod latestBreak = null;

            beatmap.Breaks.ForEach(b =>
            {
                if (b.EndTime < time)
                    latestBreak = b;
            });

            if (latestBreak == null)
                return 1;

            return beatmap.Breaks.IndexOf(latestBreak) + 2;
        }
    }
}
