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
                    Position = new Vector2(-1),
                    Size = new Vector2(Tile.SIZE + 1)
                },
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE + 1),
                    Position = new Vector2(1, -1),
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                },
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE + 1),
                    Position = new Vector2(-1, 1),
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                },
                new Tile(TileType.PlatformCorner)
                {
                    Size = new Vector2(Tile.SIZE + 1),
                    Position = new Vector2(1),
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                },
                new Tile(TileType.PlatformMiddle)
                {
                    Size = new Vector2((BosuPlayfield.TILES_WIDTH - 2) * Tile.SIZE, Tile.SIZE + 1),
                    X = Tile.SIZE,
                    Y = -1
                },
                new Tile(TileType.PlatformMiddle)
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Size = new Vector2((BosuPlayfield.TILES_WIDTH - 2) * Tile.SIZE, Tile.SIZE + 1),
                    Y = 1,
                    X = Tile.SIZE
                },
                new Tile(TileType.PlatformMiddleRotated)
                {
                    Size = new Vector2(Tile.SIZE + 1, (BosuPlayfield.TILES_HEIGHT - 2) * Tile.SIZE),
                    X = -1,
                    Y = Tile.SIZE
                },
                new Tile(TileType.PlatformMiddleRotated)
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Size = new Vector2(Tile.SIZE + 1, (BosuPlayfield.TILES_HEIGHT - 2) * Tile.SIZE),
                    X = 1,
                    Y = Tile.SIZE
                },
            });
        }
    }
}
