using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Edit.UI;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Edit;

namespace osu.Game.Rulesets.Bosu.Edit.Blueprints
{
    public partial class AngledCherrySelectionBlueprint : HitObjectSelectionBlueprint<AngledCherry>
    {
        private readonly EditorCherry circle;

        public AngledCherrySelectionBlueprint(AngledCherry angled)
            : base(angled)
        {
            InternalChildren = new Drawable[]
            {
                new EditorCherryPath(angled.Position, angled.EndPosition),
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
