using osu.Game.Rulesets.Bosu.MusicHelpers;
using osu.Framework.Graphics;
using osuTK;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class BeatmapCard : CurrentBeatmapProvider
    {
        public static readonly Vector2 SIZE = new Vector2(180, 90);

        public BeatmapCard()
        {
            Size = SIZE;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Children = new Drawable[]
            {
                new BeatmapBackgroundSprite(Beatmap.Value)
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
                    Child = new TextFlowContainer(s =>
                    {
                        s.Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold);
                        s.Shadow = true;
                        s.Anchor = Anchor.BottomCentre;
                        s.Origin = Anchor.BottomCentre;
                    })
                    {
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Text = Beatmap.Value.Beatmap.Metadata.Title,
                    }
                }
            };
        }
    }
}
