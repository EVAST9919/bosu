using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Utils;
using System;
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.UI.Death
{
    public class DeathParticle : CompositeDrawable
    {
        private const int duration = 1500;
        private const double gravity = 0.3;
        private const double max_vertical_speed = 9;

        private Vector2 speedVector;
        private double verticalSpeed = 0;

        public DeathParticle(Vector2 position, Vector2 speedVector)
        {
            this.speedVector = speedVector;

            Origin = Anchor.Centre;
            Position = position;
            Size = new Vector2(IWannaExtensions.TILE_SIZE);
        }

        private Sprite glow;
        private Sprite circle;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AddInternal(glow = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Texture = textures.Get("death-particle")
            });
            AddInternal(circle = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Texture = textures.Get("death-particle-white")
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            glow.FadeOut(duration, Easing.Out);
            glow.ResizeTo(0.7f, duration, Easing.Out);
            circle.ResizeTo(0, RNG.Next(duration - 350, duration - 300), Easing.Out);

            verticalSpeed = speedVector.Y;
        }

        protected override void Update()
        {
            base.Update();

            var elapsedFrameTime = Clock.ElapsedFrameTime;

            if (Math.Abs(verticalSpeed) > max_vertical_speed)
                verticalSpeed = Math.Sign(verticalSpeed) * max_vertical_speed;

            if (Precision.AlmostEquals(verticalSpeed, 0, 0.0001))
                verticalSpeed = 0;

            var adjustedVerticalDistance = verticalSpeed * (elapsedFrameTime / 20);

            Y -= (float)adjustedVerticalDistance;
            X += (float)(speedVector.X * (elapsedFrameTime / 20));

            verticalSpeed -= gravity * (elapsedFrameTime / 20);
        }
    }
}
