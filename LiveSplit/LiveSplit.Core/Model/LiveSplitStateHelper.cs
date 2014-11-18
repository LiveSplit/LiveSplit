using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.Model
{
    public static class LiveSplitStateHelper
    {
        /// <summary>
        /// Gets the last non-live delta in the run starting from SplitNumber.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The split number to start checking deltas from.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are comparing with.</param>
        /// <returns>Returns the last non-live delta or null if there have been no deltas yet.</returns>
        public static TimeSpan? GetLastDelta(LiveSplitState state, int splitNumber, String comparison, TimingMethod method)
        {
            for (int x = splitNumber; x >= 0; x--)
            {
                if (state.Run[x].Comparisons[comparison][method] != null && state.Run[x].SplitTime[method] != null)
                    return state.Run[x].SplitTime[method] - state.Run[x].Comparisons[comparison][method];
            }
            return null;
        }

        /// <summary>
        /// Gets the current segment or delta for a specified split.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="splitNumber">The split to calculate the current segment for.</param>
        /// <param name="liveSegment">Uses the current time if true; the split time if false.</param>
        /// <param name="currentSegment">Returns the segment time if true; the delta time if false</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are comparing with.</param>
        /// <returns>Returns the current segment or delta time.</returns>
        public static TimeSpan? GetPreviousSegment(LiveSplitState state, int splitNumber, bool liveSegment, bool currentSegment, String comparison, TimingMethod method)
        {
            if (!liveSegment && (state.Run[splitNumber].SplitTime[method] == null)) return null;
            TimeSpan? currentTime;
            if (liveSegment == false)
                currentTime = state.Run[splitNumber].SplitTime[method];
            else
                currentTime = state.CurrentTime[method];
            for (int x = splitNumber - 1; x >= 0; x--)
            {
                if (state.Run[x].SplitTime[method] != null)
                {
                    if (currentSegment)
                        return currentTime - state.Run[x].SplitTime[method];
                    else if (state.Run[x].Comparisons[comparison][method] != null)
                        return (currentTime - state.Run[splitNumber].Comparisons[comparison][method]) - (state.Run[x].SplitTime[method] - state.Run[x].Comparisons[comparison][method]);
                }
            }
            if (currentSegment)
                return currentTime;
            else
                return currentTime - state.Run[splitNumber].Comparisons[comparison][method];
        }

        /// <summary>
        /// Checks whether the live segment should now be shown.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="showWhenBehind">Specifies whether or not to start showing the live segment once you are behind.</param>
        /// <param name="comparison">The comparison that you are comparing with.</param>
        /// <param name="method">The timing method that you are comparing with.</param>
        /// <returns>Returns the current live delta.</returns>
        public static TimeSpan? CheckLiveDelta(LiveSplitState state, bool showWhenBehind, String comparison, TimingMethod method)
        {
            var useBestSegment = state.LayoutSettings.ShowBestSegments;
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? curSeg = GetPreviousSegment(state,state.CurrentSplitIndex, true, true, comparison, method);
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
        /// <param name="segmentType">The type of logic that should be applied:
        /// -1 - Allow ahead losing and behind gaining but do not show best segments.
        /// 0  - Allow ahead losing and behind gaining and show best segments.
        /// 1  - Only check for best segments.
        /// 2  - Only show ahead or behind, nothing more.</param>
        /// <param name="splitNumber">The split number that is associated with this delta.</param>
        /// <param name="comparison">The comparison that you are comparing this delta to.</param>
        /// <param name="method">The timing method that you are comparing this delta to.</param>
        /// <returns>Returns the chosen split color.</returns>
        public static Color? GetSplitColor(LiveSplitState state, TimeSpan? timeDifference, int segmentType, int splitNumber, String comparison, TimingMethod method)
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
                    if (segmentType <= 0 && splitNumber > 0 && lastDelta != null && timeDifference > lastDelta)
                        splitColor = state.LayoutSettings.AheadLosingTimeColor;
                }
                else
                {
                    splitColor = state.LayoutSettings.BehindLosingTimeColor;
                    var lastDelta = GetLastDelta(state, splitNumber - 1, comparison, method);
                    if (segmentType <= 0 && splitNumber > 0 && lastDelta != null && timeDifference < lastDelta)
                        splitColor = state.LayoutSettings.BehindGainingTimeColor;
                }
            }

            if (segmentType != 2 && segmentType != -1 && state.LayoutSettings.ShowBestSegments)
            {
                TimeSpan? curSegment;
                curSegment = GetPreviousSegment(state,splitNumber, false, true, comparison, method);
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
