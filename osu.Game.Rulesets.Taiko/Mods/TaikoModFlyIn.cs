// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Taiko.Beatmaps;
using osu.Game.Rulesets.Taiko.Objects;

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModFlyIn : Mod, IApplicableToBeatmap
    {
        public override string Name => "Fly In";

        public override string Acronym => "FLY";

        public override double ScoreMultiplier => 1;

        public override bool Ranked => false;

        public override LocalisableString Description => @"TODO : find a cool description";

        [SettingSource("Seed", "Use a custom seed instead of a random one", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> Seed { get; } = new Bindable<int?>();

        [SettingSource("Source count", "amount of hit source")]
        public BindableNumber<int> SourceCount { get; } = new BindableInt(3)
        {
            MinValue = 2,
            MaxValue = 120,
            Precision = 1,
        };

        [SettingSource("Flip", "Flip the sources vertically", SettingControlType = typeof(SettingsCheckbox))]
        public Bindable<bool> Flip { get; } = new BindableBool(false);

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            var taikoBeatmap = (TaikoBeatmap)beatmap;

            int sourceCount = SourceCount.Value;
            bool flip = Flip.Value;
            float offTrackCoefficient = 2f / (SourceCount.Value - SourceCount.Value % 2);

            // Setup random number generator
            Seed.Value ??= RNG.Next();
            var rng = new Random((int)Seed.Value);

            foreach (var obj in taikoBeatmap.HitObjects)
            {
                if (obj is Hit hit)
                {
                    // Select a lane index for the hit
                    int sourceIndex = (rng.Next() % sourceCount) - sourceCount / 2;

                    // Multiply with coefficient
                    hit.OffTrack = sourceIndex * offTrackCoefficient * (flip ? -1 : 1);
                }
            }
        }
    }
}
