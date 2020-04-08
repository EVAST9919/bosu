using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class BosuBackground : CompositeDrawable
    {
        private readonly Bindable<BackgroundType> bgType = new Bindable<BackgroundType>();
        private readonly Bindable<double> bgDim = new Bindable<double>();

        [Resolved]
        private TextureStore textures { get; set; }

        private readonly Container bgContainer;
        private readonly Container dimContainer;

        public BosuBackground()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                bgContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                dimContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(BosuRulesetConfigManager config)
        {
            config.BindWith(BosuRulesetSetting.Background, bgType);
            config.BindWith(BosuRulesetSetting.PlayfieldDim, bgDim);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            bgType.BindValueChanged(bg => onBackgroundChanged(bg.NewValue), true);
            bgDim.BindValueChanged(dim => onDimChanged(dim.NewValue), true);
        }

        private void onDimChanged(double newDim)
        {
            dimContainer.Alpha = (float)newDim;
        }

        private void onBackgroundChanged(BackgroundType background)
        {
            Drawable newBackground;

            switch (background)
            {
                case BackgroundType.Red:
                    newBackground = new Sprite
                    {
                        Texture = textures.Get("Backgrounds/Red"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    };
                    break;

                case BackgroundType.White:
                    newBackground = new Sprite
                    {
                        Texture = textures.Get("Backgrounds/White"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    };
                    break;

                default:
                case BackgroundType.None:
                    newBackground = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new[]
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
                            }
                        }
                    };
                    break;
            }

            bgContainer.Child = newBackground;
        }
    }
}
