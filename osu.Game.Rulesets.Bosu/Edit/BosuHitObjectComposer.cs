using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit.Compose.Components;
using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.Bosu.Edit
{
    public partial class BosuHitObjectComposer : HitObjectComposer<BosuHitObject, BosuAction>
    {
        public BosuHitObjectComposer(Ruleset ruleset)
            : base(ruleset)
        {
        }

        protected override IReadOnlyList<CompositionTool<BosuAction>> CompositionTools => Array.Empty<CompositionTool<BosuAction>>();

        protected override DrawableRuleset<BosuHitObject> CreateDrawableRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            => new EditorDrawableBosuRuleset(ruleset, beatmap, mods);

        protected override ComposeBlueprintContainer CreateBlueprintContainer() => new BosuBlueprintContainer(this);
        public override Bindable<TernaryState> SelectionNewComboState { get; } = new Bindable<TernaryState>();
    }
}
