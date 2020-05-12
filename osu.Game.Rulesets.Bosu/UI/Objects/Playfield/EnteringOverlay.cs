using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using osu.Framework.Audio.Sample;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class EnteringOverlay : CompositeDrawable
    {
        public override bool RemoveCompletedTransforms => false;

        private readonly Box box;
        private readonly BeatmapCard card;
        private SampleChannel enteringSample;

        public EnteringOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            Masking = true;

            AddRangeInternal(new Drawable[]
            {
                box = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                },
                card = new BeatmapCard
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Y = -BeatmapCard.SIZE.Y,
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            enteringSample = samples.Get("entering");
        }

        public void Enter(double delay)
        {
            Scheduler.AddDelayed(() => enteringSample.Play(), delay);

            using (box.BeginDelayedSequence(delay))
                box.ResizeHeightTo(0, 800, Easing.Out);

            using (card.BeginDelayedSequence(delay))
                card.MoveToY(0, 900).Delay(2200).MoveToY(-BeatmapCard.SIZE.Y, 800, Easing.Out);
        }
    }
}
