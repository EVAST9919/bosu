using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public abstract class ConstantMovingCherry : Cherry
    {
        public readonly Bindable<float> SpeedMultiplierBindable = new Bindable<float>(1);

        public float SpeedMultiplier
        {
            get => SpeedMultiplierBindable.Value;
            set => SpeedMultiplierBindable.Value = value;
        }
    }
}
