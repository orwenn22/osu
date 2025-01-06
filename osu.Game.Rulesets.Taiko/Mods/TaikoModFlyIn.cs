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

        public override string Acronym => "FI";

        public override double ScoreMultiplier => 1;

        public override bool Ranked => false;

        public override LocalisableString Description => @"TODO : find a cool description";

        [SettingSource("Seed", "Use a custom seed instead of a random one", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> Seed { get; } = new Bindable<int?>();

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            var taikoBeatmap = (TaikoBeatmap)beatmap;

            Seed.Value ??= RNG.Next();
            var rng = new Random((int)Seed.Value);

            foreach (var obj in taikoBeatmap.HitObjects)
            {
                if (obj is Hit hit)
                    hit.OffTrack = (rng.Next() % 3) - 1;
            }
        }
    }
}
