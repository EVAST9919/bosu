using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Bosu.Beatmaps;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Bosu.Replays
{
    internal class BosuAutoGenerator : AutoGenerator
    {
        public new BosuBeatmap Beatmap => (BosuBeatmap)base.Beatmap;

        public BosuAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
            Replay = new Replay();
        }

        protected Replay Replay;

        public override Replay Generate()
        {
            Replay.Frames.Add(new BosuReplayFrame(-100000));
            return Replay;
        }
    }
}
