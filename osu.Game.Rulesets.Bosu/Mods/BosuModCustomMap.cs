using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModCustomMap : Mod, IApplicableToDrawableRuleset<BosuHitObject>
    {
        public override string Name => "Custom map";

        public override string Acronym => "CM";

        public override double ScoreMultiplier => 1;

        public override ModType Type => ModType.Fun;

        public void ApplyToDrawableRuleset(DrawableRuleset<BosuHitObject> drawableRuleset)
        {
            var playfield = (BosuPlayfield)drawableRuleset.Playfield;
            playfield.CustomMap = true;
        }
    }
}
