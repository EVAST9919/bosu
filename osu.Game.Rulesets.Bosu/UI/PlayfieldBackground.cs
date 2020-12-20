using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Bosu.MusicHelpers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.UI
{
    public class PlayfieldBackground : CurrentBeatmapProvider
    {
        private const int blind_transform_duration = 50;
        private const int blind_count = 20;

        public override bool RemoveCompletedTransforms => false;

        private readonly Box[] blinds = new Box[blind_count];

        public PlayfieldBackground()
        {
            RelativeSizeAxes = Axes.Both;
            AlwaysPresent = true;

            AddInternal(new Box
            {
                RelativeSizeAxes = Axes.Both,
            });

            var width = 1f / blind_count;

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

                AddInternal(blinds[i]);
            }
        }

        protected override void OnBeatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            base.OnBeatmapChanged(beatmap);

            var newBeatmap = beatmap.NewValue;

            foreach (var effectPoint in newBeatmap.Beatmap.ControlPointInfo.EffectPoints)
            {
                if (effectPoint.KiaiMode)
                {
                    var bpmBeforeKiai = newBeatmap.Beatmap.ControlPointInfo.TimingPointAt(effectPoint.Time - 1).BeatLength;

                    for (int i = 0; i < blind_count; i++)
                    {
                        using (blinds[i].BeginAbsoluteSequence(effectPoint.Time - bpmBeforeKiai + i * (blind_transform_duration / 2f)))
                            blinds[i].ScaleTo(new Vector2(1, 1), blind_transform_duration, Easing.Out);
                    }
                }
                else
                {
                    var bpmBeforeKiaiOff = newBeatmap.Beatmap.ControlPointInfo.TimingPointAt(effectPoint.Time - 1).BeatLength;

                    for (int i = 0; i < blind_count; i++)
                    {
                        using (blinds[i].BeginAbsoluteSequence(effectPoint.Time - bpmBeforeKiaiOff + i * (blind_transform_duration / 2f)))
                            blinds[i].ScaleTo(new Vector2(0, 1), blind_transform_duration, Easing.Out);
                    }
                }
            }
        }
    }
}
