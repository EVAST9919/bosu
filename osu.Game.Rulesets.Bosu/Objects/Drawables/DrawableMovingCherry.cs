using osuTK;
using System;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Bosu.Extensions;

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
            speedMultiplier = MathExtensions.Map((float)h.SpeedMultiplier, 0, 3, 0.85f, 1.2f) / 4.5f;
            finalSize = Size.X;
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
