using osu.Game.Beatmaps;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Extensions;

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

        protected override IEnumerable<BosuHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap)
        {
            var comboData = obj as IHasCombo;
            if (comboData?.NewCombo ?? false)
                index++;

            List<BosuHitObject> hitObjects = new List<BosuHitObject>();

            switch (obj)
            {
                case IHasCurve curve:
                    hitObjects.AddRange(CherriesExtensions.ConvertSlider(obj, beatmap, curve, index));
                    break;

                case IHasEndTime endTime:
                    hitObjects.AddRange(CherriesExtensions.ConvertSpinner(obj, endTime, index));
                    break;

                default:
                    hitObjects.AddRange(CherriesExtensions.ConvertHitCircle(obj, index));
                    break;
            }

            return hitObjects;
        }

        protected override Beatmap<BosuHitObject> CreateBeatmap() => new BosuBeatmap();
    }
}
