using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Tests
{
    public partial class TestSceneCherryPiece : RulesetTestScene
    {
        private const int count = 40;

        private float red;
        private float green;
        private float blue;

        public TestSceneCherryPiece()
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Add(new CherryPiece
                    {
                        Origin = Anchor.Centre,
                        RelativePositionAxes = Axes.Both,
                        Position = new Vector2((float)i / count, (float)j / count) + new Vector2(1f / count) * 0.5f
                    });
                }
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddSliderStep("Red", 0f, 1f, 1f, r =>
            {
                red = r;
                updateColour();
            });
            AddSliderStep("Green", 0f, 1f, 1f, g =>
            {
                green = g;
                updateColour();
            });
            AddSliderStep("Blue", 0f, 1f, 1f, b =>
            {
                blue = b;
                updateColour();
            });
            AddStep("Flash", () =>
            {
                foreach (var c in this.ChildrenOfType<CherryPiece>())
                {
                    c.Flash(200);
                }
            });
        }

        private void updateColour()
        {
            foreach (var c in this.ChildrenOfType<CherryPiece>())
            {
                c.CherryColour = new Color4(red, green, blue, 1);
            }
        }
    }
}
