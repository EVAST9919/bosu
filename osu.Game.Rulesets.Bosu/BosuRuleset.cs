// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

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

namespace osu.Game.Rulesets.Bosu
{
    public class BosuRuleset : Ruleset
    {
        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableBosuRuleset(this, beatmap, mods);

        public override ScoreProcessor CreateScoreProcessor() => new BosuScoreProcessor();

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new BosuBeatmapConverter(beatmap, this);

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Left, BosuAction.MoveLeft),
            new KeyBinding(InputKey.Right, BosuAction.MoveRight),
            new KeyBinding(InputKey.Space, BosuAction.Jump),
        };

        public override IEnumerable<Mod> ConvertFromLegacyMods(LegacyMods mods)
        {
            if (mods.HasFlag(LegacyMods.SuddenDeath))
                yield return new BosuModSuddenDeath();
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new BosuModSuddenDeath()
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
    }
}
