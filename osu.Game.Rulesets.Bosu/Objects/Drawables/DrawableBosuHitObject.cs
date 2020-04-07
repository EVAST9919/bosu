using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK.Graphics;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableBosuHitObject : DrawableHitObject<BosuHitObject>
    {
        public Func<BosuHitObject, bool> CheckPosition;

        protected bool WallPassIsBlocked;

        protected Action<Wall> OnWallCollided;

        protected BosuPlayer Player;
        private bool shouldCheckCollision;

        protected DrawableBosuHitObject(BosuHitObject hitObject)
            : base(hitObject)
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.AddDelayed(() => shouldCheckCollision = true, HitObject.TimePreempt);
        }

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

        public void GetPlayerToTrace(BosuPlayer player) => Player = player;

        protected override Color4 GetComboColour(IReadOnlyList<Color4> comboColours) =>
            comboColours[(HitObject.IndexInBeatmap + 1) % comboColours.Count];

        protected override void Update()
        {
            base.Update();

            if (WallPassIsBlocked)
                checkWallCollision();

            if (!shouldCheckCollision)
                return;

            if (checkCollision(Player) && shouldCheckCollision)
            {
                Player.PlayMissAnimation();
                ApplyResult(r => r.Type = HitResult.Miss);
                shouldCheckCollision = false;
            }

            if (shouldCheckCollision && !WallPassIsBlocked)
                checkWallPass();
        }

        private void checkWallCollision()
        {
            if (Position.X >= BosuPlayfield.BASE_SIZE.X - DrawSize.X / 2f)
                OnWallCollided?.Invoke(Wall.Right);
            else if (Position.X <= DrawSize.X / 2f)
                OnWallCollided?.Invoke(Wall.Left);
            else if (Position.Y >= BosuPlayfield.BASE_SIZE.Y - DrawSize.Y / 2f)
                OnWallCollided?.Invoke(Wall.Down);
            else if (Position.Y <= DrawSize.Y / 2f)
                OnWallCollided?.Invoke(Wall.Up);
        }

        private void checkWallPass()
        {
            if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f || Position.X < -DrawSize.X / 2f || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f || Position.Y < -DrawSize.Y / 2f)
            {
                ApplyResult(r => r.Type = HitResult.Perfect);
                shouldCheckCollision = false;
            }
        }

        private bool checkCollision(BosuPlayer player)
        {
            var playerPosition = player.PlayerPositionInPlayfieldSpace();

            if (playerPosition.X + player.PlayerDrawSize().X / 2f > Position.X - DrawSize.X / 2f && playerPosition.X - player.PlayerDrawSize().X / 2f < Position.X + DrawSize.X / 2f
                && playerPosition.Y + player.PlayerDrawSize().Y / 2f > Position.Y - DrawSize.Y / 2f && playerPosition.Y - player.PlayerDrawSize().Y / 2f < Position.Y + DrawSize.Y / 2f)
                return true;

            return false;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
        }

        protected enum Wall
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}
