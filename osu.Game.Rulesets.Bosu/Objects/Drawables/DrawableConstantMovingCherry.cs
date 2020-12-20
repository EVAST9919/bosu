﻿using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableConstantMovingCherry<T> : DrawableCherry<T>
        where T : ConstantMovingCherry
    {
        public readonly IBindable<float> SpeedMultiplierBindable = new Bindable<float>();

        protected override bool CanHitPlayer { get; set; } = true;

        protected abstract float GetTargetAngle();

        private double duration;

        protected DrawableConstantMovingCherry([CanBeNull] T h = null)
            : base(h)
        {
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            using (BeginDelayedSequence(HitObject.TimePreempt))
            {
                Vector2 finalPosition = getFinalPosition();
                duration = MathExtensions.Distance(HitObject.Position, finalPosition) / SpeedMultiplierBindable.Value * 3;
                this.MoveTo(finalPosition, duration);
            }
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Hit:
                    this.Delay(duration).FadeOut();
                    break;
            }
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            base.CheckForResult(userTriggered, timeOffset);

            if (timeOffset < duration)
                return;

            ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }

        protected override void OnApply()
        {
            base.OnApply();

            SpeedMultiplierBindable.BindTo(HitObject.SpeedMultiplierBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();

            SpeedMultiplierBindable.UnbindFrom(HitObject.SpeedMultiplierBindable);
        }

        private Vector2 getFinalPosition()
        {
            var angle = MathExtensions.GetSafeAngle(GetTargetAngle());

            float finalX = 0;
            float finalY = 0;

            switch (getTargetWall(angle))
            {
                case Wall.Bottom:
                    finalY = BosuPlayfield.BASE_SIZE.Y;
                    finalX = getXPosition(HitObject.Position, finalY, angle);
                    break;

                case Wall.Top:
                    finalY = 0;
                    finalX = getXPosition(HitObject.Position, finalY, angle);
                    break;

                case Wall.Left:
                    finalX = 0;
                    finalY = getYPosition(HitObject.Position, finalX, angle);
                    break;

                case Wall.Right:
                    finalX = BosuPlayfield.BASE_SIZE.X;
                    finalY = getYPosition(HitObject.Position, finalX, angle);
                    break;
            }

            return new Vector2(finalX, finalY);
        }

        private Wall getTargetWall(float angle)
        {
            // Top/Right
            if (angle <= 90)
            {
                if (angle < getCornerAngle(new Vector2(BosuPlayfield.BASE_SIZE.X, 0)))
                    return Wall.Top;

                return Wall.Right;
            }

            // Right/Bottom
            if (angle <= 180)
            {
                if (angle < getCornerAngle(new Vector2(BosuPlayfield.BASE_SIZE.X, BosuPlayfield.BASE_SIZE.Y)))
                    return Wall.Right;

                return Wall.Bottom;
            }

            // Bottom/Left
            if (angle <= 270)
            {
                if (angle < getCornerAngle(new Vector2(0, BosuPlayfield.BASE_SIZE.Y)))
                    return Wall.Bottom;

                return Wall.Left;
            }

            // Left/Top
            if (angle < getCornerAngle(Vector2.Zero))
                return Wall.Left;

            return Wall.Top;
        }

        private float getCornerAngle(Vector2 cornerPosition)
            => MathExtensions.GetSafeAngle((float)(Math.Atan2(HitObject.Y - cornerPosition.Y, HitObject.X - cornerPosition.X) * 180 / Math.PI) - 90);

        private float getYPosition(Vector2 initialPosition, float finalX, float angle)
            => (float)(initialPosition.Y + ((finalX - initialPosition.X) / -Math.Tan(angle * Math.PI / 180)));

        private float getXPosition(Vector2 initialPosition, float finalY, float angle)
            => (float)(initialPosition.X + ((finalY - initialPosition.Y) * -Math.Tan(angle * Math.PI / 180)));

        private enum Wall
        {
            Top,
            Right,
            Left,
            Bottom
        }
    }
}
