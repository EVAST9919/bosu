using osu.Framework.Input.StateChanges;
using osu.Framework.Utils;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using osuTK;
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

        protected Vector2? Position
        {
            get
            {
                var frame = CurrentFrame;

                if (frame == null)
                    return null;

                return NextFrame != null ?
                    new Vector2(
                    Interpolation.ValueAt(CurrentTime.Value, frame.Position.X, NextFrame.Position.X, frame.Time, NextFrame.Time),
                    Interpolation.ValueAt(CurrentTime.Value, frame.Position.Y, NextFrame.Position.Y, frame.Time, NextFrame.Time))
                    : frame.Position;
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
            public Vector2? Position { get; set; }
        }
    }
}
