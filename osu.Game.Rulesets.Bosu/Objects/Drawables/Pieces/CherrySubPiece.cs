using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public class CherrySubPiece : CompositeDrawable
    {
        public new Color4 Colour
        {
            get => cherryBase.Colour;
            set => cherryBase.Colour = value;
        }

        private readonly int index;
        private readonly Sprite cherryBase;
        private readonly Sprite overlay;
        private readonly Sprite flash;

        public CherrySubPiece(int index)
        {
            this.index = index;

            Size = new Vector2(IWannaExtensions.CHERRY_DIAMETER);
            InternalChild = new Container
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = IWannaExtensions.CHERRY_FULL_SIZE,
                Children = new Drawable[]
                {
                    cherryBase = new Sprite
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    overlay = new Sprite
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    flash = new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            cherryBase.Texture = textures.Get($"Cherry/cherry-base-{index}");
            overlay.Texture = textures.Get($"Cherry/cherry-overlay-{index}");
            flash.Texture = textures.Get($"Cherry/cherry-flash-{index}");
        }

        public void Flash(double duration) => flash.FadeInFromZero(10, Easing.OutQuint).Then().FadeOut(duration);
    }
}
