using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.Edit
{
    public class EditorDrawableBosuRuleset : DrawableBosuRuleset
    {
        public EditorDrawableBosuRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new EditorBosuPlayfield();
    }
}
