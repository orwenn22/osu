// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Mania.Mods
{
    public class ManiaModBait : Mod, IApplicableToBeatmap
    {
        public override string Name => "Bait";

        public override string Acronym => "BT";

        public override double ScoreMultiplier => 1.0;

        public override bool Ranked => false;

        public override LocalisableString Description => @"Get baited lmao";

        public override ModType Type => ModType.Fun;

        [SettingSource("Seed", "Use a custom seed instead of a random one", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> Seed { get; } = new Bindable<int?>();

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            var maniaBeatmap = (ManiaBeatmap)beatmap

            Seed.Value ??= RNG.Next();
            var rng = new Random((int)Seed.Value);

            foreach (var n in beatmap.HitObjects.OfType<Note>())
            {
                n.ColumnOffset = rng.Next(-1, 1);

                int offsetedColumn = n.Column + n.ColumnOffset;

                if (offsetedColumn < 0 || offsetedColumn >= maniaBeatmap.TotalColumns)
                {
                    n.ColumnOffset = 0;
                }
            }
        }
    }
}
