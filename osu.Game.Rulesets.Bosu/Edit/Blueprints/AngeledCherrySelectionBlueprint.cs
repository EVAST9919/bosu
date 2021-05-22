using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Edit.UI;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Edit;

namespace osu.Game.Rulesets.Bosu.Edit.Blueprints
{
    public class AngeledCherrySelectionBlueprint : OverlaySelectionBlueprint
    {
        private readonly EditorCherry circle;

        public AngeledCherrySelectionBlueprint(DrawableAngeledCherry angeled)
            : base(angeled)
        {
            InternalChildren = new Drawable[]
            {
                new EditorCherryPath(angeled.HitObject.Position, ((AngeledCherry)angeled.HitObject).EndPosition),
                circle = new EditorCherry()
            };
        }

        protected override void Update()
        {
            base.Update();
            circle.Position = DrawableObject.Position;
        }
    }
}
