using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield;
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
        private readonly DeathOverlay deathOverlay;
        private readonly PlayerTrailController playerTrailController;

        public BosuPlayfield(BosuHealthProcessor healthProcessor)
        {
            this.healthProcessor = healthProcessor;

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                new PlayfieldBackground(),
                new PlayfieldBorder(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        HitObjectContainer,
                        playerTrailController = new PlayerTrailController(),
                        Player = new BosuPlayer()
                    }
                },
                deathOverlay = new DeathOverlay(Player)
            };

            playerTrailController.Player = Player;
        }

        private bool failInvoked;

        protected override void Update()
        {
            base.Update();

            trackHealth();
        }

        private void trackHealth()
        {
            if (!healthProcessor.HasFailed)
                return;

            if (failInvoked)
                return;

            deathOverlay.OnDeath();
            failInvoked = true;
        }

        public override void Add(DrawableHitObject h)
        {
            if (h is DrawableCherry drawable)
            {
                drawable.GetPlayerToTrace(Player);
                base.Add(drawable);
                return;
            }

            base.Add(h);
        }
    }
}
