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
            Set(BosuRulesetSetting.PlayerModel, PlayerModel.Bosu);
            Set(BosuRulesetSetting.Background, BackgroundType.None);
            Set(BosuRulesetSetting.PlayfieldDim, 0.5, 0, 1);
        }
    }

    public enum BosuRulesetSetting
    {
        PlayerModel,
        Background,
        PlayfieldDim,
    }

    public enum PlayerModel
    {
        Bosu,
        Boshy,
        Kid
    }

    public enum BackgroundType
    {
        None,
        Red,
        White
    }
}
