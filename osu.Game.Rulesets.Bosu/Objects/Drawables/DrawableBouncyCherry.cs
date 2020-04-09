namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableBouncyCherry : DrawableCherry
    {
        private readonly float bounceTime;

        public DrawableBouncyCherry(BouncyCherry h)
            : base(h)
        {
            bounceTime = h.LifeTime;
            WallPassIsBlocked = true;

            OnWallCollided += onWallCollided;
        }

        protected override void OnObjectReady()
        {
            base.OnObjectReady();

            Scheduler.AddDelayed(() =>
            {
                WallPassIsBlocked = false;
            }, bounceTime);
        }

        private void onWallCollided(Wall wall)
        {
            switch (wall)
            {
                case Wall.Up:
                case Wall.Down:
                    Angle = 180 - Angle;
                    return;

                case Wall.Left:
                case Wall.Right:
                    Angle = 360 - Angle;
                    return;
            }
        }
    }
}
