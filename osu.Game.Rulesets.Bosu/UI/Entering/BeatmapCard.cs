using osu.Game.Rulesets.Bosu.MusicHelpers;
using osu.Framework.Graphics;
using osuTK;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Bosu.UI.Entering
{
    public partial class BeatmapCard : CurrentBeatmapProvider
    {
        public static readonly Vector2 SIZE = new Vector2(290, 150);

        private readonly BeatmapBackgroundSprite sprite;
        private readonly TextFlowContainer text;

        public BeatmapCard()
        {
            Size = SIZE;
            InternalChildren = new Drawable[]
            {
                sprite = new BeatmapBackgroundSprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0.3f
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(5),
                    Child = text = new TextFlowContainer(s =>
                    {
                        s.Font = OsuFont.GetFont(size: 20, weight: FontWeight.SemiBold);
                        s.Shadow = true;
                        s.Anchor = Anchor.BottomCentre;
                        s.Origin = Anchor.BottomCentre;
                    })
                    {
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre
                    }
                }
            };
        }

        protected override void OnBeatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            base.OnBeatmapChanged(beatmap);

            sprite.SetBeatmap(beatmap.NewValue);
            text.Text = beatmap.NewValue.Beatmap.Metadata.Title;
        }
    }
}
