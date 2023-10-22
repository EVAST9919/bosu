using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;
using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;

namespace osu.Game.Rulesets.Bosu.Extensions
{
    public static class CherryTransformableExtensions
    {
        public static TransformSequence<T> FlashTo<T>(this T sprite, float value, double duration = 0, Easing easing = Easing.None)
            where T : CherryPiece
            => sprite.TransformTo(nameof(sprite.FlashStrength), value, duration, easing);

        public static TransformSequence<T> FlashTo<T>(this TransformSequence<T> t, float value, double duration = 0, Easing easing = Easing.None)
            where T : CherryPiece
            => t.Append(o => o.FlashTo(value, duration, easing));
    }
}
