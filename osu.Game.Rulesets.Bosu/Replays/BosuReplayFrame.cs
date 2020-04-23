using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;

namespace osu.Game.Rulesets.Bosu.Replays
{
    public class BosuReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public List<BosuAction> Actions = new List<BosuAction>();
        public float Position;
        public bool Jumping;
        public bool Shooting;

        public BosuReplayFrame()
        {
        }

        public BosuReplayFrame(double time, float? position = null, bool jumping = false, bool shooting = false, BosuReplayFrame lastFrame = null)
            : base(time)
        {
            Position = position ?? -100;
            Jumping = jumping;
            Shooting = shooting;

            if (Jumping)
                Actions.Add(BosuAction.Jump);

            if (Shooting)
                Actions.Add(BosuAction.Shoot);

            if (lastFrame != null)
            {
                if (Position > lastFrame.Position)
                    lastFrame.Actions.Add(BosuAction.MoveRight);
                else if (Position < lastFrame.Position)
                    lastFrame.Actions.Add(BosuAction.MoveLeft);
            }
        }

        public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
            Position = currentFrame.Position.X;
            Jumping = currentFrame.ButtonState == ReplayButtonState.Left1;
            Shooting = currentFrame.ButtonState == ReplayButtonState.Left2;

            if (Jumping)
                Actions.Add(BosuAction.Jump);

            if (Shooting)
                Actions.Add(BosuAction.Shoot);

            if (lastFrame is BosuReplayFrame lastBosuFrame)
            {
                if (Position > lastBosuFrame.Position)
                    lastBosuFrame.Actions.Add(BosuAction.MoveRight);
                else if (Position < lastBosuFrame.Position)
                    lastBosuFrame.Actions.Add(BosuAction.MoveLeft);
            }
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;

            if (Actions.Contains(BosuAction.Jump)) state |= ReplayButtonState.Left1;
            if (Actions.Contains(BosuAction.Shoot)) state |= ReplayButtonState.Left2;

            return new LegacyReplayFrame(Time, Position, null, state);
        }
    }
}
