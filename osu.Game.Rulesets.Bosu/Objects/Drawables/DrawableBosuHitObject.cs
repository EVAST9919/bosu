using JetBrains.Annotations;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableBosuHitObject<T> : DrawableHitObject<BosuHitObject>
        where T : BosuHitObject
    {
        protected new T HitObject => (T)base.HitObject;

        protected DrawableBosuHitObject([CanBeNull] T h = null)
            : base(h)
        {
        }
    }
}
