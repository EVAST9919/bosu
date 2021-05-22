using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Bosu.Edit.UI;

namespace osu.Game.Rulesets.Bosu.Edit.Blueprints
{
    public class InstantCherrySelectionBlueprint : OverlaySelectionBlueprint
    {
        public InstantCherrySelectionBlueprint(DrawableInstantCherry instant)
            : base(instant)
        {
            InternalChild = new EditorCherry
            {
                Position = instant.Position
            };
        }
    }
}
