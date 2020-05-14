using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Game.Rulesets.Bosu.Maps;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private const double base_speed = 1.0 / 8; // 13.5 is almost 1:1 with original;

        private readonly Bindable<PlayerState> state = new Bindable<PlayerState>(PlayerState.Idle);
        private readonly Bindable<bool> hitboxEnabed = new Bindable<bool>(false);

        [Resolved(canBeNull: true)]
        private BosuRulesetConfigManager config { get; set; }

        private SampleChannel jump;
        private SampleChannel doubleJump;
        private SampleChannel shoot;

        public override bool RemoveCompletedTransforms => false;

        private int horizontalDirection;
        private int availableJumpCount = 2;
        private float verticalSpeed;
        private bool midAir;

        public readonly Container Player;
        private readonly Container bulletsContainer;
        private readonly Container animationContainer;
        private readonly Container hitbox;
        private readonly Map map;

        public BosuPlayer(Map map)
        {
            this.map = map;

            RelativeSizeAxes = Axes.Both;
            AddRangeInternal(new Drawable[]
            {
                bulletsContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                Player = new Container
                {
                    Origin = Anchor.Centre,
                    Size = new Vector2(5.5f, 10.5f),
                    Children = new Drawable[]
                    {
                        animationContainer = new Container
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            X = -1.5f,
                            Size = new Vector2(12.5f)
                        },
                        hitbox = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Red,
                                Alpha = 0.5f,
                            }
                        }
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            jump = samples.Get("jump");
            doubleJump = samples.Get("double-jump");
            shoot = samples.Get("shoot");

            config?.BindWith(BosuRulesetSetting.EnableHitboxes, hitboxEnabed);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Player.Position = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y - PlayerSize().Y / 2f - Tile.SIZE);

            state.BindValueChanged(onStateChanged, true);
            hitboxEnabed.BindValueChanged(enabled => hitbox.Alpha = enabled.NewValue ? 1 : 0, true);
        }

        public Vector2 PlayerPosition() => Player.Position;

        public Vector2 PlayerSize() => Player.Size;

        public void PlayMissAnimation() => Player.FlashColour(Color4.Red, 1000, Easing.OutQuint);

        public PlayerState GetCurrentState() => state.Value;

        public bool OnPressed(BosuAction action)
        {
            switch (action)
            {
                case BosuAction.MoveLeft:
                    horizontalDirection--;
                    return true;

                case BosuAction.MoveRight:
                    horizontalDirection++;
                    return true;

                case BosuAction.Jump:
                    onJumpPressed();
                    return true;

                case BosuAction.Shoot:
                    onShoot();
                    return true;
            }

            return false;
        }

        public void OnReleased(BosuAction action)
        {
            switch (action)
            {
                case BosuAction.MoveLeft:
                    horizontalDirection++;
                    return;

                case BosuAction.MoveRight:
                    horizontalDirection--;
                    return;

                case BosuAction.Jump:
                    onJumpReleased();
                    return;

                case BosuAction.Shoot:
                    return;
            }
        }

        private bool rightwards = true;

        public bool Rightwards() => rightwards;

        protected override void Update()
        {
            base.Update();

            updateReplayState();

            if (horizontalDirection != 0)
            {
                rightwards = horizontalDirection > 0;

                animationContainer.Scale = new Vector2(rightwards ? 1 : -1, 1);
                animationContainer.X = rightwards ? -1.5f : 1.5f;

                if (rightwards)
                    checkRightCollision();
                else
                    checkLeftCollision();
            }

            if (verticalSpeed < 0)
                checkBottomCollision();

            if (verticalSpeed > 0)
                checkTopCollision();

            if (midAir)
            {
                verticalSpeed -= (float)Clock.ElapsedFrameTime / 4;

                // Limit maximum falling speed
                if (verticalSpeed < -80)
                    verticalSpeed = -80;

                Player.Y -= (float)(Clock.ElapsedFrameTime * verticalSpeed * 0.0045);
            }

            updatePlayerState();
        }

        private void checkRightCollision()
        {
            var playerRightBorderPosition = (int)((Player.X + PlayerSize().X / 2 + 1) / Tile.SIZE);
            var playerTopBorderPosition = (int)((Player.Y - PlayerSize().Y / 2) / Tile.SIZE);
            var playerMiddleBorderPosition = (int)((Player.Y + PlayerSize().Y / 2 - 1) / Tile.SIZE);
            var playerBottomBorderPosition = (int)((Player.Y + PlayerSize().Y / 2 + 1) / Tile.SIZE);

            var topTile = map.GetTileAt(playerRightBorderPosition, playerTopBorderPosition);
            var middleTile = map.GetTileAt(playerRightBorderPosition, playerMiddleBorderPosition);
            var bottomTile = map.GetTileAt(playerRightBorderPosition, playerBottomBorderPosition);

            var bottomIsSolid = bottomTile != ' ';

            if (topTile != ' ' || middleTile != ' ')
            {
                Player.X = playerRightBorderPosition * Tile.SIZE - PlayerSize().X / 2;
            }
            else
            {
                Player.X += (float)(Clock.ElapsedFrameTime * base_speed);

                if (!bottomIsSolid)
                    midAir = true;
            }
        }

        private void checkLeftCollision()
        {
            var playerLeftBorderPosition = (int)((Player.X - PlayerSize().X / 2 - 1) / Tile.SIZE);
            var playerTopBorderPosition = (int)((Player.Y - PlayerSize().Y / 2) / Tile.SIZE);
            var playerMiddleBorderPosition = (int)((Player.Y + PlayerSize().Y / 2 - 1) / Tile.SIZE);
            var playerBottomBorderPosition = (int)((Player.Y + PlayerSize().Y / 2 + 1) / Tile.SIZE);

            var topTile = map.GetTileAt(playerLeftBorderPosition, playerTopBorderPosition);
            var middleTile = map.GetTileAt(playerLeftBorderPosition, playerMiddleBorderPosition);
            var bottomTile = map.GetTileAt(playerLeftBorderPosition, playerBottomBorderPosition);

            var bottomIsSolid = bottomTile != ' ';

            if (topTile != ' ' || middleTile != ' ')
            {
                Player.X = (playerLeftBorderPosition + 1) * Tile.SIZE + PlayerSize().X / 2;
            }
            else
            {
                Player.X -= (float)(Clock.ElapsedFrameTime * base_speed);

                if (!bottomIsSolid)
                    midAir = true;
            }
        }

        private void checkTopCollision()
        {
            var playerTopBorderPosition = (int)((Player.Y - PlayerSize().Y / 2 - 1) / Tile.SIZE);
            var playerLeftBorderPosition = (int)((Player.X - PlayerSize().X / 2 + 1) / Tile.SIZE);
            var playerRightBorderPosition = (int)((Player.X + PlayerSize().X / 2 - 1) / Tile.SIZE);

            var leftTile = map.GetTileAt(playerLeftBorderPosition, playerTopBorderPosition);
            var rightTile = map.GetTileAt(playerRightBorderPosition, playerTopBorderPosition);

            if (leftTile != ' ' || rightTile != ' ')
            {
                Player.Y = (playerTopBorderPosition + 1) * Tile.SIZE + PlayerSize().Y / 2;
                verticalSpeed = 0;
            }
        }

        private void checkBottomCollision()
        {
            var playerBottomBorderPosition = (int)((Player.Y + PlayerSize().Y / 2 + 1) / Tile.SIZE);
            var playerLeftBorderPosition = (int)((Player.X - PlayerSize().X / 2 + 1) / Tile.SIZE);
            var playerRightBorderPosition = (int)((Player.X + PlayerSize().X / 2 - 1) / Tile.SIZE);

            var leftTile = map.GetTileAt(playerLeftBorderPosition, playerBottomBorderPosition);
            var rightTile = map.GetTileAt(playerRightBorderPosition, playerBottomBorderPosition);

            if (leftTile != ' ' || rightTile != ' ')
            {
                resetJumpLogic();
                Player.Y = playerBottomBorderPosition * Tile.SIZE - PlayerSize().Y / 2;
            }
        }

        private void resetJumpLogic()
        {
            availableJumpCount = 2;
            verticalSpeed = 0;
            midAir = false;
        }

        private void onJumpPressed()
        {
            if (availableJumpCount == 0)
                return;

            midAir = true;

            availableJumpCount--;

            switch (availableJumpCount)
            {
                case 1:
                    jump.Play();
                    verticalSpeed = 70;
                    break;

                case 0:
                    doubleJump.Play();
                    verticalSpeed = 60;
                    break;
            }
        }

        private void onJumpReleased()
        {
            if (verticalSpeed < 0)
                return;

            verticalSpeed /= 2;
        }

        private void onShoot()
        {
            shoot.Play();
            bulletsContainer.Add(new Bullet(Rightwards(), Clock.CurrentTime)
            {
                Position = PlayerPosition()
            });
        }

        private void updateReplayState()
        {
            var state = (GetContainingInputManager().CurrentState as RulesetInputManagerInputState<BosuAction>)?.LastReplayState as BosuFramedReplayInputHandler.BosuReplayState ?? null;

            if (state != null)
            {
                Player.X = state.Position.Value;
            }
        }

        private void onStateChanged(ValueChangedEvent<PlayerState> s)
        {
            animationContainer.Child = new PlayerAnimation(s.NewValue);
        }

        private void updatePlayerState()
        {
            if (verticalSpeed < 0)
            {
                state.Value = PlayerState.Fall;
                return;
            }

            if (verticalSpeed > 0)
            {
                state.Value = PlayerState.Jump;
                return;
            }

            if (horizontalDirection != 0)
            {
                state.Value = PlayerState.Run;
                return;
            }

            state.Value = PlayerState.Idle;
        }
    }
}
