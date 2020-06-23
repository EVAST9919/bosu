using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using System;

namespace osu.Game.Rulesets.Bosu.MusicHelpers
{
    public class MusicIntensityController : MusicAmplitudesProvider
    {
        public readonly BindableFloat Intensity = new BindableFloat();

        protected override void OnAmplitudesUpdate(ReadOnlyMemory<float> amplitudes)
        {
            float sum = 0;
            amplitudes.Span.ToArray().ForEach(amp => sum += amp);

            if (IsKiai.Value)
                sum *= 1.2f;

            Intensity.Value = sum;
        }
    }
}
