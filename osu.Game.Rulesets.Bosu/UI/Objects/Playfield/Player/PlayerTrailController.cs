using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.MusicHelpers;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player
{
    public class PlayerTrailController : MusicAmplitudesProvider
    {
        private const int delay = 250;

        public BosuPlayer Player { get; set; }

        public PlayerTrailController()
        {
            RelativeSizeAxes = Axes.Both;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            IsKiai.BindValueChanged(kiai => onKiaiChanged(kiai.NewValue), true);
        }

        private void onKiaiChanged(bool kiai)
        {
            if (kiai)
                Scheduler.AddDelayed(createStopFrme, delay, true);
            else
                Scheduler.CancelDelayedTasks();
        }

        private void createStopFrme()
        {
            if (Player == null)
                return;

            var playerPos = Player.PlayerPosition();
            var x = playerPos.X;
            var y = playerPos.Y + Player.PlayerSize().Y / 2;

            var frame = new PlayerStopFrame(Player.GetCurrentState(), Player.Rightwards())
            {
                Position = new Vector2(x, y)
            };

            AddInternal(frame);

            frame.FadeOut(delay * 4, Easing.OutQuint).Expire();
        }
    }
}
