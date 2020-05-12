using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using System;
using osuTK.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private const double base_speed = 1.0 / 6.5;

        private readonly Bindable<PlayerState> state = new Bindable<PlayerState>(PlayerState.Idle);

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
                        //new Box
                        //{
                        //    RelativeSizeAxes = Axes.Both,
                        //    Colour = Color4.Red,
                        //    Alpha = 0.5f,
                        //}
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
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Player.Position = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y - PlayerSize().Y / 2f);

            state.BindValueChanged(onStateChanged, true);
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
            updateReplayState();

            base.Update();

            // Collided with the ground, reset jump logic
            if (Player.Y > (BosuPlayfield.BASE_SIZE.Y - PlayerSize().Y / 2f))
            {
                resetJumpLogic();
                Player.Y = BosuPlayfield.BASE_SIZE.Y - PlayerSize().Y / 2f;
            }

            // Collided with the ceiling
            if (Player.Y < PlayerSize().Y / 2)
                verticalSpeed = 0;

            if (midAir)
            {
                verticalSpeed -= (float)Clock.ElapsedFrameTime / 4;

                // Limit maximum falling speed
                if (verticalSpeed < -80)
                    verticalSpeed = -80;

                Player.Y -= (float)(Clock.ElapsedFrameTime * verticalSpeed * 0.0045);
            }

            if (horizontalDirection != 0)
            {
                var xPos = Math.Clamp(Player.X + Math.Sign(horizontalDirection) * Clock.ElapsedFrameTime * base_speed, PlayerSize().X / 2, BosuPlayfield.BASE_SIZE.X - PlayerSize().X / 2);

                rightwards = horizontalDirection > 0;

                animationContainer.Scale = new Vector2(rightwards ? 1 : -1, 1);
                animationContainer.X = rightwards ? -1.5f : 1.5f;

                Player.X = (float)xPos;
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
                    verticalSpeed = 80;
                    break;

                case 0:
                    doubleJump.Play();
                    verticalSpeed = 70;
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
