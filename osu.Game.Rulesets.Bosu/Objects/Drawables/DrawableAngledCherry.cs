using osuTK;
using System;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableAngledCherry : DrawableCherry
    {
        private readonly float speedMultiplier;

        private readonly float finalSize;

        public DrawableAngledCherry(AngledCherry h)
            : base(h)
        {
            AlwaysPresent = true;
            speedMultiplier = MathExtensions.Map((float)h.SpeedMultiplier, 0, 3, 0.9f, 1.1f) / 4.5f;
            finalSize = Size.X;
        }

        protected virtual float GetAngle() => ((AngledCherry)HitObject).Angle;

        private double missTime;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
            {
                if (collidedWithPlayer(Player))
                {
                    Player.PlayMissAnimation();
                    missTime = timeOffset;
                    ApplyResult(r => r.Type = HitResult.Miss);
                    return;
                }

                if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f
                    || Position.X < -DrawSize.X / 2f
                    || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f
                    || Position.Y < -DrawSize.Y / 2f)
                    ApplyResult(r => r.Type = HitResult.Perfect);
            }
        }

        private bool collidedWithPlayer(BosuPlayer player)
        {
            // Let's assume that player is a circle for simplicity

            var playerPosition = player.PlayerPosition();
            var distance = Math.Sqrt(Math.Pow(Math.Abs(playerPosition.X - Position.X), 2) + Math.Pow(Math.Abs(playerPosition.Y - Position.Y), 2));
            var playerRadius = player.PlayerDrawSize().X / 2f;
            var cherryRadius = finalSize / 2f;

            if (distance < playerRadius + cherryRadius)
                return true;

            return false;
        }

        private float? angle;

        protected override void Update()
        {
            base.Update();

            Vector2 newPosition = (Time.Current > HitObject.StartTime) ? updatePosition() : HitObject.Position;

            if (newPosition == Position)
                return;

            Position = newPosition;
        }

        private Vector2 updatePosition()
        {
            if (angle == null)
                angle = GetAngle();

            var elapsedTime = Clock.CurrentTime - HitObject.StartTime;
            var xPosition = HitObject.Position.X + (elapsedTime * speedMultiplier * Math.Sin((angle ?? 0) * Math.PI / 180));
            var yPosition = HitObject.Position.Y + (elapsedTime * speedMultiplier * -Math.Cos((angle ?? 0) * Math.PI / 180));
            return new Vector2((float)xPosition, (float)yPosition);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            switch (state)
            {
                case ArmedState.Miss:
                    // Check DrawableHitCircle L#168
                    this.Delay(missTime).FadeOut();
                    break;
            }
        }
    }
}
