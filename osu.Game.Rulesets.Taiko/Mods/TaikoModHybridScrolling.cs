// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.Taiko.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModHybridScrolling : Mod, IApplicableToDrawableRuleset<TaikoHitObject>
    {
        public override string Name => "Hybrid Scrolling";
        public override string Acronym => "HS";
        public override double ScoreMultiplier => 1.0;
        public override LocalisableString Description => "(only works with compatible maps)";
        public override ModType Type => ModType.Fun;
        public override bool Ranked => false;
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TaikoModConstantSpeed) }).ToArray();

        public void ApplyToDrawableRuleset(DrawableRuleset<TaikoHitObject> drawableRuleset)
        {
            var taikoRuleset = (DrawableTaikoRuleset)drawableRuleset;
            taikoRuleset.VisualisationMethod = ScrollVisualisationMethod.Hybrid;
        }
    }
}
