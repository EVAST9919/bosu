using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;
using System.ComponentModel;

namespace osu.Game.Rulesets.Bosu
{
    public partial class BosuInputManager : RulesetInputManager<BosuAction>
    {
        public BosuInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum BosuAction
    {
        [Description("Move Left")]
        MoveLeft,

        [Description("Move Right")]
        MoveRight,

        [Description("Jump")]
        Jump,

        [Description("Shoot")]
        Shoot
    }
}
