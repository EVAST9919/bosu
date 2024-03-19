using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public partial class BeatSyncedSprite : Sprite
    {
        private int lastBeat;

        private TimingControlPoint? lastTimingPoint { get; set; }

        protected bool IsKiaiTime { get; private set; }

        /// <summary>
        /// The time in milliseconds until the next beat.
        /// </summary>
        public double TimeUntilNextBeat { get; private set; }

        /// <summary>
        /// The time in milliseconds since the last beat
        /// </summary>
        public double TimeSinceLastBeat { get; private set; }

        /// <summary>
        /// How many beats per beatlength to trigger. Defaults to 1.
        /// </summary>
        public int Divisor { get; set; } = 1;

        /// <summary>
        /// An optional minimum beat length. Any beat length below this will be multiplied by two until valid.
        /// </summary>
        public double MinimumBeatLength { get; set; }

        /// <summary>
        /// Whether this container is currently tracking a beat sync provider.
        /// </summary>
        protected bool IsBeatSyncedWithTrack { get; private set; }

        [Resolved]
        protected IBeatSyncProvider BeatSyncSource { get; private set; } = null!;

        protected virtual void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
        }

        protected override void Update()
        {
            TimingControlPoint timingPoint;
            EffectControlPoint effectPoint;

            IsBeatSyncedWithTrack = BeatSyncSource.Clock.IsRunning;

            double currentTrackTime;

            if (IsBeatSyncedWithTrack)
            {
                currentTrackTime = BeatSyncSource.Clock.CurrentTime;

                timingPoint = BeatSyncSource.ControlPoints?.TimingPointAt(currentTrackTime) ?? TimingControlPoint.DEFAULT;
                effectPoint = BeatSyncSource.ControlPoints?.EffectPointAt(currentTrackTime) ?? EffectControlPoint.DEFAULT;
            }
            else
            {
                // this may be the case where the beat syncing clock has been paused.
                // we still want to show an idle animation, so use this container's time instead.
                currentTrackTime = Clock.CurrentTime;

                timingPoint = TimingControlPoint.DEFAULT;
                effectPoint = EffectControlPoint.DEFAULT;
            }

            double beatLength = timingPoint.BeatLength / Divisor;

            while (beatLength < MinimumBeatLength)
                beatLength *= 2;

            int beatIndex = (int)((currentTrackTime - timingPoint.Time) / beatLength) - (timingPoint.OmitFirstBarLine ? 1 : 0);

            // The beats before the start of the first control point are off by 1, this should do the trick
            if (currentTrackTime < timingPoint.Time)
                beatIndex--;

            TimeUntilNextBeat = (timingPoint.Time - currentTrackTime) % beatLength;
            if (TimeUntilNextBeat <= 0)
                TimeUntilNextBeat += beatLength;

            TimeSinceLastBeat = beatLength - TimeUntilNextBeat;

            if (ReferenceEquals(timingPoint, lastTimingPoint) && beatIndex == lastBeat)
                return;

            OnNewBeat(beatIndex, timingPoint, effectPoint, BeatSyncSource.CurrentAmplitudes);

            lastBeat = beatIndex;
            lastTimingPoint = timingPoint;

            IsKiaiTime = effectPoint.KiaiMode;
        }
    }
}
