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

        public bool CustomMap { get; set; }

        internal BosuPlayer Player;

        private readonly BosuHealthProcessor healthProcessor;
        private readonly DeathOverlay deathOverlay;
        private readonly PlayerTrailController playerTrailController;
        private readonly EnteringOverlay enteringOverlay;
        private readonly Container drawableMapContainer;

        public BosuPlayfield(BosuHealthProcessor healthProcessor)
        {
            this.healthProcessor = healthProcessor;

            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                new PlayfieldBackground(),
                drawableMapContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = HitObjectContainer
                },
                new PlayfieldBorder(),
                playerTrailController = new PlayerTrailController(),
                Player = new BosuPlayer(),
                deathOverlay = new DeathOverlay(Player),
                enteringOverlay = new EnteringOverlay()
            });

            playerTrailController.Player = Player;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Map map = createMap();

            drawableMapContainer.Add(new DrawableMap(map));
            Player.Map = map;

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
            Player.Dead = true;
            failInvoked = true;
        }

        private void zoomMod()
        {
            var playerPosition = Player.PlayerPosition();

            Position = new Vector2(-(playerPosition.X - BASE_SIZE.X / 2f), -(playerPosition.Y - BASE_SIZE.Y / 2f) + (CustomMap ? 0 : 50)) * Scale;
        }

        private Map createMap()
        {
            if (CustomMap)
                return new BossMap();

            return new EmptyMap();
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
