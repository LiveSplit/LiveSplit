using LiveSplit.Model.Comparisons;
using LiveSplit.UI;
using System;
using System.Drawing;

namespace LiveSplit.Model
{
    public static class LiveSplitStateHelper
    {
        /// <summary>
        /// Gets the last non-live delta in the run starting from splitNumber.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The split number to start checking deltas from.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the last non-live delta or null if there have been no deltas yet.</returns>
        public static TimeSpan? GetLastDelta(LiveSplitState state, int splitNumber, string comparison, TimingMethod method)
        {
            for (var x = splitNumber; x >= 0; x--)
            {
                if (state.Run[x].Comparisons[comparison][method] != null && state.Run[x].SplitTime[method] != null)
                    return state.Run[x].SplitTime[method] - state.Run[x].Comparisons[comparison][method];
            }
            return null;
        }

        private static TimeSpan? GetSegmentTimeOrSegmentDelta(LiveSplitState state, int splitNumber, bool useCurrentTime, bool segmentTime, string comparison, TimingMethod method)
        {
            TimeSpan? currentTime;
            if (useCurrentTime)
                currentTime = state.CurrentTime[method];
            else
                currentTime = state.Run[splitNumber].SplitTime[method];
            if (currentTime == null)
                return null;

            for (int x = splitNumber - 1; x >= 0; x--)
            {
                var splitTime = state.Run[x].SplitTime[method];
                if (splitTime != null)
                {
                    if (segmentTime)
                        return currentTime - splitTime;
                    else if (state.Run[x].Comparisons[comparison][method] != null)
                        return (currentTime - state.Run[splitNumber].Comparisons[comparison][method]) - (splitTime - state.Run[x].Comparisons[comparison][method]);
                }
            }

            if (segmentTime)
                return currentTime;
            else
                return currentTime - state.Run[splitNumber].Comparisons[comparison][method];
        }

        /// <summary>
        /// Gets the length of the last segment that leads up to a certain split.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The index of the split that represents the end of the segment.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the length of the segment leading up to splitNumber, returning null if the split is not completed yet.</returns>
        public static TimeSpan? GetPreviousSegmentTime(LiveSplitState state, int splitNumber, TimingMethod method)
        {
            return GetSegmentTimeOrSegmentDelta(state, splitNumber, false, true, Run.PersonalBestComparisonName, method);
        }

        /// <summary>
        /// Gets the length of the last segment that leads up to a certain split, using the live segment time if the split is not completed yet.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The index of the split that represents the end of the segment.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the length of the segment leading up to splitNumber, returning the live segment time if the split is not completed yet.</returns>
        public static TimeSpan? GetLiveSegmentTime(LiveSplitState state, int splitNumber, TimingMethod method)
        {
            return GetSegmentTimeOrSegmentDelta(state, splitNumber, true, true, Run.PersonalBestComparisonName, method);
        }

        /// <summary>
        /// Gets the amount of time lost or gained on a certain split.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The index of the split for which the delta is calculated.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the segment delta for a certain split, returning null if the split is not completed yet.</returns>
        public static TimeSpan? GetPreviousSegmentDelta(LiveSplitState state, int splitNumber, string comparison, TimingMethod method)
        {
            return GetSegmentTimeOrSegmentDelta(state, splitNumber, false, false, comparison, method);
        }

        /// <summary>
        /// Gets the amount of time lost or gained on a certain split, using the live segment delta if the split is not completed yet.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The index of the split for which the delta is calculated.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the segment delta for a certain split, returning the live segment delta if the split is not completed yet.</returns>
        public static TimeSpan? GetLiveSegmentDelta(LiveSplitState state, int splitNumber, string comparison, TimingMethod method)
        {
            return GetSegmentTimeOrSegmentDelta(state, splitNumber, true, false, comparison, method);
        }

