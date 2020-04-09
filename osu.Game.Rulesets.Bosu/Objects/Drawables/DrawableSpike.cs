using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSpike : DrawableBosuHitObject
    {
        private bool isMoving;

        private readonly Sprite sprite;

        private double? startTime;

        public DrawableSpike(Spike h)
            : base(h)
        {
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomCentre;
            X = h.Position.X;
            Size = new Vector2(20);
            AddInternal(sprite = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = textures.Get("spike");
        }

        protected override void Update()
        {
            base.Update();

            if (startTime.HasValue && Clock.CurrentTime > startTime.Value + ((Spike)HitObject).Curve.Duration)
            {
                ApplyResult(r => r.Type = HitResult.Perfect);
                ShouldCheckCollision = false;
                return;
            }

            if (!isMoving)
                return;

            if (startTime.HasValue)
                X = HitObject.Position.X + ((Spike)HitObject).Curve.CurvePositionAt((Clock.CurrentTime - startTime.Value) / ((Spike)HitObject).Curve.Duration).X;
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            sprite.FadeIn(HitObject.TimePreempt);
        }

        protected override void OnObjectReady()
        {
            base.OnObjectReady();

            isMoving = true;
            startTime = Clock.CurrentTime;
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

        protected override bool CheckPlayerCollision(BosuPlayer player) => false;
    }
}
