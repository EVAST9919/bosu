using osu.Framework.Bindables;
using System;

namespace osu.Game.Rulesets.Bosu.MusicHelpers
{
    public abstract class MusicAmplitudesProvider : CurrentBeatmapProvider
    {
        public readonly BindableBool IsKiai = new BindableBool();

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;
            OnAmplitudesUpdate(track?.CurrentAmplitudes.FrequencyAmplitudes ?? new ReadOnlyMemory<float>());
            IsKiai.Value = Beatmap.Value?.Beatmap.ControlPointInfo.EffectPointAt(track?.CurrentTime ?? 0).KiaiMode ?? false;
        }

        protected virtual void OnAmplitudesUpdate(ReadOnlyMemory<float> amplitudes)
        {
        }
    }
}
