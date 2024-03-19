using osu.Game.Rulesets.Bosu.Edit.Blueprints;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Bosu.Edit
{
    public partial class BosuBlueprintContainer : ComposeBlueprintContainer
    {
        public BosuBlueprintContainer(BosuHitObjectComposer composer)
            : base(composer)
        {
        }

        protected override SelectionHandler<HitObject> CreateSelectionHandler() => new BosuSelectionHandler();

        public override HitObjectSelectionBlueprint CreateHitObjectBlueprintFor(HitObject hitObject)
        {
            switch (hitObject)
            {
                case InstantCherry instant:
                    return new InstantCherrySelectionBlueprint(instant);

                case AngledCherry angled:
                    return new AngledCherrySelectionBlueprint(angled);
            }

            return base.CreateHitObjectBlueprintFor(hitObject);
        }
    }
}
