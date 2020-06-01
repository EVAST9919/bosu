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
using System;
using osu.Framework.Utils;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private static Vector2 size = new Vector2(11, 21);

        // Legacy constants
        private const double max_horizontal_speed = 0.2; // 0.15 is legacy
        private const double vertical_stop_speed_multiplier = 0.45;
        private const double jump_speed = 8.5;
        private const double jump2_speed = 7;
        private const double gravity = 0.45; // 0.4 is legacy, but this one matches better for some reason
        private const double max_vertical_speed = 9;

        private readonly Bindable<PlayerState> state = new Bindable<PlayerState>(PlayerState.Idle);
        private readonly Bindable<bool> hitboxEnabed = new Bindable<bool>(false);

        [Resolved(canBeNull: true)]
        private BosuRulesetConfigManager config { get; set; }

        private SampleChannel jump;
        private SampleChannel doubleJump;
        private SampleChannel shoot;

        public override bool RemoveCompletedTransforms => false;

        public bool Dead { get; set; }

        private int horizontalDirection;
        private int availableJumpCount = 2;
        private double verticalSpeed;
        private bool midAir;

        public readonly Container Player;
        private readonly Container bulletsContainer;
        private readonly Container animationContainer;
        private readonly Container hitbox;

        public BosuPlayer()
        {
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
                    Size = size,
                    Children = new Drawable[]
                    {
                        animationContainer = new Container
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            X = -1.5f,
                            Size = new Vector2(Tile.SIZE)
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

            Player.Position = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y - size.Y / 2f - Tile.SIZE);

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

            if (Dead)
                return;

            var elapsedFrameTime = Clock.ElapsedFrameTime;

            // Limit vertical speed
            if (Math.Abs(verticalSpeed) > max_vertical_speed)
                verticalSpeed = Math.Sign(verticalSpeed) * max_vertical_speed;

            if (Precision.AlmostEquals(verticalSpeed, 0, 0.0001))
                verticalSpeed = 0;

            if (verticalSpeed < 0)
            {
                if (PlayerPosition().Y < 0 || PlayerPosition().Y + size.Y / 2f > BosuPlayfield.BASE_SIZE.Y - Tile.SIZE)
                {
                    Player.Y = BosuPlayfield.BASE_SIZE.Y - Tile.SIZE - size.Y / 2f;
                    resetJumpLogic();
                }
            }

            if (horizontalDirection != 0)
            {
                rightwards = horizontalDirection > 0;

                animationContainer.Scale = new Vector2(rightwards ? 1 : -1, 1);
                animationContainer.X = rightwards ? -1.5f : 1.5f;

                Player.X += (float)(elapsedFrameTime * max_horizontal_speed) * (rightwards ? 1 : -1);

                Player.X = Math.Clamp(Player.X, Tile.SIZE + size.X / 2f, BosuPlayfield.BASE_SIZE.X - Tile.SIZE - size.X / 2f);
            }

            if (midAir)
            {
                var legacyDistance = verticalSpeed;
                var adjustedDistance = legacyDistance * (elapsedFrameTime / 20);

                Player.Y -= (float)adjustedDistance;

                verticalSpeed -= gravity * (elapsedFrameTime / 20);
            }

            updatePlayerState();
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
                    verticalSpeed = jump_speed;
                    break;

                case 0:
                    doubleJump.Play();
                    verticalSpeed = jump2_speed;
                    break;
            }
        }

        private void onJumpReleased()
        {
            if (verticalSpeed < 0)
                return;

            verticalSpeed *= vertical_stop_speed_multiplier;
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
