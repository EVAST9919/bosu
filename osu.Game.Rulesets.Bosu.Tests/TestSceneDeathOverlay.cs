using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Bosu.UI.Death;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Tests
{
    public partial class TestSceneDeathOverlay : RulesetTestScene
    {
        private Container content;
        private DeathOverlay deathOverlay;

        public TestSceneDeathOverlay()
        {
            Add(content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = BosuPlayfield.BASE_SIZE
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddStep("Trigger death", () =>
            {
                content.Clear();
                content.Add(deathOverlay = new DeathOverlay());
                deathOverlay.Show(new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y / 2f), new Vector2(3, 0));
            });
        }
    }
}
