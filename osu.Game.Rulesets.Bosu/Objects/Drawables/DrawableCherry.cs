using osu.Framework.Graphics;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Framework.Allocation;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableCherry : DrawableBosuHitObject
    {
        protected override Color4 GetComboColour(IReadOnlyList<Color4> comboColours) =>
            comboColours[(HitObject.IndexInBeatmap + 1) % comboColours.Count];

        protected virtual float GetBaseSize() => 25;

        private readonly CherryPiece cherry;
        private readonly KiaiCherryPiece kiaiCherry;

        protected DrawableCherry(Cherry h)
            : base(h)
        {
            Origin = Anchor.Centre;
            Size = new Vector2(GetBaseSize() * MathExtensions.Map(h.CircleSize, 0, 10, 0.2f, 1));
            Position = h.Position;
            Scale = Vector2.Zero;

            if (h.IsKiai)
                AddInternal(kiaiCherry = new KiaiCherryPiece
                {
                    Scale = new Vector2(1.8f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                });

            AddInternal(cherry = new CherryPiece
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AccentColour.BindValueChanged(accent =>
            {
                if (kiaiCherry != null)
                    kiaiCherry.Colour = accent.NewValue;

                cherry.Colour = accent.NewValue;
            }, true);
        }

        protected override void UpdateInitialTransforms()
        {
            this.ScaleTo(Vector2.One, HitObject.TimePreempt);

            cherry.Sprite.Delay(HitObject.TimePreempt).Then().FlashColour(Color4.White, 300);
        }
    }
}
