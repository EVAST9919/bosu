using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Bosu.Configuration;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class PlayfieldBackground : CompositeDrawable
    {
        private readonly Bindable<BackgroundType> bgType = new Bindable<BackgroundType>();
        private readonly Bindable<double> bgDim = new Bindable<double>();

        [Resolved(canBeNull: true)]
        private BosuRulesetConfigManager config { get; set; }

        private readonly Container bgContainer;
        private readonly Container dimContainer;

        public PlayfieldBackground()
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
        private void load()
        {
            config?.BindWith(BosuRulesetSetting.Background, bgType);
            config?.BindWith(BosuRulesetSetting.PlayfieldDim, bgDim);
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
                case BackgroundType.White:
                    newBackground = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    };
                    break;

                default:
                case BackgroundType.None:
                    newBackground = Empty();
                    break;
            }

            bgContainer.Child = newBackground;
        }
    }
}
