// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Catch.Judgements;
using osu.Game.Rulesets.Catch.Scoring;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Catch.Objects
{
    public class Fruit : PalpableCatchHitObject
    {
        public override Judgement CreateJudgement() => new CatchJudgement();

        protected override HitWindows CreateHitWindows() => new AltCatchHitWindow();

        public static FruitVisualRepresentation GetVisualRepresentation(int indexInBeatmap) => (FruitVisualRepresentation)(indexInBeatmap % 4);
    }
}
