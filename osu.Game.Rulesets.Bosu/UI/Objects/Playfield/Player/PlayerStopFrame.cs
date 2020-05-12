using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osu.Framework.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player
{
    public class PlayerStopFrame : CompositeDrawable
    {
        private readonly PlayerState state;
        private readonly Sprite sprite;

        public PlayerStopFrame(PlayerState state, bool rightwards)
        {
            this.state = state;

            Size = new Vector2(15);
            Origin = Anchor.Centre;
            AddInternal(sprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0.7f,
            });

            if (!rightwards)
                sprite.Scale = new Vector2(-1, 1);
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            switch (state)
            {
                case PlayerState.Fall:
                    sprite.Texture = textures.Get("Player/player_fall_0");
                    break;

                case PlayerState.Jump:
                    sprite.Texture = textures.Get("Player/player_jump_0");
                    break;

                case PlayerState.Run:
                    sprite.Texture = textures.Get("Player/player_run_0");
                    break;
            }
        }
    }
}
