using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield
{
    public class DeathParticle : Sprite
    {
        public DeathParticle()
        {
            Size = new Vector2(10);
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Texture = textures.Get("death-particle");
        }
    }
}
