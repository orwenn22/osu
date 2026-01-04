// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Legacy;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModVariableApproachRate : Mod, IRequiresApproachCircles, IApplicableToBeatmap //IApplicableToDrawableHitObject
    {
        public override string Name => "Variable Approach Rate";
        public override string Acronym => "VAR";
        public override LocalisableString Description => "AR scales with bpm wow";
        public override double ScoreMultiplier => 1;

        public override Type[] IncompatibleMods => new[] { typeof(IHidesApproachCircles), typeof(OsuModFreezeFrame) };

        private double averageBpm = 120;
        private double averageBeatLength = 60000.0 / 120.0;

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            averageBeatLength = beatmap.GetMostCommonBeatLength();
            averageBpm = 60000 / averageBeatLength;

            foreach (var hitObject in beatmap.HitObjects)
            {
                if (hitObject is OsuHitObject osuHitObject)
                {
                    TimingControlPoint timingPoint = beatmap.ControlPointInfo.TimingPointAt(osuHitObject.StartTime);
                    double scrollSpeed = averageBpm / timingPoint.BPM;

                    //from OsuHitObject.ApplyDefaultsToSelf
                    osuHitObject.TimePreempt = (float)IBeatmapDifficultyInfo.DifficultyRange(beatmap.Difficulty.ApproachRate, OsuHitObject.PREEMPT_RANGE) * scrollSpeed; //TODO: might not take diff adjust into account
                    osuHitObject.TimeFadeIn = 400 * Math.Min(1, osuHitObject.TimePreempt / OsuHitObject.PREEMPT_MIN);
                    osuHitObject.Scale = LegacyRulesetExtensions.CalculateScaleFromCircleSize(beatmap.Difficulty.CircleSize, true); //TODO: might not take diff adjust into account
                }
            }
        }
    }
}
