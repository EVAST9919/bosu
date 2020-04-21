using System;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableFallingCherry : DrawableCherry
    {
        private const float speed_multiplier = 4.5f;

        protected override float GetBaseSize() => 20;

        private readonly float finalSize;
        private float duration;

        public DrawableFallingCherry(FallingCherry h)
            : base(h)
        {
            AlwaysPresent = true;
            finalSize = Size.X;
            X = h.Position.X;
            Y = -finalSize;
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            duration = BosuPlayfield.BASE_SIZE.Y / MathExtensions.Map((float)((FallingCherry)HitObject).SpeedMultiplier, 0, 3, 0.9f, 1.2f) * speed_multiplier;

            this.Delay(HitObject.TimePreempt).MoveToY(BosuPlayfield.BASE_SIZE.Y + finalSize / 2f, duration);
        }

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

                if (timeOffset > duration)
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
