using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Bosu.Maps;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class BosuPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(384, 304);
        public static readonly int TILES_WIDTH = 24;
        public static readonly int TILES_HEIGHT = 19;

        public bool Zoom { get; set; }
        public double ZoomLevel;

        internal readonly BosuPlayer Player;

        private readonly BosuHealthProcessor healthProcessor;
        private readonly DeathOverlay deathOverlay;
        private readonly PlayerTrailController playerTrailController;
        private readonly EnteringOverlay enteringOverlay;

        public BosuPlayfield(BosuHealthProcessor healthProcessor)
        {
            this.healthProcessor = healthProcessor;

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                new PlayfieldBackground(),
                new DrawableMap(new EmptyMap()),
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
                deathOverlay = new DeathOverlay(Player),
                enteringOverlay = new EnteringOverlay()
            };

            playerTrailController.Player = Player;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            enteringOverlay.Enter(250);

            if (Zoom)
                Scale = new Vector2((float)ZoomLevel);
        }

        private bool failInvoked;

        protected override void Update()
        {
            base.Update();

            if (Zoom)
                zoomMod();

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

        private void zoomMod()
        {
            var playerPosition = Player.PlayerPosition();

            Position = new Vector2(-(playerPosition.X - BASE_SIZE.X / 2f), -(playerPosition.Y - BASE_SIZE.Y / 2f) + 50) * Scale;
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
