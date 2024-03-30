using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Bosu.Extensions;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces
{
    public partial class CherryPiece : BeatSyncedSprite
    {
        private Color4 cherryColour = Color4.White;

        public Color4 CherryColour
        {
            get => cherryColour;
            set
            {
                cherryColour = value;
                Invalidate(Invalidation.DrawNode);
            }
        }

        private bool state;

        public bool State
        {
            get => state;
            set
            {
                state = value;
                Texture = state ? base1 : base2;
            }
        }

        private float flashStrength;

        public float FlashStrength
        {
            get => flashStrength;
            set
            {
                if (flashStrength == value)
                    return;

                flashStrength = value;
                Invalidate(Invalidation.DrawNode);
            }
        }

        private Texture base1;
        private Texture base2;
        private Texture overlay1;
        private Texture overlay2;
        private Texture flash1;
        private Texture flash2;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Size = IWannaExtensions.CHERRY_FULL_SIZE;

            base1 = textures.Get("Cherry/cherry-base-1");
            base2 = textures.Get("Cherry/cherry-base-2");
            overlay1 = textures.Get("Cherry/cherry-overlay-1");
            overlay2 = textures.Get("Cherry/cherry-overlay-2");
            flash1 = textures.Get("Cherry/cherry-flash-1");
            flash2 = textures.Get("Cherry/cherry-flash-2");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Invalidate(Invalidation.DrawNode);
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            State = beatIndex % 2 == 0;
        }

        public void Flash(double duration) => this.FlashTo(1f, 10, Easing.OutQuint).Then().FlashTo(0f, duration);

        protected override DrawNode CreateDrawNode() => new CherryDrawNode(this);

        private class CherryDrawNode : SpriteDrawNode
        {
            private readonly CherryPiece source;

            public CherryDrawNode(CherryPiece source)
                : base(source)
            {
                this.source = source;
            }

            private Texture overlayTexture;
            private Texture flashTexture;
            private ColourInfo cherryColour;
            private ColourInfo flashColour;
            private float flashStrength;

            public override void ApplyState()
            {
                base.ApplyState();

                cherryColour = DrawColourInfo.Colour;
                cherryColour.ApplyChild(source.cherryColour);

                bool state = source.state;
                overlayTexture = state ? source.overlay1 : source.overlay2;

                flashStrength = source.flashStrength;

                if (flashStrength == 0f)
                    return;

                flashTexture = state ? source.flash1 : source.flash2;

                flashColour = DrawColourInfo.Colour;
                flashColour.ApplyChild(new Color4(1f, 1f, 1f, flashStrength));
            }

            protected override void Blit(IRenderer renderer)
            {
                if (DrawRectangle.Width == 0 || DrawRectangle.Height == 0)
                    return;

                renderer.DrawQuad(Texture, ScreenSpaceDrawQuad, cherryColour);
                renderer.DrawQuad(overlayTexture, ScreenSpaceDrawQuad, DrawColourInfo.Colour);

                if (flashStrength == 0f)
                    return;

                renderer.DrawQuad(flashTexture, ScreenSpaceDrawQuad, flashColour);
            }
        }
    }
}
