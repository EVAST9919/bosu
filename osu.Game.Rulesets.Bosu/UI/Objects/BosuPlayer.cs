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

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private const double base_speed = 1.0 / 2500;

        [Resolved]
        private TextureStore textures { get; set; }

        private SampleChannel jump;
        private SampleChannel doubleJump;

        private readonly Bindable<PlayerModel> playerModel = new Bindable<PlayerModel>();

        private int horizontalDirection;
        private Action jumpPressed;
        private Action jumpReleased;
        private int availableJumpCount = 2;
        private float verticalSpeed;
        private bool midAir;

        private readonly Container player;
        private readonly Sprite drawablePlayer;

        public BosuPlayer()
        {
            RelativeSizeAxes = Axes.Both;
            AddInternal(player = new Container
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
            });
        }

        [BackgroundDependencyLoader]
        private void load(BosuRulesetConfigManager config, ISampleStore samples)
        {
            config.BindWith(BosuRulesetSetting.PlayerModel, playerModel);

            jump = samples.Get("jump");
            doubleJump = samples.Get("double-jump");
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

            jumpPressed += onJumpPressed;
            jumpReleased += onJumpReleased;
        }

        public Vector2 PlayerPositionInPlayfieldSpace() => player.Position * BosuPlayfield.BASE_SIZE;

        public Vector2 PlayerDrawSize() => player.DrawSize * 0.7f;

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
                    jumpPressed?.Invoke();
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
                    jumpReleased?.Invoke();
                    return;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Collided with the ground, reset jump logic
            if (player.Y > 1 || player.Y < 0)
            {
                availableJumpCount = 2;
                verticalSpeed = 0;
                midAir = false;
                player.Y = 1;
            }

            if (midAir)
            {
                verticalSpeed -= (float)Clock.ElapsedFrameTime / 3.5f;
                player.Y -= (float)(Clock.ElapsedFrameTime * verticalSpeed * 0.00001);
            }

            if (horizontalDirection != 0)
            {
                var position = Math.Clamp(player.X + Math.Sign(horizontalDirection) * Clock.ElapsedFrameTime * base_speed, 0, 1);

                player.Scale = new Vector2(Math.Abs(Scale.X) * (horizontalDirection > 0 ? 1 : -1), player.Scale.Y);

                if (position == player.X)
                    return;

                player.X = (float)position;
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
                    jump?.Play();
                    verticalSpeed = 100;
                    break;

                case 0:
                    doubleJump?.Play();
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
    }
}
