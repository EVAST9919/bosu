using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Replays
{
    public class BosuReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public List<BosuAction> Actions = new List<BosuAction>();
        public Vector2 Position;
        public bool Jumping;
        public bool Shooting;

        public BosuReplayFrame()
        {
        }

        public BosuReplayFrame(double time, Vector2? position = null, bool jumping = false, bool shooting = false, BosuReplayFrame lastFrame = null)
            : base(time)
        {
            Position = position ?? new Vector2(-100, BosuPlayfield.BASE_SIZE.Y - IWannaExtensions.PLAYER_SIZE.Y / 2f - IWannaExtensions.TILE_SIZE);
            Jumping = jumping;
            Shooting = shooting;

            if (Jumping)
                Actions.Add(BosuAction.Jump);

            if (Shooting)
                Actions.Add(BosuAction.Shoot);

            if (lastFrame != null)
            {
                if (Position.X > lastFrame.Position.X)
                    lastFrame.Actions.Add(BosuAction.MoveRight);
                else if (Position.X < lastFrame.Position.X)
                    lastFrame.Actions.Add(BosuAction.MoveLeft);
            }
        }

        public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
            Position = currentFrame.Position;
            Jumping = currentFrame.ButtonState == ReplayButtonState.Left1;
            Shooting = currentFrame.ButtonState == ReplayButtonState.Left2;

            if (Jumping)
                Actions.Add(BosuAction.Jump);

            if (Shooting)
                Actions.Add(BosuAction.Shoot);

            if (lastFrame is BosuReplayFrame lastBosuFrame)
            {
                if (Position.X > lastBosuFrame.Position.X)
                    lastBosuFrame.Actions.Add(BosuAction.MoveRight);
                else if (Position.X < lastBosuFrame.Position.X)
                    lastBosuFrame.Actions.Add(BosuAction.MoveLeft);
            }
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;

            if (Actions.Contains(BosuAction.Jump)) state |= ReplayButtonState.Left1;
            if (Actions.Contains(BosuAction.Shoot)) state |= ReplayButtonState.Left2;

            return new LegacyReplayFrame(Time, Position.X, Position.Y, state);
        }
    }
}
