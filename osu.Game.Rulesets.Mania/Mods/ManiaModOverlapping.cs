// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Mania.Mods
{
    public class ManiaModOverlapping : Mod, IApplicableToDrawableRuleset<ManiaHitObject>
    {
        public override string Name => "Overlapping";

        public override string Acronym => "OL";

        public override double ScoreMultiplier => 1.0;

        public override bool Ranked => false;

        public override LocalisableString Description => "Taiko SVs, but in mania";

        public override ModType Type => ModType.Fun;

        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[]
        {
            typeof(ManiaModConstantSpeed),
            typeof(ManiaModHybridScrolling)
        }).ToArray();

        public void ApplyToDrawableRuleset(DrawableRuleset<ManiaHitObject> drawableRuleset)
        {
            var maniaRuleset = (DrawableManiaRuleset)drawableRuleset;
            maniaRuleset.VisualisationMethod = ScrollVisualisationMethod.Overlapping;
        }
    }
}
