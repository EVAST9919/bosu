using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osuTK;
using System;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableCherry<T> : DrawableBosuHitObject<T>
        where T : Cherry
    {
        public readonly IBindable<Vector2> PositionBindable = new Bindable<Vector2>();
        public readonly Bindable<int> IndexInBeatmap = new Bindable<int>();

        protected abstract bool CanHitPlayer { get; set; }

        public Func<Vector2, bool> CheckHit;

        private CherryPiece piece;

        protected DrawableCherry([CanBeNull] T h = null)
            : base(h)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Alpha = 0;
            Scale = Vector2.Zero;

            Origin = Anchor.Centre;
            Size = new Vector2(IWannaExtensions.CHERRY_DIAMETER);
            AddInternal(piece = new CherryPiece());

            AccentColour.BindValueChanged(c => piece.Colour = c.NewValue);
            PositionBindable.BindValueChanged(p => Position = p.NewValue);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();
            this.FadeInFromZero();
            this.ScaleTo(Vector2.One, HitObject.TimePreempt);

            using (piece.BeginDelayedSequence(HitObject.TimePreempt))
                piece.Flash(300);
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
                ApplyResult(h => h.Type = h.Judgement.MinResult);
                return;
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
            IndexInBeatmap.BindTo(HitObject.IndexInBeatmapBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();

            PositionBindable.UnbindFrom(HitObject.PositionBindable);
            IndexInBeatmap.UnbindFrom(HitObject.IndexInBeatmapBindable);
        }


        protected override double InitialLifetimeOffset => HitObject.TimePreempt;
    }
}
