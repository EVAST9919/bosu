using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public class CherryPiece : CompositeDrawable
    {
        private Color4 colour;

        public new Color4 Colour
        {
            get => colour;
            set
            {
                colour = value;
                Sprite.Colour = value;
            }
        }

        public readonly Sprite Sprite;
        private readonly Sprite overlay;
        private readonly Sprite branch;

        public CherryPiece()
        {
            AddRangeInternal(new[]
            {
                Sprite = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                },
                overlay = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                },
                branch = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Position = new Vector2(1, -1)
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Sprite.Texture = textures.Get("cherry");
            overlay.Texture = textures.Get("cherry-overlay");
            branch.Texture = textures.Get("cherry-branch");
        }
    }
}
