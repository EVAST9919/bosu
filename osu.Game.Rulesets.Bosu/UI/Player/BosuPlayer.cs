using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osuTK;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Utils;
using osu.Framework.Bindables;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Bosu.Replays;

namespace osu.Game.Rulesets.Bosu.UI.Player
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        public readonly Bindable<PlayerSprite> Sprite = new Bindable<PlayerSprite>(PlayerSprite.Idle);

        public Vector2 PlayerPosition => movingPlayer.Position;

        public Vector2 PlayerSpeed => new Vector2((float)horizontalSpeed, (float)verticalSpeed);

        private bool rightwards = true;
        private bool midAir = false;
        private bool isDead;
        private int availableJumpCount = 2;
        private int horizontalDirection;
        private double verticalSpeed;
        private double horizontalSpeed;

        private SampleChannel jump;
        private SampleChannel doubleJump;
        private SampleChannel shootSample;

        private readonly Container bulletsContainer;
        private readonly Container movingPlayer;
        private readonly Container spriteContainer;

        public BosuPlayer()
        {
            RelativeSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                bulletsContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                movingPlayer = new Container
                {
                    Origin = Anchor.Centre,
                    Size = IWannaExtensions.PLAYER_SIZE,
                    Position = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.PLAYER_SIZE.Y / 2f - IWannaExtensions.TILE_SIZE),
                    Child = spriteContainer = new Container
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        X = -1.5f,
                        Size = new Vector2(IWannaExtensions.TILE_SIZE)
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            jump = samples.Get("jump");
            doubleJump = samples.Get("double-jump");
            shootSample = samples.Get("shoot");

            Sprite.BindValueChanged(onSpriteChanged, true);
        }

        protected override void Update()
        {
            base.Update();

            if (isDead)
                return;

            var elapsedFrameTime = Clock.ElapsedFrameTime;
            var timeDifference = elapsedFrameTime / 21;

            if (horizontalDirection > 0)
            {
                horizontalSpeed = 3;
                rightwards = true;
            }
            else if (horizontalDirection < 0)
            {
                rightwards = false;
                horizontalSpeed = -3;
            }
            else
                horizontalSpeed = 0;

            if (horizontalSpeed != 0)
                updateSpriteDirection();

            if (midAir)
            {
                if (Precision.AlmostEquals(verticalSpeed, 0, 0.0001))
                    verticalSpeed = 0;

                verticalSpeed -= (verticalSpeed > 0 ? IWannaExtensions.PLAYER_GRAVITY_UP : IWannaExtensions.PLAYER_GRAVITY_DOWN) * timeDifference;

                if (verticalSpeed < -IWannaExtensions.PLAYER_MAX_VERTICAL_SPEED)
                    verticalSpeed = -IWannaExtensions.PLAYER_MAX_VERTICAL_SPEED;
            }

            var replayState = (GetContainingInputManager().CurrentState as RulesetInputManagerInputState<BosuAction>)?.LastReplayState as BosuFramedReplayInputHandler.BosuReplayState;

            if (replayState?.Position.Value != null)
            {
                movingPlayer.X = replayState.Position.Value.X;
                movingPlayer.Y = replayState.Position.Value.Y;

                // Required for accurate jump sounds
                if (verticalSpeed <= 0 && Precision.AlmostEquals(movingPlayer.Y + IWannaExtensions.PLAYER_HALF_HEIGHT, BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.TILE_SIZE, 0.01f))
                    resetJumpLogic();
            }
            else
            {
                if (horizontalSpeed > 0)
                    moveRight(elapsedFrameTime);

                if (horizontalSpeed < 0)
                    moveLeft(elapsedFrameTime);

                if (midAir)
                    moveVertical(timeDifference);

                if (verticalSpeed <= 0)
                {
                    if (movingPlayer.Y + IWannaExtensions.PLAYER_HALF_HEIGHT > BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.TILE_SIZE)
                    {
                        movingPlayer.Y = BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.TILE_SIZE - IWannaExtensions.PLAYER_HALF_HEIGHT;
                        resetJumpLogic();
                    }
                }
            }

            updatePlayerSprite();
        }

        public void Die()
        {
            isDead = true;
            movingPlayer.Hide();
        }

        public void OnHit()
        {
            movingPlayer.FlashColour(Colour4.Red, 250, Easing.Out);
        }

        private void moveRight(double elapsedFrameTime)
        {
            movingPlayer.X += (float)(IWannaExtensions.PLAYER_MAX_HORIZONTAL_SPEED * elapsedFrameTime);

            if (movingPlayer.X + IWannaExtensions.PLAYER_HALF_WIDTH > BosuPlayfield.BASE_SIZE.X - IWannaExtensions.TILE_SIZE)
                movingPlayer.X = BosuPlayfield.BASE_SIZE.X - IWannaExtensions.TILE_SIZE - IWannaExtensions.PLAYER_HALF_WIDTH;
        }

        private void moveLeft(double elapsedFrameTime)
        {
            movingPlayer.X -= (float)(IWannaExtensions.PLAYER_MAX_HORIZONTAL_SPEED * elapsedFrameTime);

            if (movingPlayer.X - IWannaExtensions.PLAYER_HALF_WIDTH < IWannaExtensions.TILE_SIZE)
                movingPlayer.X = IWannaExtensions.TILE_SIZE + IWannaExtensions.PLAYER_HALF_WIDTH;
        }

        private void moveVertical(double timeDifference)
        {
            movingPlayer.Y -= (float)(verticalSpeed * timeDifference);
        }

        private void resetJumpLogic()
        {
            availableJumpCount = 2;
            verticalSpeed = 0;
            midAir = false;
        }

        private void updateSpriteDirection()
        {
            var newScale = new Vector2(rightwards ? 1 : -1, 1);
            var newX = rightwards ? -1.5f : 1.5f;

            if (spriteContainer.Scale != newScale)
                spriteContainer.Scale = newScale;

            if (spriteContainer.X != newX)
                spriteContainer.X = newX;
        }

        public bool OnPressed(BosuAction action)
        {
            if (isDead)
                return false;

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
            if (isDead)
                return;

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

        private void onJumpPressed()
        {
            midAir = true;

            if (availableJumpCount == 0)
                return;

            availableJumpCount--;

            switch (availableJumpCount)
            {
                case 1:
                    jump.Play();
                    verticalSpeed = IWannaExtensions.PLAYER_JUMP_SPEED;
                    break;

                case 0:
                    doubleJump.Play();
                    verticalSpeed = IWannaExtensions.PLAYER_JUMP2_SPEED;
                    break;
            }
        }

        private void onJumpReleased()
        {
            if (verticalSpeed < 0)
                return;

            verticalSpeed *= IWannaExtensions.PLAYER_VERTICAL_STOP_SPEED_MULTIPLIER;
        }

        private void onShoot()
        {
            shootSample.Play();

            var playerPosition = PlayerPosition;

            var newBullet = new DrawableBullet
            {
                Position = playerPosition
            };

            bulletsContainer.Add(newBullet);

            var distance = (rightwards ? BosuPlayfield.BASE_SIZE.X - playerPosition.X : playerPosition.X) - IWannaExtensions.TILE_SIZE;
            var duration = IWannaExtensions.BULLET_SPEED / 20f * distance;

            newBullet.MoveToX(rightwards ? BosuPlayfield.BASE_SIZE.X - IWannaExtensions.TILE_SIZE : IWannaExtensions.TILE_SIZE, duration).Expire(true);
        }

        private void onSpriteChanged(ValueChangedEvent<PlayerSprite> s)
        {
            spriteContainer.Child = new AnimatedPlayerSprite(s.NewValue);
        }

        private void updatePlayerSprite()
        {
            if (verticalSpeed < 0)
            {
                Sprite.Value = PlayerSprite.Fall;
                return;
            }

            if (verticalSpeed > 0)
            {
                Sprite.Value = PlayerSprite.Jump;
                return;
            }

            if (horizontalSpeed != 0)
            {
                Sprite.Value = PlayerSprite.Run;
                return;
            }

            Sprite.Value = PlayerSprite.Idle;
        }
    }

    public enum PlayerSprite
    {
        Idle,
        Run,
        Jump,
        Fall
    }
}
