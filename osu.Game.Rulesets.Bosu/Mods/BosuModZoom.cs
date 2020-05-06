using osu.Framework.Bindables;
using osu.Game.Configuration;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModZoom : Mod, IApplicableToDrawableRuleset<BosuHitObject>
    {
        public override string Name => "Zoom";

        public override string Acronym => "ZM";

        public override double ScoreMultiplier => 1;

        public override ModType Type => ModType.Fun;

        public void ApplyToDrawableRuleset(DrawableRuleset<BosuHitObject> drawableRuleset)
        {
            var playfield = (BosuPlayfield)drawableRuleset.Playfield;
            playfield.Zoom = true;
            playfield.ZoomLevel = ZoomValue.Value;
        }

        [SettingSource("Zoom value", "Zoom value")]
        public BindableNumber<double> ZoomValue { get; } = new BindableDouble
        {
            MinValue = 1.2,
            MaxValue = 2,
            Default = 1.5,
            Value = 1.5,
            Precision = 0.01,
        };
    }
}