        /// <summary>
        /// Checks whether the live segment should now be shown.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitDelta">Specifies whether to return a split delta rather than a segment delta and to start showing the live segment once you are behind.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the current live delta.</returns>
        public static TimeSpan? CheckLiveDelta(LiveSplitState state, bool splitDelta, string comparison, TimingMethod method)
        {
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                var useBestSegment = state.LayoutSettings.ShowBestSegments;
                var curSplit = state.Run[state.CurrentSplitIndex].Comparisons[comparison][method];
                var currentTime = state.CurrentTime[method];
                var curSegment = GetLiveSegmentTime(state, state.CurrentSplitIndex, method);
                var bestSegment = state.Run[state.CurrentSplitIndex].BestSegmentTime[method];
                var bestSegmentDelta = GetLiveSegmentDelta(state, state.CurrentSplitIndex, BestSegmentsComparisonGenerator.ComparisonName, method);
                var comparisonDelta = GetLiveSegmentDelta(state, state.CurrentSplitIndex, comparison, method);

                if (splitDelta && currentTime > curSplit
                    || useBestSegment && bestSegment != null && curSegment > bestSegment && bestSegmentDelta > TimeSpan.Zero
                    || comparisonDelta > TimeSpan.Zero)
                {
                    if (splitDelta)
                        return currentTime - curSplit;
                    return comparisonDelta;
                }
            }
            return null;
        }

        /// <summary>
        /// Chooses a split color from the Layout Settings based on the current run.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="timeDifference">The delta that you want to find a color for.</param>
        /// <param name="splitNumber">The split number that is associated with this delta.</param>
        /// <param name="showSegmentDeltas">Can show ahead gaining and behind losing colors if true.</param>
        /// <param name="showBestSegments">Can show the best segment color if true.</param>
        /// <param name="comparison">The comparison that you are comparing this delta to.</param>
        /// <param name="method">The timing method of this delta.</param>
        /// <returns>Returns the chosen color.</returns>
        public static Color? GetSplitColor(LiveSplitState state, TimeSpan? timeDifference, int splitNumber, bool showSegmentDeltas, bool showBestSegments, string comparison, TimingMethod method)
        {
            Color? splitColor = null;
            if (splitNumber < 0)
                return splitColor;

            if (timeDifference != null)
            {
                if (timeDifference < TimeSpan.Zero)
                {
                    splitColor = state.LayoutSettings.AheadGainingTimeColor;
                    var lastDelta = GetLastDelta(state, splitNumber - 1, comparison, method);
                    if (showSegmentDeltas && splitNumber > 0 && lastDelta != null && timeDifference > lastDelta)
                        splitColor = state.LayoutSettings.AheadLosingTimeColor;
                }
                else
                {
                    splitColor = state.LayoutSettings.BehindLosingTimeColor;
                    var lastDelta = GetLastDelta(state, splitNumber - 1, comparison, method);
                    if (showSegmentDeltas && splitNumber > 0 && lastDelta != null && timeDifference < lastDelta)
                        splitColor = state.LayoutSettings.BehindGainingTimeColor;
                }
            }

            if (showBestSegments && state.LayoutSettings.ShowBestSegments && CheckBestSegment(state, splitNumber, method))
            {
                splitColor = GetBestSegmentColor(state);
            }

            return splitColor;
        }

        /// <summary>
        /// Calculates whether or not the Split Times for the indicated split qualify as a Best Segment.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The split to check.</param>
        /// <param name="method">The timing method to use.</param>
        /// <returns>Returns whether or not the indicated split is a Best Segment.</returns>
        public static bool CheckBestSegment(LiveSplitState state, int splitNumber, TimingMethod method)
        {
            if (state.Run[splitNumber].SplitTime[method] == null)
                return false;
            var delta = GetPreviousSegmentDelta(state, splitNumber, BestSegmentsComparisonGenerator.ComparisonName, method);
            var curSegment = GetPreviousSegmentTime(state, splitNumber, method);
            var bestSegment = state.Run[splitNumber].BestSegmentTime[method];
            return bestSegment == null || curSegment < bestSegment || delta < TimeSpan.Zero;
        }

        private static Color GetBestSegmentColor(LiveSplitState state)
        {
            if (state.LayoutSettings.UseRainbowColor)
            {
                var hue = (((int)DateTime.Now.TimeOfDay.TotalMilliseconds / 100) % 36) * 10;
                var rainbowColor = ColorExtensions.FromHSV(hue, 1, 1);
                return Color.FromArgb((rainbowColor.R*2 + 255*1) / 3, (rainbowColor.G*2 + 255*1) / 3, (rainbowColor.B*2 + 255*1) / 3);
            }

            return state.LayoutSettings.BestSegmentColor;
        }
    }
}
