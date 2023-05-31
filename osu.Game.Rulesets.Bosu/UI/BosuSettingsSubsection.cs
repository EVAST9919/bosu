using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Bosu.Configuration;

namespace osu.Game.Rulesets.Bosu.UI
{
    public partial class BosuSettingsSubsection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header => "bosu!";

        public BosuSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (BosuRulesetConfigManager)Config;

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Transparent background",
                    Current = config.GetBindable<bool>(BosuRulesetSetting.TransparentBackground)
                }
            };
        }
    }
}
