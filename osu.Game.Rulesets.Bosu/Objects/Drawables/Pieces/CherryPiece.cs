using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public partial class CherryPiece : BeatSyncedContainer
    {
        public new Color4 Colour
        {
            get => cherryBase.Colour;
            set => cherryBase.Colour = value;
        }

        private readonly Sprite cherryBase;
        private readonly Sprite overlay;
        private readonly Sprite flash;

        private Texture base1;
        private Texture base2;
        private Texture overlay1;
        private Texture overlay2;
        private Texture flash1;
        private Texture flash2;

        public CherryPiece()
        {
            Size = new Vector2(IWannaExtensions.CHERRY_DIAMETER);
            AddRange(new Drawable[]
            {
                cherryBase = new Sprite
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = IWannaExtensions.CHERRY_FULL_SIZE,
                },
                overlay = new Sprite
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = IWannaExtensions.CHERRY_FULL_SIZE,
                },
                flash = new Sprite
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = IWannaExtensions.CHERRY_FULL_SIZE,
                    Alpha = 0
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            base1 = textures.Get($"Cherry/cherry-base-1");
            base2 = textures.Get($"Cherry/cherry-base-2");
            overlay1 = textures.Get($"Cherry/cherry-overlay-1");
            overlay2 = textures.Get($"Cherry/cherry-overlay-2");
            flash1 = textures.Get($"Cherry/cherry-flash-1");
            flash2 = textures.Get($"Cherry/cherry-flash-2");

            updateTextures(true);
        }

        public void Flash(double duration) => flash.FadeInFromZero(10, Easing.OutQuint).Then().FadeOut(duration);

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            updateTextures(beatIndex % 2 == 0);
        }

        private void updateTextures(bool state)
        {
            cherryBase.Texture = state ? base1 : base2;
            overlay.Texture = state ? overlay1 : overlay2;
            flash.Texture = state ? flash1 : flash2;
        }
    }
}
