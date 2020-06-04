using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osu.Framework.Graphics;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class PlayfieldBorder : Sprite
    {
        public PlayfieldBorder()
        {
            Size = BosuPlayfield.BASE_SIZE + Vector2.One;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Texture = textures.Get("Playfield/border");
        }
    }
}
