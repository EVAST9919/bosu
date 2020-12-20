using osu.Framework.Bindables;
using osu.Game.Rulesets.Bosu.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Bosu.Objects
{
    public class AngeledCherry : ConstantMovingCherry
    {
        public readonly Bindable<float> AngleBindable = new Bindable<float>();

        public float Angle
        {
            get => AngleBindable.Value;
            set => AngleBindable.Value = value;
        }

        public override Judgement CreateJudgement() => new BosuJudgement();
    }
}
