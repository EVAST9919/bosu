using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;
using osuTK;
using System;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Framework.Audio.Track;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Death
{
    public class DeathOverlay : CompositeDrawable
    {
        private Box tint;
        private Box blackFlash;
        private Sprite sprite;
        private LetterboxOverlay letterbox;
        private DrawableSample deathSample;
        private Container<DeathParticle> particles;
        private Sprite circle;

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples, TextureStore textures)
        {
            RelativeSizeAxes = Axes.Both;
            Masking = true;

            AddRangeInternal(new Drawable[]
            {
                particles = new Container<DeathParticle>
                {
                    RelativeSizeAxes = Axes.Both
                },
                circle = new Sprite
                {
                    Size = new Vector2(CherriesExtensions.TILE_SIZE * 5),
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
                    Texture = textures.Get("game-over"),
                    Alpha = 0,
                },
                letterbox = new LetterboxOverlay
                {
                    Alpha = 0,
                },
                deathSample = new DrawableSample(samples.Get("death")),
            });
        }

        public void Play((Vector2 position, Vector2 playerSpeed) value)
        {
            tint.FadeTo(0.6f, 260);
            blackFlash.FadeIn(0.8f).Then().FadeOut(180);
            sprite.Delay(200).FadeIn(600);
            letterbox.Delay(330).FadeIn(700);
            deathSample.Play();

            var rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 50; i++)
            {
                var speedVector = new Vector2(MathExtensions.Map((float)rand.NextDouble(), 0, 1, -7, 7) + value.playerSpeed.X, MathExtensions.Map((float)rand.NextDouble(), 0, 1, -5, 5));
                var particle = new DeathParticle(value.position, speedVector);
                particles.Add(particle);
            }

            circle.Position = value.position;
            circle.FadeIn().Delay(250).FadeOut(750, Easing.Out);
            circle.Colour = Color4.White;
            circle.FadeColour(Color4.Red, 1000, Easing.Out);
            circle.ScaleTo(1, 1000, Easing.Out);
        }
    }
}
