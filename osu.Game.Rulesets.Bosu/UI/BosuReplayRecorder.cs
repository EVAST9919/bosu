using System.Collections.Generic;
using osu.Game.Replays;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class BosuReplayRecorder : ReplayRecorder<BosuAction>
    {
        private readonly BosuPlayfield playfield;

        public BosuReplayRecorder(Replay target, BosuPlayfield playfield)
            : base(target)
        {
            this.playfield = playfield;
        }

        protected override ReplayFrame HandleFrame(Vector2 mousePosition, List<BosuAction> actions, ReplayFrame previousFrame)
            => new BosuReplayFrame(Time.Current, playfield.Player.PlayerPositionInPlayfieldSpace().X, actions.Contains(BosuAction.Jump), previousFrame as BosuReplayFrame);
    }
}
