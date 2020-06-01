namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public class DrawableTickCherry : DrawableHomingCherry
    {
        protected override float GetBaseSize() => base.GetBaseSize() / 2;

        public DrawableTickCherry(TickCherry h)
            : base(h)
        {
        }
    }
}
