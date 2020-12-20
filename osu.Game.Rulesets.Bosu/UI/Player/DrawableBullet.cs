using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI.Player
{
    public class DrawableBullet : CompositeDrawable
    {
        private readonly Sprite sprite;

        public DrawableBullet()
        {
            Origin = Anchor.Centre;
            Size = new Vector2(IWannaExtensions.BULLET_SIZE);
            InternalChild = sprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = textures.Get("bullet");
        }
    }
}
