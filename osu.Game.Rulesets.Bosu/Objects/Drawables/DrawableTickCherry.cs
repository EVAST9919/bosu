namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableTickCherry : DrawableHomingCherry
    {
        protected override float GetBaseSize() => 10;

        public DrawableTickCherry(TickCherry h)
            : base(h)
        {
        }
    }
}
