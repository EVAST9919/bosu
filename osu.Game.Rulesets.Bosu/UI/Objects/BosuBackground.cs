using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Bosu.Configuration;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class BosuBackground : CompositeDrawable
    {
        private readonly Bindable<BackgroundType> bgType = new Bindable<BackgroundType>();

        [Resolved]
        private TextureStore textures { get; set; }

        public BosuBackground()
        {
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(BosuRulesetConfigManager config)
        {
            config.BindWith(BosuRulesetSetting.Background, bgType);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            bgType.BindValueChanged(bg => onBackgroundChanged(bg.NewValue), true);
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
                    };
                    break;

                case BackgroundType.White:
                    newBackground = new Sprite
                    {
                        Texture = textures.Get("Backgrounds/White"),
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

            InternalChild = newBackground;
        }
    }
}
