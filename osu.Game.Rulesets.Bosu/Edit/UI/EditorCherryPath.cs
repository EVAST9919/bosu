using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Edit.UI
{
    public partial class EditorCherryPath : Box
    {
        public EditorCherryPath(Vector2 startPosition, Vector2 endPosition)
        {
            Origin = Anchor.CentreLeft;
            Position = startPosition;
            Height = 1;
            Width = Vector2.Distance(startPosition, endPosition);
            Rotation = getRotation(startPosition, endPosition);
            Colour = Color4.Black;
            EdgeSmoothness = Vector2.One;
        }

        private static float getRotation(Vector2 startPosition, Vector2 endPosition)
        {
            float xDiff = endPosition.X - startPosition.X;
            float yDiff = endPosition.Y - startPosition.Y;
            return (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
        }
    }
}
