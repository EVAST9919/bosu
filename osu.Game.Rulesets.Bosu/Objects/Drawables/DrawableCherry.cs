using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableCherry : DrawableBosuHitObject
    {
        private readonly float speedMultiplier;

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
            speedMultiplier = h.ApproachRate * 0.025f;

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
            this.FadeIn(HitObject.TimePreempt).Finally(_ => OnObjectReady());

            Scheduler.AddDelayed(() =>
            {
                sprite.FlashColour(Color4.White, 150);
                overlay.FlashColour(Color4.White, 150);
            }, HitObject.TimePreempt);
        }

        protected override void Update()
        {
            base.Update();

            if (!isMoving)
                return;

            var xDelta = Clock.ElapsedFrameTime * Math.Sin(Angle * Math.PI / 180) * speedMultiplier;
            var yDelta = Clock.ElapsedFrameTime * -Math.Cos(Angle * Math.PI / 180) * speedMultiplier;

            Position = new Vector2(Position.X + (float)xDelta, Position.Y + (float)yDelta);
        }

        protected virtual void OnObjectReady()
        {
            isMoving = true;
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
    }
}
