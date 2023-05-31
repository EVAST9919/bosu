using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Bosu.UI.Entering
{
    public partial class BeatmapBackgroundSprite : BufferedContainer
    {
        [Resolved]
        private TextureStore textures { get; set; }

        private readonly Sprite sprite;

        public BeatmapBackgroundSprite()
            : base(cachedFrameBuffer: true)
        {
            RelativeSizeAxes = Axes.Both;

            Child = sprite = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
            };
        }

        public void SetBeatmap(WorkingBeatmap beatmap)
        {
            sprite.Texture = beatmap?.Background ?? textures.Get(@"Backgrounds/bg4");
        }
    }
}
