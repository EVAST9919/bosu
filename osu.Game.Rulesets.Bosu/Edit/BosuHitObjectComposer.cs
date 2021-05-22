using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit.Compose.Components;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Bosu.Edit
{
    public class BosuHitObjectComposer : HitObjectComposer<BosuHitObject>
    {
        public BosuHitObjectComposer(Ruleset ruleset)
            : base(ruleset)
        {
        }

        protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => Array.Empty<HitObjectCompositionTool>();

        protected override DrawableRuleset<BosuHitObject> CreateDrawableRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            => new EditorDrawableBosuRuleset(ruleset, beatmap, mods);

        protected override ComposeBlueprintContainer CreateBlueprintContainer() => new BosuBlueprintContainer(this);
    }
}
