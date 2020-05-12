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
            Set(BosuRulesetSetting.EnableBackground, true);
            Set(BosuRulesetSetting.EnableHitboxes, false);
            Set(BosuRulesetSetting.PlayfieldDim, 0.5, 0, 1);
        }
    }

    public enum BosuRulesetSetting
    {
        EnableBackground,
        PlayfieldDim,
        EnableHitboxes
    }
}
