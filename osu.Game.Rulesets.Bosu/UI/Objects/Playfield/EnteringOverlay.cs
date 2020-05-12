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
        private SampleChannel enteringSample;

        public EnteringOverlay()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = box = new Box
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black
            };
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            enteringSample = samples.Get("entering");
        }

        public void Enter(double delay)
        {
            box.Delay(delay).ResizeHeightTo(0, 700, Easing.OutQuint);
            Scheduler.AddDelayed(() => enteringSample.Play(), delay);
        }
    }
}
