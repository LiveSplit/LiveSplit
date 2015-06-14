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
            for (int x = splitNumber; x >= 0; x--)
            {
                if (state.Run[x].Comparisons[comparison][method] != null && state.Run[x].SplitTime[method] != null)
                    return state.Run[x].SplitTime[method] - state.Run[x].Comparisons[comparison][method];
            }
            return null;
        }

        private static TimeSpan? GetSegmentTimeOrSegmentDelta(LiveSplitState state, int splitNumber, bool useCurrentTime, bool segmentTime, string comparison, TimingMethod method)
        {
            if (!useCurrentTime && (state.Run[splitNumber].SplitTime[method] == null)) return null;
            TimeSpan? currentTime;
            if (useCurrentTime == false)
                currentTime = state.Run[splitNumber].SplitTime[method];
            else
                currentTime = state.CurrentTime[method];
            for (int x = splitNumber - 1; x >= 0; x--)
            {
                if (state.Run[x].SplitTime[method] != null)
                {
                    if (segmentTime)
                        return currentTime - state.Run[x].SplitTime[method];
                    else if (state.Run[x].Comparisons[comparison][method] != null)
                        return (currentTime - state.Run[splitNumber].Comparisons[comparison][method]) - (state.Run[x].SplitTime[method] - state.Run[x].Comparisons[comparison][method]);
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
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the length of the segment leading up to splitNumber, returning null if the split is not completed yet.</returns>
        public static TimeSpan? GetPreviousSegmentTime(LiveSplitState state, int splitNumber, string comparison, TimingMethod method)
        {
            return GetSegmentTimeOrSegmentDelta(state, splitNumber, false, true, comparison, method);
        }

        /// <summary>
        /// Gets the length of the last segment that leads up to a certain split, using the live segment time if the split is not completed yet.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The index of the split that represents the end of the segment.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the length of the segment leading up to splitNumber, returning the live segment time if the split is not completed yet.</returns>
        public static TimeSpan? GetCurrentSegmentTime(LiveSplitState state, int splitNumber, string comparison, TimingMethod method)
        {
            return GetSegmentTimeOrSegmentDelta(state, splitNumber, true, true, comparison, method);
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
        /// <param name="showWhenBehind">Specifies whether or not to start showing the live segment once you are behind.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are using.</param>
        /// <returns>Returns the current live delta.</returns>
        public static TimeSpan? CheckLiveDelta(LiveSplitState state, bool showWhenBehind, string comparison, TimingMethod method)
        {
            var useBestSegment = state.LayoutSettings.ShowBestSegments;
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? curSeg = state.CurrentTime[method] - (state.CurrentSplitIndex > 0 ? state.Run[state.CurrentSplitIndex - 1].SplitTime[method] : TimeSpan.Zero);
                var bestSegmentDifference = (-state.Run[state.CurrentSplitIndex].Comparisons[comparison][method]
                    + ((state.CurrentSplitIndex - 1 >= 0) ? state.Run[state.CurrentSplitIndex - 1].Comparisons[comparison][method] : TimeSpan.Zero))
                    + state.Run[state.CurrentSplitIndex].BestSegmentTime[method];
                if (bestSegmentDifference == null || bestSegmentDifference > TimeSpan.Zero || !useBestSegment)
                    bestSegmentDifference = TimeSpan.Zero;

                var lastDelta = GetLastDelta(state, state.CurrentSplitIndex - 1, comparison, method);
                if (lastDelta == null)
                    lastDelta = TimeSpan.Zero;

                if (state.Run[state.CurrentSplitIndex].Comparisons[comparison][method] != null &&
                        ((state.CurrentTime[method] > state.Run[state.CurrentSplitIndex].Comparisons[comparison][method] + lastDelta + bestSegmentDifference) ||
                        (useBestSegment && curSeg != null && curSeg > state.Run[state.CurrentSplitIndex].BestSegmentTime[method]) ||
                        !showWhenBehind && state.CurrentTime[method] > state.Run[state.CurrentSplitIndex].Comparisons[comparison][method]))
                    return state.CurrentTime[method] - state.Run[state.CurrentSplitIndex].Comparisons[comparison][method];
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

            if (showBestSegments && state.LayoutSettings.ShowBestSegments)
            {
                TimeSpan? curSegment;
                curSegment = GetPreviousSegmentTime(state, splitNumber, comparison, method);
                if (curSegment != null)
                {
                    if (state.Run[splitNumber].BestSegmentTime[method] == null || curSegment < state.Run[splitNumber].BestSegmentTime[method])
                        splitColor = state.LayoutSettings.BestSegmentColor;
                }
            }
            return splitColor;
        }
    }
}
