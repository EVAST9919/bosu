using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class PlayerAnimation : TextureAnimation
    {
        private const double duration = 100;

        private readonly PlayerState state;

        public PlayerAnimation(PlayerState state)
        {
            this.state = state;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            switch (state)
            {
                case PlayerState.Fall:
                    for (int i = 0; i < 2; i++)
                        AddFrame(textures.Get($"Player/player_fall_{i}"), duration);
                    break;

                case PlayerState.Idle:
                    for (int i = 0; i < 4; i++)
                        AddFrame(textures.Get($"Player/player_idle_{i}"), duration);
                    break;

                case PlayerState.Jump:
                    for (int i = 0; i < 2; i++)
                        AddFrame(textures.Get($"Player/player_jump_{i}"), duration);
                    break;

                case PlayerState.Run:
                    for (int i = 0; i < 5; i++)
                        AddFrame(textures.Get($"Player/player_run_{i}"), duration / 2);
                    break;
            }
        }
    }

    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall
    }
}
