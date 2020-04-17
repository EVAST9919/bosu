using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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
                new BosuBackground(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        HitObjectContainer,
                        Player = new BosuPlayer()
                    }
                }
            };
        }

        public override void Add(DrawableHitObject h)
        {
            if (h is DrawableMovingCherry drawable)
            {
                drawable.GetPlayerToTrace(Player);
                base.Add(drawable);
                return;
            }

            base.Add(h);
        }
    }
}
