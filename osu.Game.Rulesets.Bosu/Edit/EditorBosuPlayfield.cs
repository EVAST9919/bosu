using osu.Game.Rulesets.Bosu.UI;

namespace osu.Game.Rulesets.Bosu.Edit
{
    public class EditorBosuPlayfield : BosuPlayfield
    {
        protected override bool UseEnteringAnimation { get; set; } = false;
    }
}
