using osu.Framework.Graphics;
using osuTK;
using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;
using System.Collections.Generic;
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableCherry : DrawableBosuHitObject
    {
        public Action<DrawableCherry> Ready;

        protected override Color4 GetComboColour(IReadOnlyList<Color4> comboColours) =>
            comboColours[(HitObject.IndexInBeatmap + 1) % comboColours.Count];

        protected virtual float GetBaseSize() => 25;

        private readonly Sprite sprite;
        private readonly Sprite overlay;
        private readonly Sprite branch;

        public DrawableCherry(Cherry h)
            : base(h)
        {
            Origin = Anchor.Centre;
            Size = new Vector2(GetBaseSize() * MathExtensions.Map(h.CircleSize, 0, 10, 0.2f, 1));
            Position = h.Position;
            Scale = Vector2.Zero;
            Alpha = 0;
            AlwaysPresent = true;

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    sprite = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                    },
                    overlay = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                    },
                    branch = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Position = new Vector2(1, -1)
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = textures.Get("cherry");
            overlay.Texture = textures.Get("cherry-overlay");
            branch.Texture = textures.Get("cherry-branch");

            AccentColour.BindValueChanged(accent => sprite.Colour = accent.NewValue, true);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.ScaleTo(Vector2.One, GetReadyStateOffset());
            this.FadeIn(GetReadyStateOffset());
        }

        protected override void OnObjectReady()
        {
            base.OnObjectReady();

            Ready?.Invoke(this);

            sprite.FlashColour(Color4.White, 150);
            overlay.FlashColour(Color4.White, 150);
        }
    }
}
