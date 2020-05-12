using osu.Framework.Graphics.Containers;
using osu.Game.Tests.Visual;
using osu.Framework.Graphics;
using osuTK;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.Tests
{
    public class CollisionTest : OsuTestScene
    {
        private readonly Circle circle;
        private readonly Box box;
        private readonly Container container;

        public CollisionTest()
        {
            Add(container = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(600),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black
                    },
                    box = new Box
                    {
                        Position = new Vector2(100),
                        Size = new Vector2(50, 100),
                    },
                    circle = new Circle
                    {
                        Size = new Vector2(100),
                        Origin = Anchor.Centre,
                    }
                }
            });
        }

        protected override void Update()
        {
            base.Update();
            circle.Position = container.ToLocalSpace(GetContainingInputManager().CurrentState.Mouse.Position);

            circle.Colour = MathExtensions.Collided(50, circle.Position, box.Position, box.Size) ? Color4.Red : Color4.White;
        }
    }
}
