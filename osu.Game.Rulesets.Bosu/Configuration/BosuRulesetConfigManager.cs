using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Bosu.Configuration
{
    public class BosuRulesetConfigManager : RulesetConfigManager<BosuRulesetSetting>
    {
        public BosuRulesetConfigManager(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();
            SetValue(BosuRulesetSetting.TransparentBackground, false);
        }
    }

    public enum BosuRulesetSetting
    {
        TransparentBackground
    }
}
