using osu.Framework.Input.StateChanges;
using osu.Framework.Utils;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.Replays
{
    public class BosuFramedReplayInputHandler : FramedReplayInputHandler<BosuReplayFrame>
    {
        public BosuFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(BosuReplayFrame frame) => true;

        protected float? Position
        {
            get
            {
                var frame = CurrentFrame;

                if (frame == null)
                    return null;

                return NextFrame != null ? Interpolation.ValueAt(CurrentTime.Value, frame.Position, NextFrame.Position, frame.Time, NextFrame.Time) : frame.Position;
            }
        }

        public override void CollectPendingInputs(List<IInput> inputs)
        {
            if (Position.HasValue)
            {
                inputs.Add(new BosuReplayState
                {
                    PressedActions = CurrentFrame?.Actions ?? new List<BosuAction>(),
                    Position = Position.Value
                });
            }
        }

        public class BosuReplayState : ReplayState<BosuAction>
        {
            public float? Position { get; set; }
        }
    }
}
