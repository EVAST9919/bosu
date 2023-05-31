using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.UI
{
    public partial class BosuPlayfieldBorder : Container
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Size = BosuPlayfield.BASE_SIZE; // 24x19
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            for (int i = 1; i <= 22; i++)
            {
                // top
                Add(new Pipe
                {
                    X = i * IWannaExtensions.TILE_SIZE
                });

                // bottom
                Add(new Pipe
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = i * IWannaExtensions.TILE_SIZE
                });
            }

            for (int i = 1; i <= 17; i++)
            {
                // left
                Add(new Pipe
                {
                    Rotation = 90,
                    X = IWannaExtensions.TILE_SIZE,
                    Y = i * IWannaExtensions.TILE_SIZE,
                });

                // right
                Add(new Pipe
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Rotation = -90,
                    X = -IWannaExtensions.TILE_SIZE,
                    Y = i * IWannaExtensions.TILE_SIZE,
                });
            }

            Add(new Corner());
            Add(new Corner() { Anchor = Anchor.TopRight, Origin = Anchor.TopRight });
            Add(new Corner() { Anchor = Anchor.BottomLeft, Origin = Anchor.BottomLeft });
            Add(new Corner() { Anchor = Anchor.BottomRight, Origin = Anchor.BottomRight });
        }

        private partial class BorderPart : Sprite
        {
            private readonly string textureName;

            public BorderPart(string textureName)
            {
                this.textureName = textureName;
                Size = new Vector2(IWannaExtensions.TILE_SIZE);
                EdgeSmoothness = Vector2.One;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                Texture = textures.Get($"Playfield/{textureName}", WrapMode.Repeat, WrapMode.Repeat);
            }
        }

        private partial class Corner : BorderPart
        {
            public Corner()
                : base("corner")
            {
            }
        }

        private partial class Pipe : BorderPart
        {
            public Pipe()
                : base("pipe")
            {
            }
        }
    }
}
