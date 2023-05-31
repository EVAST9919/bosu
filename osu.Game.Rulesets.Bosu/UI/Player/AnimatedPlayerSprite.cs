using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Bosu.UI.Player
{
    public partial class AnimatedPlayerSprite : TextureAnimation
    {
        private const double duration = 100;

        private readonly PlayerSprite sprite;

        public AnimatedPlayerSprite(PlayerSprite sprite)
        {
            this.sprite = sprite;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            switch (sprite)
            {
                case PlayerSprite.Fall:
                    for (int i = 0; i < 2; i++)
                        AddFrame(textures.Get($"Player/player_fall_{i}"), duration);
                    break;

                case PlayerSprite.Idle:
                    for (int i = 0; i < 4; i++)
                        AddFrame(textures.Get($"Player/player_idle_{i}"), duration);
                    break;

                case PlayerSprite.Jump:
                    for (int i = 0; i < 2; i++)
                        AddFrame(textures.Get($"Player/player_jump_{i}"), duration);
                    break;

                case PlayerSprite.Run:
                    for (int i = 0; i < 5; i++)
                        AddFrame(textures.Get($"Player/player_run_{i}"), duration / 2);
                    break;
            }
        }
    }
}
