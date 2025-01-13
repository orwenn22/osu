// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Lists;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Timing;

namespace osu.Game.Rulesets.UI.Scrolling.Algorithms
{
    public class HybridScrollAlgorithm : IScrollAlgorithm
    {
        private readonly SortedList<MultiplierControlPoint> controlPoints;

        private readonly IScrollAlgorithm overlappingAlgorithm;
        private readonly IScrollAlgorithm sequentialAlgorithm;

        public HybridScrollAlgorithm(SortedList<MultiplierControlPoint> controlPoints)
        {
            Console.WriteLine("Made HybridScrollAlgorithm");
            this.controlPoints = controlPoints;
            overlappingAlgorithm = new OverlappingScrollAlgorithm(controlPoints);
            sequentialAlgorithm = new SequentialScrollAlgorithm(controlPoints);
            //sequentialAlgorithm = new SequentialScrollAlgorithm(controlPoints);
        }

        public double GetDisplayStartTime(double originTime, float offset, double timeRange, float scrollLength)
        {
            var controlPoint = controlPointAt(originTime);
            return getScrollingAlgorithm(controlPoint).GetDisplayStartTime(originTime, offset, timeRange, scrollLength);
        }

        public float GetLength(double startTime, double endTime, double timeRange, float scrollLength)
        {
            //TODO : it is possible that the end of the note ends up after another control point. In that case it would be necessary to do more complex calculations to find the correct length.
            var controlPointStart = controlPointAt(startTime);
            //var controlPointEnd = controlPointAt(endTime);

            return getScrollingAlgorithm(controlPointStart).GetLength(startTime, endTime, timeRange, scrollLength);
        }

        public float PositionAt(double time, double currentTime, double timeRange, float scrollLength, double? originTime = null)
        {
            var controlPoint = controlPointAt(time);
            return getScrollingAlgorithm(controlPoint).PositionAt(time, currentTime, timeRange, scrollLength, originTime);
        }

        public void Reset()
        {
            overlappingAlgorithm.Reset();
            sequentialAlgorithm.Reset();
        }

        public double TimeAt(float position, double currentTime, double timeRange, float scrollLength)
        {
            //TODO : actually compute the time correctly instead of assuming overlapping scrolling.
            //       this is (probably) required to be able to place notes in the editor, if we ever want to have an hybrid scroll preview.
            Debug.Assert(controlPoints.Count > 0);
            return overlappingAlgorithm.TimeAt(position, currentTime, timeRange, scrollLength);
        }

        private IScrollAlgorithm getScrollingAlgorithm(MultiplierControlPoint controlPoint)
        {
            return controlPoint.EffectPoint.ScrollMode == EffectControlPointScrollMode.Overlapping ? overlappingAlgorithm : sequentialAlgorithm;
        }

        /// <summary>
        /// Finds the <see cref="MultiplierControlPoint"/> which affects the speed of hitobjects at a specific time.
        /// </summary>
        /// <param name="time">The time which the <see cref="MultiplierControlPoint"/> should affect.</param>
        /// <returns>The <see cref="MultiplierControlPoint"/>.</returns>
        private MultiplierControlPoint controlPointAt(double time)
        {
            return ControlPointInfo.BinarySearch(controlPoints, time)
                   // The standard binary search will fail if there's no control points, or if the time is before the first.
                   // For this method, we want to use the first control point in the latter case.
                   ?? controlPoints.FirstOrDefault()
                   ?? new MultiplierControlPoint(double.NegativeInfinity);
        }
    }
}
