using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Maps
{
    public class DrawableMap : CompositeDrawable
    {
        private readonly Map map;

        public DrawableMap(Map map)
        {
            this.map = map;

            Size = BosuPlayfield.BASE_SIZE;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            for (int i = 0; i < BosuPlayfield.TILES_WIDTH; i++)
            {
                for (int j = 0; j < BosuPlayfield.TILES_HEIGHT; j++)
                {
                    var tile = map.GetTileAt(i, j);

                    if (tile != ' ')
                    {
                        AddInternal(new Tile(getTileType(tile))
                        {
                            Position = new Vector2(i * Tile.SIZE, j * Tile.SIZE)
                        });
                    }                        
                }
            }
        }

        private TileType getTileType(char input)
        {
            switch (input)
            {
                case '+':
                    return TileType.PlatformCorner;

                case 'X':
                    return TileType.PlatformMiddle;

                case '-':
                    return TileType.PlatformMiddleRotated;

                default:
                    throw new NotImplementedException($"char {input} is not supported");
            }
        }
    }
}
