using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using System;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class Tile : CompositeDrawable
    {
        public const int SIZE = 16;

        [Resolved]
        private TextureStore textures { get; set; }

        private readonly TileType type;

        public Tile(TileType type)
        {
            this.type = type;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Texture = getTexture()
            });
        }

        private Texture getTexture()
        {
            switch (type)
            {
                case TileType.PlatformCorner:
                    return textures.Get("Playfield/platform-corner");

                case TileType.PlatformMiddle:
                    return textures.Get("Playfield/platform-middle");

                case TileType.PlatformMiddleRotated:
                    return textures.Get("Playfield/platform-middle-rotated");
            }

            throw new NotImplementedException($"{type} tile is not implemented");
        }
    }

    public enum TileType
    {
        PlatformCorner,
        PlatformMiddle,
        PlatformMiddleRotated
    }

    public enum TileDirection
    {
        Vertical,
        Horizontal
    }
}
