namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableTickCherry : DrawableAngledCherry
    {
        protected override float GetBaseSize() => 15;

        public DrawableTickCherry(TickCherry h)
            : base(h)
        {
        }
    }
}
