using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Bosu.Tests
{
    public abstract partial class RulesetTestScene : OsuTestScene
    {
        protected override Ruleset CreateRuleset() => new BosuRuleset();
    }
}
