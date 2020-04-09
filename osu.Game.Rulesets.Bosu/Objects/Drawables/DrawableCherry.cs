using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;
using System.Collections.Generic;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableCherry : DrawableBosuHitObject
    {
        protected bool WallPassIsBlocked;

        protected Action<Wall> OnWallCollided;

        private readonly float speedMultiplier;

        protected override Color4 GetComboColour(IReadOnlyList<Color4> comboColours) =>
            comboColours[(HitObject.IndexInBeatmap + 1) % comboColours.Count];

        private bool isMoving;
        protected float Angle { get; set; }

        protected virtual float GetBaseSize() => 25;

        private readonly Sprite sprite;
        private readonly Sprite overlay;
        private readonly Sprite branch;

        public DrawableCherry(Cherry h)
            : base(h)
        {
            Angle = h.Angle;
            speedMultiplier = h.ApproachRate * 0.025f * (float)Math.Clamp(h.SpeedMultiplier, 0.75, 1.2);

            Origin = Anchor.Centre;
            Size = new Vector2(GetBaseSize() * ((1.0f - 0.7f * (h.CircleSize - 5) / 5) / 2));
            Position = h.Position;
            Scale = Vector2.Zero;
            Alpha = 0;
            AlwaysPresent = true;

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    sprite = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                    },
                    overlay = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                    },
                    branch = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Position = new Vector2(1, -1)
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = textures.Get("cherry");
            overlay.Texture = textures.Get("cherry-overlay");
            branch.Texture = textures.Get("cherry-branch");

            AccentColour.BindValueChanged(accent => sprite.Colour = accent.NewValue, true);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.ScaleTo(Vector2.One, HitObject.TimePreempt);
            this.FadeIn(HitObject.TimePreempt);
        }

        protected override void Update()
        {
            base.Update();

            if (ShouldCheckCollision)
            {
                if (WallPassIsBlocked)
                    checkWallCollision();
                else
                    checkWallPass();
            }

            if (!isMoving)
                return;

            var xDelta = Clock.ElapsedFrameTime * Math.Sin(Angle * Math.PI / 180) * speedMultiplier;
            var yDelta = Clock.ElapsedFrameTime * -Math.Cos(Angle * Math.PI / 180) * speedMultiplier;

            Position = new Vector2(Position.X + (float)xDelta, Position.Y + (float)yDelta);
        }

        protected override void OnObjectReady()
        {
            base.OnObjectReady();

            isMoving = true;

            sprite.FlashColour(Color4.White, 150);
            overlay.FlashColour(Color4.White, 150);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            switch (state)
            {
                case ArmedState.Idle:
                    break;

                default:
                    this.FadeOut();
                    break;
            }
        }

        protected override bool CheckPlayerCollision(BosuPlayer player)
        {
            // Let's assume that player is a circle for simplicity

            var playerPosition = player.PlayerPositionInPlayfieldSpace();
            var distance = Math.Sqrt(Math.Pow(Math.Abs(playerPosition.X - Position.X), 2) + Math.Pow(Math.Abs(playerPosition.Y - Position.Y), 2));
            var playerRadius = player.PlayerDrawSize().X / 2f;
            var cherryRadius = DrawSize.X / 2f;

            if (distance < playerRadius + cherryRadius)
                return true;

            return false;
        }

        private void checkWallCollision()
        {
            if (Position.X >= BosuPlayfield.BASE_SIZE.X - DrawSize.X / 2f)
                OnWallCollided?.Invoke(Wall.Right);
            else if (Position.X <= DrawSize.X / 2f)
                OnWallCollided?.Invoke(Wall.Left);
            else if (Position.Y >= BosuPlayfield.BASE_SIZE.Y - DrawSize.Y / 2f)
                OnWallCollided?.Invoke(Wall.Down);
            else if (Position.Y <= DrawSize.Y / 2f)
                OnWallCollided?.Invoke(Wall.Up);
        }

        private void checkWallPass()
        {
            if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f || Position.X < -DrawSize.X / 2f || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f || Position.Y < -DrawSize.Y / 2f)
            {
                ApplyResult(r => r.Type = HitResult.Perfect);
                ShouldCheckCollision = false;
            }
        }

        protected enum Wall
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}
