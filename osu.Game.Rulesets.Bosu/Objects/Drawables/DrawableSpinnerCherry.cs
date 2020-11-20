using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;
using System;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableSpinnerCherry : DrawableCherry
    {
        private readonly Vector2 initialPosition;
        private readonly IHasDuration endTime;
        private readonly float initialAngle;

        public DrawableSpinnerCherry(SpinnerCherry h)
            : base(h)
        {
            endTime = h.EndTime;
            initialAngle = h.InitialAngle;
            initialPosition = new Vector2(BosuPlayfield.BASE_SIZE.X / 2f, BosuPlayfield.BASE_SIZE.Y / 4f);
            Position = initialPosition;
        }

        protected override void Update()
        {
            base.Update();

            Vector2 newPosition = (Time.Current < HitObject.StartTime) ? updatePreemptPosition(Time.Current) : updateLifePosition(Time.Current);

            if (newPosition == Position)
                return;

            Position = newPosition;
        }

        private Vector2 updatePreemptPosition(double time)
        {
            var timeFromStart = getTimeFromStart(time);
            var angle = MathExtensions.GetSafeAngle((float)(initialAngle + timeFromStart / DrawableSpinnerBurstCherry.FULL_ROTATION_DURATION * 360f));

            var currentDistance = timeFromStart / HitObject.TimePreempt * DrawableSpinnerBurstCherry.DISTANCE;

            var rotatedXPos = initialPosition.X + (currentDistance * Math.Sin(angle * Math.PI / 180));
            var rotatedYPos = initialPosition.Y + (currentDistance * -Math.Cos(angle * Math.PI / 180));

            return new Vector2((float)rotatedXPos, (float)rotatedYPos);
        }

        private Vector2 updateLifePosition(double time)
        {
            var angle = MathExtensions.GetSafeAngle((float)(initialAngle + getTimeFromStart(time) / DrawableSpinnerBurstCherry.FULL_ROTATION_DURATION * 360f));

            var rotatedXPos = initialPosition.X + (DrawableSpinnerBurstCherry.DISTANCE * Math.Sin(angle * Math.PI / 180));
            var rotatedYPos = initialPosition.Y + (DrawableSpinnerBurstCherry.DISTANCE * -Math.Cos(angle * Math.PI / 180));

            return new Vector2((float)rotatedXPos, (float)rotatedYPos);
        }

        private double getTimeFromStart(double time) => time - (HitObject.StartTime - HitObject.TimePreempt);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > endTime.EndTime - HitObject.StartTime)
                ApplyResult(r => r.Type = HitResult.IgnoreHit);
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Hit:
                    this.ScaleTo(0, 150).Then().FadeOut();
                    break;
            }
        }
    }
}
