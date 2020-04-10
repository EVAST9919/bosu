using osu.Framework.Graphics;
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
                HitObjectContainer,
                Player = new BosuPlayer()
            };
        }

        protected override HitObjectContainer CreateHitObjectContainer() => new BosuHitObjectContainer();

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            var drawable = (DrawableBosuHitObject)h;
            drawable.GetPlayerToTrace(Player);
        }

        private class BosuHitObjectContainer : HitObjectContainer
        {
            public BosuHitObjectContainer()
            {
                Masking = true;
            }
        }
    }
}
