using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Edit.UI;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Edit;

namespace osu.Game.Rulesets.Bosu.Edit.Blueprints
{
    public partial class AngeledCherrySelectionBlueprint : HitObjectSelectionBlueprint<AngeledCherry>
    {
        private readonly EditorCherry circle;

        public AngeledCherrySelectionBlueprint(AngeledCherry angeled)
            : base(angeled)
        {
            InternalChildren = new Drawable[]
            {
                new EditorCherryPath(angeled.Position, angeled.EndPosition),
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
