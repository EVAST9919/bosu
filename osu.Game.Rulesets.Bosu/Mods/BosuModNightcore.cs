using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModNightcore : ModNightcore<BosuHitObject>
    {
        public override double ScoreMultiplier => 1.1f;
    }
}
