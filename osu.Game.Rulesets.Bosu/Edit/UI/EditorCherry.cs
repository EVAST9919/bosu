using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics;
using osuTK.Graphics;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Edit.UI
{
    public class EditorCherry : Circle
    {
        public EditorCherry()
        {
            Origin = Anchor.Centre;
            Size = new Vector2(IWannaExtensions.CHERRY_DIAMETER);
            BorderThickness = 4;
            BorderColour = Color4.Black;
        }
    }
}
