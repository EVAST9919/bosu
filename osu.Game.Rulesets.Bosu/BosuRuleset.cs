using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using System;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Bosu.Scoring;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Bosu.Difficulty;
using osu.Game.Rulesets.Bosu.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Bosu.Mods;
using osu.Game.Rulesets.Configuration;
using osu.Game.Configuration;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Bosu.Replays;

namespace osu.Game.Rulesets.Bosu
{
    public class BosuRuleset : Ruleset
    {
        public BosuHealthProcessor HealthProcessor { get; private set; }

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableBosuRuleset(this, beatmap, mods);

        public override ScoreProcessor CreateScoreProcessor() => new BosuScoreProcessor();

        public override HealthProcessor CreateHealthProcessor(double drainStartTime) => HealthProcessor = new BosuHealthProcessor();

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new BosuBeatmapConverter(beatmap, this);

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new BosuReplayFrame();

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Left, BosuAction.MoveLeft),
            new KeyBinding(InputKey.Right, BosuAction.MoveRight),
            new KeyBinding(InputKey.Shift, BosuAction.Jump),
            new KeyBinding(InputKey.Z, BosuAction.Shoot),
        };

        public override IEnumerable<Mod> ConvertFromLegacyMods(LegacyMods mods)
        {
            if (mods.HasFlag(LegacyMods.Nightcore))
                yield return new BosuModNightcore();
            else if (mods.HasFlag(LegacyMods.DoubleTime))
                yield return new BosuModDoubleTime();

            if (mods.HasFlag(LegacyMods.SuddenDeath))
                yield return new BosuModSuddenDeath();

            if (mods.HasFlag(LegacyMods.NoFail))
                yield return new BosuModNoFail();

            if (mods.HasFlag(LegacyMods.HalfTime))
                yield return new BosuModHalfTime();

            if (mods.HasFlag(LegacyMods.Easy))
                yield return new BosuModEasy();

            if (mods.HasFlag(LegacyMods.Relax))
                yield return new BosuModRelax();

            if (mods.HasFlag(LegacyMods.Hidden))
                yield return new BosuModHidden();

            if (mods.HasFlag(LegacyMods.Flashlight))
                yield return new BosuModFlashlight();

            if (mods.HasFlag(LegacyMods.Cinema))
                yield return new BosuModCinema();
            else if (mods.HasFlag(LegacyMods.Autoplay))
                yield return new BosuModAutoplay();
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new BosuModEasy(),
                        new BosuModNoFail(),
                        new MultiMod(new BosuModHalfTime(), new BosuModDaycore())
                    };

                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new BosuModSuddenDeath(),
                        new MultiMod(new BosuModDoubleTime(), new BosuModNightcore()),
                        new BosuModHidden(),
                        new BosuModFlashlight()
                    };

                case ModType.Automation:
                    return new Mod[]
                    {
                        new MultiMod(new BosuModAutoplay(), new BosuModCinema()),
                        new BosuModRelax(),
                    };

                case ModType.Fun:
                    return new Mod[]
                    {
                        new MultiMod(new ModWindUp(), new ModWindDown()),
                        new BosuModZoom(),
                        new BosuModCustomMap()
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string Description => "bosu!";

        public override string ShortName => "bosu!";

        public override string PlayingVerb => "Avoiding apples";

        public override Drawable CreateIcon() => new Sprite
        {
            Texture = new TextureStore(new TextureLoaderStore(CreateResourceStore()), false).Get("Textures/logo"),
        };

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new BosuDifficultyCalculator(this, beatmap);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new BosuRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings() => new BosuSettingsSubsection(this);
    }
}
