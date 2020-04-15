using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using System;
using osuTK.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Framework.Bindables;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Bosu.Replays;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private const double base_speed = 1.0 / 2500;

        [Resolved]
        private TextureStore textures { get; set; }

        private SampleChannel jump;
        private SampleChannel doubleJump;
        private SampleChannel shoot;

        private readonly Bindable<PlayerModel> playerModel = new Bindable<PlayerModel>();

        private int horizontalDirection;
        private int availableJumpCount = 2;
        private float verticalSpeed;
        private bool midAir;

        public readonly Container Player;
        private readonly Sprite drawablePlayer;
        private readonly Container bulletsContainer;

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
                    Origin = Anchor.BottomCentre,
                    RelativePositionAxes = Axes.Both,
                    Position = new Vector2(0.5f, 1),
                    Size = new Vector2(15),
                    Child = drawablePlayer = new Sprite
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.Both
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(BosuRulesetConfigManager config, ISampleStore samples)
        {
            config.BindWith(BosuRulesetSetting.PlayerModel, playerModel);

            jump = samples.Get("jump");
            doubleJump = samples.Get("double-jump");
            shoot = samples.Get("shoot");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            playerModel.BindValueChanged(model =>
            {
                switch (model.NewValue)
                {
                    case PlayerModel.Boshy:
                        drawablePlayer.Texture = textures.Get("Player/boshy");
                        return;

                    case PlayerModel.Kid:
                        drawablePlayer.Texture = textures.Get("Player/kid");
                        return;

                    default:
                        drawablePlayer.Texture = textures.Get("Player/bosu");
                        return;
                }
            }, true);
        }

        public Vector2 PlayerPositionInPlayfieldSpace(Vector2? offset = null) => new Vector2(Player.Position.X * BosuPlayfield.BASE_SIZE.X + (offset?.X ?? 0), (Player.Position.Y * BosuPlayfield.BASE_SIZE.Y - drawablePlayer.DrawHeight / 2) - (offset?.Y ?? 0));

        public Vector2 PlayerDrawSize() => drawablePlayer.DrawSize;

        public void PlayMissAnimation()
        {
            drawablePlayer.FadeColour(Color4.Red).Then().FadeColour(Color4.White, 1000, Easing.OutQuint);
        }

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

        protected override void Update()
        {
            updateReplayState();

            base.Update();

            // Collided with the ground, reset jump logic
            if (Player.Y > 1 || Player.Y < 0)
            {
                availableJumpCount = 2;
                verticalSpeed = 0;
                midAir = false;
                Player.Y = 1;
            }

            if (midAir)
            {
                verticalSpeed -= (float)Clock.ElapsedFrameTime / 3.5f;
                Player.Y -= (float)(Clock.ElapsedFrameTime * verticalSpeed * 0.00001);
            }

            if (horizontalDirection != 0)
            {
                var position = Math.Clamp(Player.X + Math.Sign(horizontalDirection) * Clock.ElapsedFrameTime * base_speed, 0, 1);

                Player.Scale = new Vector2(Math.Abs(Scale.X) * (horizontalDirection > 0 ? 1 : -1), Player.Scale.Y);

                if (position == Player.X)
                    return;

                Player.X = (float)position;
            }
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
                    verticalSpeed = 100;
                    break;

                case 0:
                    doubleJump.Play();
                    verticalSpeed = 90;
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
            bulletsContainer.Add(new Bullet(Player.Scale.X > 0, Clock.CurrentTime)
            {
                Position = PlayerPositionInPlayfieldSpace(new Vector2(0, 1))
            });
        }

        private void updateReplayState()
        {
            var state = (GetContainingInputManager().CurrentState as RulesetInputManagerInputState<BosuAction>)?.LastReplayState as BosuFramedReplayInputHandler.BosuReplayState ?? null;

            if (state != null)
            {
                Player.X = state.Position.Value / BosuPlayfield.BASE_SIZE.X;
            }
        }
    }
}
