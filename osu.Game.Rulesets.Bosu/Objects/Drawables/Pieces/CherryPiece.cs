using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public class CherryPiece : BeatSyncedContainer
    {
        private Color4 colour;

        public new Color4 Colour
        {
            get => colour;
            set
            {
                colour = value;

                main.Colour = value;
                additional.Colour = value;
            }
        }

        private readonly CherrySubPiece main;
        private readonly CherrySubPiece additional;

        public CherryPiece()
        {
            Size = new Vector2(IWannaExtensions.CHERRY_DIAMETER);
            AddRange(new Drawable[]
            {
                main = new CherrySubPiece(1),
                additional = new CherrySubPiece(2)
            });

            applyClockState(true);
        }

        public void Flash(double duration)
        {
            main.Flash(duration);
            additional.Flash(duration);
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            applyClockState(beatIndex % 2 == 0);
        }

        private void applyClockState(bool state)
        {
            main.Alpha = state ? 1 : 0;
            additional.Alpha = state ? 0 : 1;
        }
    }
}
