// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;
using System.ComponentModel;

namespace osu.Game.Rulesets.Bosu
{
    public class BosuInputManager : RulesetInputManager<BosuAction>
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
        Jump
    }
}
