using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
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

        protected override Playfield CreatePlayfield() => new BosuPlayfield();

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new BosuPlayfieldAdjustmentContainer();

        public override DrawableHitObject<BosuHitObject> CreateDrawableRepresentation(BosuHitObject h)
        {
            switch (h)
            {
                case Cherry bullet:
                    return new DrawableCherry(bullet);
            }

            return null;
        }
    }
}
