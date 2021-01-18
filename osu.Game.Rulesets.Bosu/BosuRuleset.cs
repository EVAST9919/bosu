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
using osu.Game.Rulesets.Bosu.Difficulty;
using osu.Game.Rulesets.Bosu.Beatmaps;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Bosu.Mods;
using osu.Game.Rulesets.Bosu.Replays;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Bosu.Edit;
using osu.Game.Rulesets.Configuration;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Bosu.Configuration;

namespace osu.Game.Rulesets.Bosu
{
    public class BosuRuleset : Ruleset
    {
        private DrawableBosuRuleset drawableRuleset;

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => drawableRuleset = new DrawableBosuRuleset(this, beatmap, mods);

        public override HealthProcessor CreateHealthProcessor(double drainStartTime)
        {
            var hp = new BosuHealthProcessor();
            drawableRuleset.HealthProcessor = hp;
            return hp;
        }

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new BosuBeatmapConverter(beatmap, this);

        // Not sure why overriding bp makes combo colours work
        public override IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => new BosuBeatmapProcessor(beatmap);

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new BosuReplayFrame();

        public override HitObjectComposer CreateHitObjectComposer() => new BosuHitObjectComposer(this);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new BosuRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings() => new BosuSettingsSubsection(this);

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Left, BosuAction.MoveLeft),
            new KeyBinding(InputKey.Right, BosuAction.MoveRight),
            new KeyBinding(InputKey.Shift, BosuAction.Jump),
            new KeyBinding(InputKey.Z, BosuAction.Shoot),
        };

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
                        new BosuModHidden(),
                        new MultiMod(new BosuModDoubleTime(), new BosuModNightcore())
                    };

                case ModType.Automation:
                    return new Mod[]
                    {
                        new MultiMod(new BosuModAutoplay(), new BosuModCinema()),
                    };

                case ModType.Fun:
                    return new Mod[]
                    {
                        new MultiMod(new ModWindUp(), new ModWindDown())
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string Description => "bosu!";

        public override string ShortName => "bosu!";

        public override string PlayingVerb => "Avoiding cherries";

        public override Drawable CreateIcon() => new Sprite
        {
            Texture = new TextureStore(new TextureLoaderStore(CreateResourceStore()), false).Get("Textures/logo"),
        };

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new BosuDifficultyCalculator(this, beatmap);

        protected override IEnumerable<HitResult> GetValidHitResults() => new[]
        {
            HitResult.Perfect
        };
    }
}
