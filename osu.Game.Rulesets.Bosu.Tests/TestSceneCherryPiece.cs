using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Tests
{
    public partial class TestSceneCherryPiece : RulesetTestScene
    {
        private readonly CherryPiece piece;

        private float red;
        private float green;
        private float blue;

        public TestSceneCherryPiece()
        {
            Add(piece = new CherryPiece
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
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
        }

        private void updateColour()
        {
            piece.Colour = new Color4(red, green, blue, 1);
        }
    }
}
