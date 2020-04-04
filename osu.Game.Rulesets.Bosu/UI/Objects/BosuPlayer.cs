using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using System;
using osuTK.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Bosu.UI.Objects
{
    public class BosuPlayer : CompositeDrawable, IKeyBindingHandler<BosuAction>
    {
        private const double base_speed = 1.0 / 2048;

        private int horizontalDirection;

        private readonly Container player;
        private readonly Sprite drawablePlayer;

        public BosuPlayer()
        {
            RelativeSizeAxes = Axes.Both;
            AddInternal(player = new Container
            {
                Origin = Anchor.BottomCentre,
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0.5f, 1),
                Size = new Vector2(15),
                Child = drawablePlayer = new Sprite
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            drawablePlayer.Texture = textures.Get("bosu");
        }

        public Vector2 PlayerPositionInPlayfieldSpace() => player.Position * BosuPlayfield.BASE_SIZE;

        public Vector2 PlayerDrawSize() => player.DrawSize * 0.7f;

        public void PlayMissAnimation()
        {
            drawablePlayer.FadeColour(Color4.Red).Then().FadeColour(Color4.White, 1000, Easing.OutQuint);
        }

        public bool OnPressed(BosuAction action)
        {
            switch (action)
            {
                case BosuAction.MoveLeft:
                    horizontalDirection--;
                    return true;

                case BosuAction.MoveRight:
                    horizontalDirection++;
                    return true;

                case BosuAction.Jump:
                    return true;
            }

            return false;
        }

        public void OnReleased(BosuAction action)
        {
            switch (action)
            {
                case BosuAction.MoveLeft:
                    horizontalDirection++;
                    return;

                case BosuAction.MoveRight:
                    horizontalDirection--;
                    return;

                case BosuAction.Jump:
                    return;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (horizontalDirection != 0)
            {
                var position = Math.Clamp(player.X + Math.Sign(horizontalDirection) * Clock.ElapsedFrameTime * base_speed, 0, 1);

                player.Scale = new Vector2(Math.Abs(Scale.X) * (horizontalDirection > 0 ? 1 : -1), player.Scale.Y);

                if (position == player.X)
                    return;

                player.X = (float)position;
            }
        }
    }
}
