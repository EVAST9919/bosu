using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.MusicHelpers;
using osuTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Bosu.Extensions;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public class KiaiCherryPiece : MusicIntensityController
    {
        private Color4 colour;

        public new Color4 Colour
        {
            get => colour;
            set
            {
                colour = value;
                sprite.Colour = value;
            }
        }

        private readonly Sprite sprite;

        public KiaiCherryPiece()
        {
            Child = sprite = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = textures.Get("cherry-kiai");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Intensity.BindValueChanged(onIntensityChanged, true);
        }

        private void onIntensityChanged(ValueChangedEvent<float> intensity)
        {
            sprite.Alpha = MathExtensions.Map(intensity.NewValue, 4, 6, 0.7f, 1);
        }
    }
}
