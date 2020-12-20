using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Game.Tests.Visual;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Tests
{
    public class TestSceneCherryPiece : OsuTestScene
    {
        protected override Ruleset CreateRuleset() => new BosuRuleset();

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
