using osuTK;
using System;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableMovingCherry : DrawableCherry
    {
        protected float Angle { get; set; }

        private readonly float speedMultiplier;
        private readonly float finalSize;

        public DrawableMovingCherry(MovingCherry h)
            : base(h)
        {
            Angle = h.Angle;
            speedMultiplier = MathExtensions.Map((float)h.SpeedMultiplier, 0, 3, 0.9f, 1.1f) / 4.5f;
            finalSize = Size.X;
        }

        protected override void OnObjectUpdate()
        {
            base.OnObjectUpdate();
            this.MoveTo(getOffset()); // Should be moved to initial transforms
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            base.CheckForResult(userTriggered, timeOffset);

            if (!Judged && timeOffset > 0)
            {
                // Wall pass check
                if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f
                    || Position.X < -DrawSize.X / 2f
                    || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f
                    || Position.Y < -DrawSize.Y / 2f)
                {
                    ApplyResult(r => r.Type = HitResult.Perfect);
                }
            }
        }

        private Vector2 getOffset()
        {
            var elapsedTime = Clock.CurrentTime - HitObject.StartTime;
            var xPosition = HitObject.Position.X + (elapsedTime * speedMultiplier * Math.Sin(Angle * Math.PI / 180));
            var yPosition = HitObject.Position.Y + (elapsedTime * speedMultiplier * -Math.Cos(Angle * Math.PI / 180));
            return new Vector2((float)xPosition, (float)yPosition);
        }

        protected override bool CollidedWithPlayer(BosuPlayer player)
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

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                case ArmedState.Miss:
                    this.FadeOut();
                    break;
            }
        }
    }
}
