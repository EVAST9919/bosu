using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class SpinnerCherry : Cherry
    {
        public IHasDuration EndTime { get; set; }

        public float InitialAngle { get; set; }
    }
}
