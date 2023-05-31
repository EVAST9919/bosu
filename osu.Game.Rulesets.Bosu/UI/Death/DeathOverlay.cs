using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Audio;
using System;
using osu.Framework.Utils;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Framework.Audio.Sample;

namespace osu.Game.Rulesets.Bosu.UI.Death
{
    public partial class DeathOverlay : CompositeDrawable
    {
        private Box tint;
        private Box blackFlash;
        private Sprite sprite;
        private LetterboxOverlay letterbox;
        private Container<DeathParticle> particles;
        private Sprite circle;
        private DrawableSample deathSample;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, ISampleStore samples)
        {
            RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                particles = new Container<DeathParticle>
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true
                },
                circle = new Sprite
                {
                    Size = new Vector2(IWannaExtensions.TILE_SIZE * 5),
                    Origin = Anchor.Centre,
                    Scale = Vector2.Zero,
                    Alpha = 0,
                    AlwaysPresent = true,
                    Texture = textures.Get("death-circle")
                },
                tint = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Red,
                    Alpha = 0
                },
                blackFlash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0
                },
                sprite = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.8f),
                    FillMode = FillMode.Fit,
                    Alpha = 0,
                },
                letterbox = new LetterboxOverlay
                {
                    Alpha = 0,
                },
                deathSample = new DrawableSample(samples.Get("death")),
            };

            sprite.Texture = textures.Get("game-over");
        }

        public void Show(Vector2 playerPosition, Vector2 playerSpeed)
        {
            tint.FadeTo(0.6f, 260);
            blackFlash.FadeIn(0.8f).Then().FadeOut(180);
            sprite.Delay(200).FadeIn(600);
            letterbox.Delay(330).FadeIn(700);
            deathSample.Play();

            var rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 50; i++)
            {
                var speedVector = new Vector2(Interpolation.ValueAt((float)rand.NextDouble(), -7f, 7f, 0, 1f) + playerSpeed.X, Interpolation.ValueAt((float)rand.NextDouble(), -5f, 5f, 0, 1f));
                var particle = new DeathParticle(playerPosition, speedVector);
                particles.Add(particle);
            }

            circle.Position = playerPosition;
            circle.FadeIn().Delay(250).FadeOut(750, Easing.Out);
            circle.Colour = Color4.White;
            circle.FadeColour(Color4.Red, 1000, Easing.Out);
            circle.ScaleTo(1, 1000, Easing.Out);
        }
    }
}
