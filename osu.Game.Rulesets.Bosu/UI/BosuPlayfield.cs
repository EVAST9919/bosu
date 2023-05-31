using osu.Game.Rulesets.UI;
using osuTK;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.UI.Player;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Bosu.UI.Death;
using osu.Game.Rulesets.Bosu.UI.Entering;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Mods;
using System.Linq;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Bosu.UI
{
    public partial class BosuPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(768, 608);

        protected virtual bool EditMode { get; } = false;

        [Resolved(canBeNull: true)]
        private BosuRulesetConfigManager config { get; set; }

        private readonly Bindable<bool> transparentBackground = new Bindable<bool>();

        public readonly BosuPlayer Player;
        private readonly PlayfieldBackground bg;
        private readonly EnteringOverlay enteringOverlay;
        private readonly DeathOverlay deathOverlay;

        public BosuPlayfield()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        bg = new PlayfieldBackground(),
                        HitObjectContainer,
                    }
                },
                new BosuPlayfieldBorder(),
                Player = new BosuPlayer(),
                enteringOverlay = new EnteringOverlay
                {
                    Alpha = EditMode ? 0 : 1
                },
                deathOverlay = new DeathOverlay()
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RegisterPool<AngeledCherry, DrawableAngeledCherry>(300, 1500);
            RegisterPool<InstantCherry, DrawableInstantCherry>(500, 1500);

            config?.BindWith(BosuRulesetSetting.TransparentBackground, transparentBackground);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (EditMode)
                bg.Alpha = 0;
            else
                transparentBackground.BindValueChanged(transparent => bg.Alpha = transparent.NewValue ? 0 : 1, true);

            if (!EditMode)
                enteringOverlay.Enter(250);
        }

        public void ApplyHealthProcessor(BosuHealthProcessor p)
        {
            p.Failed += onDeath;
        }

        private bool onDeath()
        {
            if (!Mods.OfType<IApplicableFailOverride>().All(m => m.PerformFail()))
                return false;

            deathOverlay.Show(Player.PlayerPosition, Player.PlayerSpeed);
            Player.Die();
            return true;
        }

        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            switch (drawableHitObject)
            {
                case DrawableAngeledCherry cherry:
                    cherry.CheckHit += checkHit;
                    cherry.DistanceToPlayer += getDistanceToPlayer;
                    break;
            }
        }

        private float getDistanceToPlayer(Vector2 cherryPosition) => Vector2.Distance(cherryPosition, Player.PlayerPosition);

        private bool checkHit(Vector2 cherryPosition)
        {
            var playerPosition = Player.PlayerPosition;
            var isHit = MathExtensions.Collided(
                IWannaExtensions.CHERRY_RADIUS,
                cherryPosition,
                new Vector2(playerPosition.X - IWannaExtensions.PLAYER_HALF_WIDTH, playerPosition.Y - IWannaExtensions.PLAYER_HALF_HEIGHT),
                IWannaExtensions.PLAYER_SIZE);

            if (isHit)
                Player.OnHit();

            return isHit;
        }

        protected override HitObjectLifetimeEntry CreateLifetimeEntry(HitObject hitObject) => new BosuHitObjectLifetimeEntry(hitObject);

        private class BosuHitObjectLifetimeEntry : HitObjectLifetimeEntry
        {
            public BosuHitObjectLifetimeEntry(HitObject hitObject)
                : base(hitObject)
            {
            }

            protected override double InitialLifetimeOffset
            {
                get
                {
                    if (HitObject is Cherry cherry)
                        return cherry.TimePreempt;

                    return base.InitialLifetimeOffset;
                }
            }
        }
    }
}
