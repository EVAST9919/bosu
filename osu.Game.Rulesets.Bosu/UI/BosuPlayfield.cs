using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Death;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class BosuPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(768, 608);
        public static readonly int TILES_WIDTH = 24;
        public static readonly int TILES_HEIGHT = 19;

        public bool Zoom { get; set; }
        public double ZoomLevel;

        internal BosuPlayer Player;

        private readonly BosuHealthProcessor healthProcessor;
        private readonly DeathOverlay deathOverlay;
        private readonly PlayerTrailController playerTrailController;
        private readonly EnteringOverlay enteringOverlay;

        public BosuPlayfield(BosuHealthProcessor healthProcessor)
        {
            this.healthProcessor = healthProcessor;

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                new PlayfieldBackground(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = HitObjectContainer
                },
                new PlayfieldBorder(),
                playerTrailController = new PlayerTrailController(),
                Player = new BosuPlayer(),
                deathOverlay = new DeathOverlay(),
                enteringOverlay = new EnteringOverlay()
            });

            playerTrailController.Player = Player;

            Player.OnDeath += deathOverlay.Play;
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

            Player.ForceDeath();
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
