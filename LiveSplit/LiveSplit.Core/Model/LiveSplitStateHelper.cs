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
        public static TimeSpan? GetLastDelta(LiveSplitState state, int splitNumber, String comparison, TimingMethod method)
        {
            for (int x = splitNumber; x >= 0; x--)
            {
                if (state.Run[x].Comparisons[comparison][method] != null && state.Run[x].SplitTime[method] != null)
                    return state.Run[x].SplitTime[method] - state.Run[x].Comparisons[comparison][method];
            }
            return null;
        }

        public static TimeSpan? GetPreviousSegment(LiveSplitState state, int splitNumber, bool liveSegment, bool currentSegment, bool loopThrough, String comparison, TimingMethod method)
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
                if (!loopThrough) return null;
            }
            if (currentSegment)
                return currentTime;
            else
                return currentTime - state.Run[splitNumber].Comparisons[comparison][method];
        }

        public static TimeSpan? CheckBestSegment(LiveSplitState state, bool isSegmentLabel, bool useBestSegment, String comparison, TimingMethod method)
        {
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? curSeg = GetPreviousSegment(state,state.CurrentSplitIndex, true, true, false, comparison, method);
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
                        !isSegmentLabel && state.CurrentTime[method] > state.Run[state.CurrentSplitIndex].Comparisons[comparison][method]))
                    return state.CurrentTime[method] - state.Run[state.CurrentSplitIndex].Comparisons[comparison][method];
            }
            return null;
        }

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
                    if ((segmentType == 0 && GetPreviousSegment(state, splitNumber, false, false, true, comparison, method) > TimeSpan.Zero)
                            || (segmentType == -1 && splitNumber > 0 && GetLastDelta(state, splitNumber - 1, comparison, method) != null && timeDifference > GetLastDelta(state, splitNumber - 1, comparison, method)))
                        splitColor = state.LayoutSettings.AheadLosingTimeColor;
                }
                else
                {
                    splitColor = state.LayoutSettings.BehindLosingTimeColor;
                    if (segmentType == 0 && GetPreviousSegment(state, splitNumber, false, false, true, comparison, method) < TimeSpan.Zero
                           || (segmentType == -1 && splitNumber > 0 && GetLastDelta(state, splitNumber - 1, comparison, method) != null && timeDifference < GetLastDelta(state, splitNumber - 1, comparison, method)))
                        splitColor = state.LayoutSettings.BehindGainingTimeColor;
                }
            }
             //Check for best segment
            if (segmentType != 2 && segmentType != -1 && state.LayoutSettings.ShowBestSegments)
            {
                TimeSpan? curSegment;
                curSegment = GetPreviousSegment(state,splitNumber, false, true, true, comparison, method);
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
