using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Bosu.Tests
{
    [TestFixture]
    public partial class TestSceneEditor : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new BosuRuleset();
    }
}
