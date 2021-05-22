using osu.Game.Rulesets.Bosu.Edit.Blueprints;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Bosu.Edit
{
    public class BosuBlueprintContainer : ComposeBlueprintContainer
    {
        public BosuBlueprintContainer(BosuHitObjectComposer composer)
            : base(composer)
        {
        }

        protected override SelectionHandler<HitObject> CreateSelectionHandler() => new BosuSelectionHandler();

        public override OverlaySelectionBlueprint CreateBlueprintFor(DrawableHitObject hitObject)
        {
            switch (hitObject)
            {
                case DrawableInstantCherry instant:
                    return new InstantCherrySelectionBlueprint(instant);

                case DrawableAngeledCherry angeled:
                    return new AngeledCherrySelectionBlueprint(angeled);
            }

            return base.CreateBlueprintFor(hitObject);
        }
    }
}
