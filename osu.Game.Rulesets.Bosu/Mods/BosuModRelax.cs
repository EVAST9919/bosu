using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModRelax : ModRelax, IApplicableToDrawableRuleset<BosuHitObject>
    {
        public override string Description => @"Use the mouse to control the player.";

        public void ApplyToDrawableRuleset(DrawableRuleset<BosuHitObject> drawableRuleset)
        {
            drawableRuleset.Cursor.Add(new MouseInputHelper((BosuPlayfield)drawableRuleset.Playfield));
        }

        private class MouseInputHelper : Drawable, IKeyBindingHandler<BosuAction>, IRequireHighFrequencyMousePosition
        {
            private readonly Container player;

            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

            public MouseInputHelper(BosuPlayfield playfield)
            {
                player = playfield.Player.Player;
                RelativeSizeAxes = Axes.Both;
            }

            public bool OnPressed(BosuAction action)
            {
                switch (action)
                {
                    case BosuAction.MoveLeft:
                    case BosuAction.MoveRight:
                        return true;
                }

                return false;
            }

            public void OnReleased(BosuAction action)
            {
                switch (action)
                {
                    case BosuAction.MoveLeft:
                    case BosuAction.MoveRight:
                        return;
                }
            }

            protected override bool OnMouseMove(MouseMoveEvent e)
            {
                player.X = e.MousePosition.X / DrawSize.X * BosuPlayfield.BASE_SIZE.X;
                return base.OnMouseMove(e);
            }
        }
    }
}
