using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class BosuPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(512, 384);

        internal readonly BosuPlayer Player;

        private readonly BosuHealthProcessor healthProcessor;
        private readonly Sprite failSprite;

        public BosuPlayfield(BosuHealthProcessor healthProcessor)
        {
            this.healthProcessor = healthProcessor;

            InternalChildren = new Drawable[]
            {
                new BosuBackground(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        HitObjectContainer,
                        Player = new BosuPlayer()
                    }
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
        private void load(TextureStore textures)
        {
            failSprite.Texture = textures.Get("game-over");
        }

        private bool failInvoked;

        protected override void Update()
        {
            base.Update();

            if (!healthProcessor.HasFailed)
                return;

            if (failInvoked)
                return;

            onFail();
            failInvoked = true;
        }

        private void onFail()
        {
            failSprite.FadeIn();
        }

        public override void Add(DrawableHitObject h)
        {
            if (h is DrawableMovingCherry drawable)
            {
                drawable.GetPlayerToTrace(Player);
                base.Add(drawable);
                return;
            }

            base.Add(h);
        }
    }
}
