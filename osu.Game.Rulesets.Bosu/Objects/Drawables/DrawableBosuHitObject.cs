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

        protected BosuPlayer Player;

        private bool shouldCheckCollision;

        public DrawableBosuHitObject(BosuHitObject hitObject)
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

        protected abstract bool CheckCollision(BosuPlayer player);

        protected override void Update()
        {
            base.Update();

            if (!shouldCheckCollision)
                return;

            if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f || Position.X < -DrawSize.X / 2f || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f || Position.Y < -DrawSize.Y / 2f)
            {
                ApplyResult(r => r.Type = HitResult.Perfect);
                shouldCheckCollision = false;
            }

            if (CheckCollision(Player) && shouldCheckCollision)
            {
                Player.PlayMissAnimation();
                ApplyResult(r => r.Type = HitResult.Miss);
                shouldCheckCollision = false;
            }
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
        }
    }
}
