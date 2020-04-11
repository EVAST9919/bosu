using osu.Framework.Graphics;
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
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableCherry : DrawableBosuHitObject
    {
        private readonly float speedMultiplier;
        private readonly float finalSize;
        public Action<DrawableCherry> Ready;

        protected override Color4 GetComboColour(IReadOnlyList<Color4> comboColours) =>
            comboColours[(HitObject.IndexInBeatmap + 1) % comboColours.Count];

        protected float Angle { get; set; }

        protected virtual float GetBaseSize() => 25;

        private readonly Sprite sprite;
        private readonly Sprite overlay;
        private readonly Sprite branch;

        public DrawableCherry(Cherry h)
            : base(h)
        {
            Angle = h.Angle;
            speedMultiplier = MathExtensions.Map((float)h.SpeedMultiplier, 0, 3, 0.85f, 1.2f) / 4.5f;

            Origin = Anchor.Centre;
            Size = new Vector2(GetBaseSize() * MathExtensions.Map(h.CircleSize, 0, 10, 0.2f, 1));
            finalSize = Size.X;
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

        protected override void OnObjectReady()
        {
            base.OnObjectReady();

            Ready?.Invoke(this);

            sprite.FlashColour(Color4.White, 150);
            overlay.FlashColour(Color4.White, 150);
        }

        protected override void OnObjectUpdate()
        {
            base.OnObjectUpdate();

            if (Result?.HasResult ?? true)
                return;

            OnMove();
            CheckWallPass();
        }

        protected virtual void OnMove()
        {
            var xDelta = Clock.ElapsedFrameTime * Math.Sin(Angle * Math.PI / 180) * speedMultiplier;
            var yDelta = Clock.ElapsedFrameTime * -Math.Cos(Angle * Math.PI / 180) * speedMultiplier;

            Position = new Vector2(Position.X + (float)xDelta, Position.Y + (float)yDelta);
        }

        protected override bool CheckPlayerCollision(BosuPlayer player)
        {
            // Let's assume that player is a circle for simplicity

            var playerPosition = player.PlayerPositionInPlayfieldSpace();
            var distance = Math.Sqrt(Math.Pow(Math.Abs(playerPosition.X - Position.X), 2) + Math.Pow(Math.Abs(playerPosition.Y - Position.Y), 2));
            var playerRadius = player.PlayerDrawSize().X / 2f;
            var cherryRadius = finalSize / 2f;

            if (distance < playerRadius + cherryRadius)
                return true;

            return false;
        }

        protected virtual void CheckWallPass()
        {
            if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f || Position.X < -DrawSize.X / 2f || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f || Position.Y < -DrawSize.Y / 2f)
            {
                ApplyResult(r => r.Type = HitResult.Perfect);
            }
        }
    }
}
