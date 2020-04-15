using osu.Framework.Graphics.Containers;
using osuTK;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class Bullet : CompositeDrawable
    {
        private readonly Sprite sprite;

        private readonly bool right;
        private readonly double spawnTime;

        public Bullet(bool right, double spawnTime)
        {
            this.right = right;
            this.spawnTime = spawnTime;

            Size = new Vector2(3);
            Origin = Anchor.Centre;
            InternalChild = sprite = new Sprite
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.Both
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = textures.Get("bullet");
        }

        protected override void Update()
        {
            base.Update();

            if (Clock.CurrentTime < spawnTime)
            {
                this.FadeOut();
                return;
            }

            this.FadeIn();

            if (Position.X > BosuPlayfield.BASE_SIZE.X || Position.X < 0)
            {
                Expire();
                return;
            }

            var delta = (right ? 1 : -1) * (float)Clock.ElapsedFrameTime / 2;

            X += delta;
        }
    }
}
