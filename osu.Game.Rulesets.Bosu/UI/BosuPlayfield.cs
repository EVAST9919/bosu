using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Bosu.Objects.Drawables;
using osu.Game.Rulesets.Bosu.UI.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class BosuPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(512, 384);

        internal readonly BosuPlayer Player;

        public BosuPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.TopCentre,
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    EdgeSmoothness = Vector2.One
                },
                new Box
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    EdgeSmoothness = Vector2.One
                },
                new Box
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreRight,
                    RelativeSizeAxes = Axes.Y,
                    Width = 1,
                    EdgeSmoothness = Vector2.One
                },
                new Box
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Y,
                    Width = 1,
                    EdgeSmoothness = Vector2.One
                },
                HitObjectContainer,
                Player = new BosuPlayer()
            };
        }

        protected override HitObjectContainer CreateHitObjectContainer() => new JsbHitObjectContainer();

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            var drawable = (DrawableBosuHitObject)h;
            drawable.GetPlayerToTrace(Player);
        }

        private class JsbHitObjectContainer : HitObjectContainer
        {
            public JsbHitObjectContainer()
            {
                Masking = true;
            }
        }
    }
}
