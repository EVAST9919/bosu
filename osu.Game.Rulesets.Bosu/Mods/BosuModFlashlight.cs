using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Bosu.Objects;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Bosu.Mods
{
    public class BosuModFlashlight : ModFlashlight<BosuHitObject>
    {
        public override double ScoreMultiplier => 1.12;

        private const float default_flashlight_size = 250;

        public override Flashlight CreateFlashlight() => new BosuFlashlight(playfield);

        private BosuPlayfield playfield;

        public override void ApplyToDrawableRuleset(DrawableRuleset<BosuHitObject> drawableRuleset)
        {
            playfield = (BosuPlayfield)drawableRuleset.Playfield;
            base.ApplyToDrawableRuleset(drawableRuleset);
        }

        private class BosuFlashlight : Flashlight
        {
            private readonly BosuPlayfield playfield;

            public BosuFlashlight(BosuPlayfield playfield)
            {
                this.playfield = playfield;
                FlashlightSize = new Vector2(0, getSizeFor(0));
            }

            protected override void Update()
            {
                base.Update();

                var playerPos = playfield.Player.PlayerPosition();

                FlashlightPosition = playfield.ToSpaceOfOtherDrawable(new Vector2(playerPos.X, playerPos.Y), this);
            }

            private float getSizeFor(int combo)
            {
                if (combo > 200)
                    return default_flashlight_size * 0.65f;
                else if (combo > 100)
                    return default_flashlight_size * 0.8f;
                else
                    return default_flashlight_size;
            }

            protected override void OnComboChange(ValueChangedEvent<int> e)
            {
                this.TransformTo(nameof(FlashlightSize), new Vector2(0, getSizeFor(e.NewValue)), FLASHLIGHT_FADE_DURATION);
            }

            protected override string FragmentShader => "CircularFlashlight";
        }
    }
}
