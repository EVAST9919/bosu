using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osuTK;
using System;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Framework.Utils;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract partial class DrawableCherry<T> : DrawableBosuHitObject<T>
        where T : Cherry
    {
        private const int hidden_distance = 70;
        private const int hidden_distance_buffer = 50;
        private const int flash_duration = 300;

        public readonly IBindable<Vector2> PositionBindable = new Bindable<Vector2>();

        protected abstract bool CanHitPlayer { get; }

        public Func<Vector2, bool> CheckHit;
        public Func<Vector2, float> DistanceToPlayer;

        public bool HiddenApplied { get; set; }

        private CherryPiece piece;
        protected double StartTime;
        protected double TimePreempt;

        protected DrawableCherry([CanBeNull] Cherry h = null)
            : base(h)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Origin = Anchor.Centre;
            Size = new Vector2(IWannaExtensions.CHERRY_DIAMETER);
            AddInternal(piece = new CherryPiece
            {
                Origin = Anchor.BottomCentre,
                Anchor = Anchor.BottomCentre
            });

            PositionBindable.BindValueChanged(p => Position = p.NewValue);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AccentColour.BindValueChanged(c => piece.CherryColour = c.NewValue, true);
        }

        protected override void Update()
        {
            base.Update();

            Scale = new Vector2(getScale(Clock.CurrentTime));
            updateFlash(Clock.CurrentTime);

            if (Judged)
                return;

            if (HiddenApplied)
                updateHidden();
        }

        private void updateFlash(double time)
        {
            if (time < StartTime)
            {
                piece.FlashStrength = 0;
                return;
            }

            double timeOffset = Math.Clamp(time, StartTime, StartTime + flash_duration) - StartTime;
            piece.FlashStrength = 1f - (float)(timeOffset / flash_duration);
        }

        private float getScale(double time)
        {
            if (time < StartTime)
            {
                double timeOffset = Math.Clamp(time, StartTime - TimePreempt, StartTime) - StartTime + TimePreempt;
                return (float)(timeOffset / TimePreempt);
            }

            return GetScaleDuringLifetime(time);
        }

        protected virtual float GetScaleDuringLifetime(double time) => 1f;

        private void updateHidden()
        {
            var distance = DistanceToPlayer.Invoke(Position);
            piece.Alpha = Interpolation.ValueAt(Math.Clamp(distance, hidden_distance, hidden_distance + hidden_distance_buffer) - hidden_distance, 0f, 1f, 0, hidden_distance_buffer);
        }

        private double missTime;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset < 0)
                return;

            if (!CanHitPlayer)
                return;

            if (CheckHit?.Invoke(Position) ?? false)
            {
                missTime = timeOffset;
                ApplyMinResult();
            }
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Miss:
                    this.Delay(missTime).FadeOut();
                    break;
            }
        }

        protected override void OnApply()
        {
            base.OnApply();

            PositionBindable.BindTo(HitObject.PositionBindable);
            StartTime = HitObject.StartTime;
            TimePreempt = HitObject.TimePreempt;
        }

        protected override void OnFree()
        {
            base.OnFree();

            PositionBindable.UnbindFrom(HitObject.PositionBindable);
        }

        protected override double InitialLifetimeOffset => HitObject.TimePreempt;
    }
}
