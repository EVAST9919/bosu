using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Bosu.MusicHelpers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class KiaiBackground : CurrentBeatmapProvider
    {
        private const int blind_transform_duration = 50;
        private const int blind_count = 20;

        public override bool RemoveCompletedTransforms => false;

        private readonly Box[] blinds = new Box[blind_count];

        public KiaiBackground()
        {
            RelativeSizeAxes = Axes.Both;

            Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
            });

            var width = (float)1.0 / blind_count;

            for (int i = 0; i < blind_count; i++)
            {
                blinds[i] = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.X,
                    Origin = Anchor.TopCentre,
                    Width = width,
                    Scale = new Vector2(0, 1),
                    X = i * width + width / 2f,
                    Colour = Color4.Black,
                };

                Add(blinds[i]);
            }

            AlwaysPresent = true;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            foreach (var effectPoint in Beatmap.Value.Beatmap.ControlPointInfo.EffectPoints)
            {
                if (effectPoint.KiaiMode)
                {
                    var bpmBeforeKiai = Beatmap.Value.Beatmap.ControlPointInfo.TimingPointAt(effectPoint.Time - 1).BeatLength;

                    for (int i = 0; i < blind_count; i++)
                    {
                        var blind = blinds[i];
                        using (blind.BeginAbsoluteSequence(effectPoint.Time - bpmBeforeKiai + i * (blind_transform_duration / 2f)))
                            blind.ScaleTo(new Vector2(1, 1), blind_transform_duration, Easing.Out);
                    }
                }
                else
                {
                    var bpmBeforeKiaiOff = Beatmap.Value.Beatmap.ControlPointInfo.TimingPointAt(effectPoint.Time - 1).BeatLength;

                    for (int i = 0; i < blind_count; i++)
                    {
                        var blind = blinds[i];
                        using (blind.BeginAbsoluteSequence(effectPoint.Time - bpmBeforeKiaiOff + i * (blind_transform_duration / 2f)))
                            blind.ScaleTo(new Vector2(0, 1), blind_transform_duration, Easing.Out);
                    }
                }
            }
        }
    }
}
