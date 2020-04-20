using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class DrawableBosuRuleset : DrawableRuleset<BosuHitObject>
    {
        public DrawableBosuRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override PassThroughInputManager CreateInputManager() => new BosuInputManager(Ruleset.RulesetInfo);

        protected override Playfield CreatePlayfield() => new BosuPlayfield(((BosuRuleset)Ruleset).HealthProcessor);

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new BosuPlayfieldAdjustmentContainer();

        protected override ReplayRecorder CreateReplayRecorder(Replay replay) => new BosuReplayRecorder(replay, (BosuPlayfield)Playfield);

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new BosuFramedReplayInputHandler(replay);

        public override DrawableHitObject<BosuHitObject> CreateDrawableRepresentation(BosuHitObject h)
        {
            switch (h)
            {
                case SoundHitObject sound:
                    return new DrawableSoundHitObject(sound);

                case SliderPartCherry sliderPart:
                    return new DrawableSliderPartCherry(sliderPart);

                case TickCherry tick:
                    return new DrawableTickCherry(tick);

                case MovingCherry moving:
                    return new DrawableMovingCherry(moving);
            }

            return null;
        }
    }
}
