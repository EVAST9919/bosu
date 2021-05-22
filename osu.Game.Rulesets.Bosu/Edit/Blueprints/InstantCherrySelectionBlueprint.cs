using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Bosu.Edit.UI;
using osu.Game.Rulesets.Bosu.Objects;

namespace osu.Game.Rulesets.Bosu.Edit.Blueprints
{
    public class InstantCherrySelectionBlueprint : HitObjectSelectionBlueprint<InstantCherry>
    {
        public InstantCherrySelectionBlueprint(InstantCherry instant)
            : base(instant)
        {
            InternalChild = new EditorCherry
            {
                Position = instant.Position
            };
        }
    }
}
