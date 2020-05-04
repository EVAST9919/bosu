using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class DeathOverlay : CompositeDrawable
    {
        private const int particle_count = 30;

        private SampleChannel deathSound;

        private readonly BosuPlayer player;
        private readonly Container<DeathParticle> particlesContainer;
        private readonly Box tint;
        private readonly Sprite failSprite;

        public DeathOverlay(BosuPlayer player)
        {
            this.player = player;

            RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                particlesContainer = new Container<DeathParticle>
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                },
                tint = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    Colour = Color4.Red,
                },
                failSprite = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Width = 0.7f,
                    Alpha = 0,
                    FillMode = FillMode.Fit,
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, ISampleStore samples)
        {
            failSprite.Texture = textures.Get("game-over");
            deathSound = samples.Get("death");
        }

        public void OnDeath()
        {
            var position = player.PlayerPosition();

            for (int i = 0; i < particle_count; i++)
            {
                var particle = new DeathParticle
                {
                    Origin = Anchor.Centre,
                    Position = position
                };

                particlesContainer.Add(particle);

                particle.MoveToOffset(new Vector2(RNG.Next(-150, 150), RNG.Next(-150, 150)), 2000, Easing.Out);
                particle.FadeOut(2000, Easing.OutQuint);
            }

            player.FadeOut();
            tint.FadeTo(0.7f, 1000, Easing.OutQuint);
            failSprite.FadeTo(1, 1000, Easing.OutQuint);
            deathSound.Play();
        }
    }
}
