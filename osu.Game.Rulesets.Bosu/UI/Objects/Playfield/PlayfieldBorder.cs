using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class PlayfieldBorder : CompositeDrawable
    {
        public PlayfieldBorder()
        {
            RelativeSizeAxes = Axes.Both;
            AddRangeInternal(new[]
            {
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE),
                    Origin = Anchor.BottomRight,
                },
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE),
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.BottomLeft,
                },
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE),
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.TopRight,
                },
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE),
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.TopLeft,
                },
                new Tile(TileType.PlatformMiddle)
                {
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    Height = Tile.SIZE
                },
                new Tile(TileType.PlatformMiddle)
                {
                    Anchor = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    Height = Tile.SIZE
                },
                new Tile(TileType.PlatformMiddleRotated)
                {
                    Origin = Anchor.TopRight,
                    RelativeSizeAxes = Axes.Y,
                    Width = Tile.SIZE
                },
                new Tile(TileType.PlatformMiddleRotated)
                {
                    Anchor = Anchor.TopRight,
                    RelativeSizeAxes = Axes.Y,
                    Width = Tile.SIZE
                },
            });
        }
    }
}
