using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModZoomIn : Mod, IUpdatableByPlayfield, IApplicableToDrawableRuleset<BosuHitObject>
    {
        public override string Name => "Zoom In";

        public override string Acronym => "ZM";

        public override LocalisableString Description => "Camera is too close.";

        public override double ScoreMultiplier => 1.0;

        [SettingSource("Zoom level")]
        public BindableNumber<float> ZoomLevel { get; } = new BindableFloat(1.5f)
        {
            MinValue = 1.1f,
            MaxValue = 2.0f,
            Precision = 0.1f,
        };

        public override string SettingDescription => $"Zoom: {ZoomLevel}";

        public void ApplyToDrawableRuleset(DrawableRuleset<BosuHitObject> drawableRuleset)
        {
            var bp = (BosuPlayfield)((DrawableBosuRuleset)drawableRuleset).Playfield;
            bp.Scale = new Vector2(ZoomLevel.Value);
        }

        public void Update(Playfield playfield)
        {
            var bp = (BosuPlayfield)playfield;

            bp.Position = (new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y / 2f) - bp.Player.PlayerPosition) * new Vector2(ZoomLevel.Value);
        }
    }
}
