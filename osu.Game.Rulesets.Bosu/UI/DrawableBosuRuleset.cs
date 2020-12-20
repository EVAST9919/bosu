using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class DrawableBosuRuleset : DrawableRuleset<BosuHitObject>
    {
        public BosuHealthProcessor HealthProcessor
        {
            set
            {
                if (Playfield is BosuPlayfield p)
                    p.ApplyHealthProcessor(value);
            }
        }

        public DrawableBosuRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override PassThroughInputManager CreateInputManager() => new BosuInputManager(Ruleset.RulesetInfo);

        protected override Playfield CreatePlayfield() => new BosuPlayfield();

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new BosuPlayfieldAdjustmentContainer();

        public override DrawableHitObject<BosuHitObject> CreateDrawableRepresentation(BosuHitObject h) => null;

        protected override ReplayRecorder CreateReplayRecorder(Replay replay) => new BosuReplayRecorder(replay, (BosuPlayfield)Playfield);

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new BosuFramedReplayInputHandler(replay);
    }
}
