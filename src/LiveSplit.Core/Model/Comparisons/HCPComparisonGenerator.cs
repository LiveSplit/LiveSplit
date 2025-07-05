using System;
using System.Collections.Generic;
using System.Linq;

using LiveSplit.Options;

namespace LiveSplit.Model.Comparisons;

public class HCPComparisonGenerator : IComparisonGenerator
{
    public IRun Run { get; set; }
    public const string ComparisonName = "Golf HCP";
    public const string ShortComparisonName = "HCP";
    public string Name => ComparisonName;
    private int _numberOfLatestRunsToInclude = 20;
    private int _maximumNumberOfBestRunsToInclude = 8;
    
    // Reusable collections to minimize allocations
    private readonly List<double> _totalTimes;
    private readonly List<List<double>> _segmentTimesPerRun;
    private readonly List<double> _tempSegmentTimes;
    private readonly List<int> _bestRunIndices;

    public HCPComparisonGenerator(IRun run)
    {
        Run = run;
        _totalTimes = new(_numberOfLatestRunsToInclude);
        _segmentTimesPerRun = new(_numberOfLatestRunsToInclude);
        _tempSegmentTimes = new(_numberOfLatestRunsToInclude);
        _bestRunIndices = new(_maximumNumberOfBestRunsToInclude);
    }

    [Obsolete]
    protected TimeSpan CalculateAverage(IEnumerable<TimeSpan> curList)
    {
        double averageTime = curList.OrderBy(x => x.TotalSeconds).Take(_maximumNumberOfBestRunsToInclude).Average(x => x.TotalSeconds);
        return TimeSpan.FromTicks((long)(averageTime * TimeSpan.TicksPerSecond));
    }

    private double CalculateAverage(List<double> times, int startIndex, int count)
    {
        if (count == 0)
        {
            return 0;
        }

        return times.Skip(startIndex)
                    .Take(count)
                    .Average();
    }

    public void Generate(TimingMethod method)
    {
        int segmentCount = Run.Count;
        if (segmentCount == 0)
        {
            return;
        }

        // Reset collections
        _totalTimes.Clear();
        if (_totalTimes.Capacity < _numberOfLatestRunsToInclude)
        {
            _totalTimes.Capacity = _numberOfLatestRunsToInclude;
        }

        _segmentTimesPerRun.Clear();
        if (_segmentTimesPerRun.Capacity < _numberOfLatestRunsToInclude)
        {
            _segmentTimesPerRun.Capacity = _numberOfLatestRunsToInclude;
        }

        _tempSegmentTimes.Clear();
        if (_tempSegmentTimes.Capacity < _numberOfLatestRunsToInclude)
        {
            _tempSegmentTimes.Capacity = _numberOfLatestRunsToInclude;
        }

        _bestRunIndices.Clear();
        if (_bestRunIndices.Capacity < _maximumNumberOfBestRunsToInclude)
        {
            _bestRunIndices.Capacity = _maximumNumberOfBestRunsToInclude;
        }

        var validSegmentTimes = new Dictionary<int, List<double>>(segmentCount);

        // Reset all segment comparisons
        for (int i = 0; i < segmentCount; i++)
        {
            var time = new Time(Run[i].Comparisons[Name]);
            time[method] = null;
            Run[i].Comparisons[Name] = time;

            // Initialize segment times collection
            if (!validSegmentTimes.ContainsKey(i))
            {
                validSegmentTimes[i] = new List<double>(_numberOfLatestRunsToInclude);
            }
            else
            {
                validSegmentTimes[i].Clear();
            }
        }

        // Find attempts with a valid last segment time
        IList<Attempt> attemptHistory = Run.AttemptHistory;
        int attemptCount = attemptHistory.Count;
        int lastSegmentIndex = segmentCount - 1;
        ISegment lastSegment = Run[lastSegmentIndex];

        // Collect run data
        var runData = new List<(int AttemptIndex, double TotalTime, List<double> SegmentTimes)>();

        // Get the most recent valid attempts
        int validAttemptsFound = 0;
        for (int i = attemptCount - 1; i >= 0 && validAttemptsFound < _numberOfLatestRunsToInclude; i--)
        {
            Attempt attempt = attemptHistory[i];

            // Check if the run was finished
            if (!lastSegment.SegmentHistory.TryGetValue(attempt.Index, out Time lastSegmentTime) ||
                lastSegmentTime[method] == null)
            {
                continue;
            }

            // Use the attempt's official time as the total run time
            double totalTime = 0;
            if (attempt.Time[method] != null)
            {
                totalTime = attempt.Time[method].Value.TotalSeconds;
            }
            else if (lastSegmentTime[method] != null)
            {
                totalTime = lastSegmentTime[method].Value.TotalSeconds;
            }
            else
            {
                continue; // Skip this attempt if no valid time is available
            }

            if (totalTime <= 0)
            {
                continue;
            }

            _totalTimes.Add(totalTime);

            // Collect segment times for this attempt
            var segmentTimes = new List<double>(segmentCount);
            bool hasValidSegments = false;

            for (int j = 0; j < segmentCount; j++)
            {
                double segmentTime = 0;

                if (Run[j].SegmentHistory.TryGetValue(attempt.Index, out var segmentHistory) &&
                    segmentHistory[method] != null)
                {
                    segmentTime = segmentHistory[method].Value.TotalSeconds;

                    if (segmentTime > 0)
                    {
                        validSegmentTimes[j].Add(segmentTime);
                        hasValidSegments = true;
                    }
                }

                segmentTimes.Add(segmentTime);
            }

            if (hasValidSegments)
            {
                runData.Add((attempt.Index, totalTime, segmentTimes));
                _segmentTimesPerRun.Add(segmentTimes);
                validAttemptsFound++;
            }
        }

        if (_totalTimes.Count == 0)
        {
            return;
        }

        var totalTimeWithIndices = _totalTimes
            .Select((time, index) => (Index: index, Time: time))
            .OrderBy(t => t.Time)
            .Take(_maximumNumberOfBestRunsToInclude)
            .ToList();

        // Sort by total time
        totalTimeWithIndices.Sort((a, b) => a.Time.CompareTo(b.Time));
        _bestRunIndices.Clear();
        _bestRunIndices.AddRange(totalTimeWithIndices.Select(t => t.Index).ToList());

        // Calculate average total time from best runs (LINQ version)
        int bestRunsCount = _bestRunIndices.Count;
        double avgTotal = bestRunsCount > 0
            ? _bestRunIndices.Average(index => _totalTimes[index])
            : 0;

        if (avgTotal <= 0)
        {
            return;
        }

        // Calculate HCP times for each segment using the same best runs
        double[] segmentHCPTimes = new double[segmentCount];
        bool[] hasValidTimes = new bool[segmentCount];

        for (int segIndex = 0; segIndex < segmentCount; segIndex++)
        {
            var bestSegmentTimes = _bestRunIndices
                .Where(runIndex => runIndex < _segmentTimesPerRun.Count)
                .Select(runIndex => _segmentTimesPerRun[runIndex][segIndex])
                .Where(segTime => segTime > 0)
                .ToList();

            if (bestSegmentTimes.Count > 0)
            {
                // Calculate average for this segment from best runs
                segmentHCPTimes[segIndex] = bestSegmentTimes.Average();
                hasValidTimes[segIndex] = true;
            }
            else
            {
                // Alternative method: Calculate based on all valid times for this segment
                List<double> allTimes = validSegmentTimes[segIndex];
                if (allTimes.Count > 0)
                {
                    _tempSegmentTimes.Clear();
                    _tempSegmentTimes.AddRange(allTimes);
                    _tempSegmentTimes.Sort();

                    int timesToUse = Math.Min(allTimes.Count, _maximumNumberOfBestRunsToInclude);
                    segmentHCPTimes[segIndex] = CalculateAverage(_tempSegmentTimes, 0, timesToUse);
                    hasValidTimes[segIndex] = segmentHCPTimes[segIndex] > 0;
                }
                else
                {
                    hasValidTimes[segIndex] = false;
                }
            }
        }

        // Calculate total of all segment HCP times
        double totalHCPSegmentTime = segmentHCPTimes.Where(time => time > 0).Sum();

        // If we have valid segment times, scale them to match total HCP time
        if (totalHCPSegmentTime > 0)
        {
            TimeSpan? cumulative = TimeSpan.Zero;

            // Scale factor to ensure segments add up to total HCP time
            double scaleFactor = avgTotal / totalHCPSegmentTime;

            for (int i = 0; i < segmentCount; i++)
            {
                if (hasValidTimes[i])
                {
                    // Scale individual segment HCP by the total ratio
                    var segmentDuration = TimeSpan.FromSeconds(segmentHCPTimes[i] * scaleFactor);

                    if (cumulative != null)
                    {
                        cumulative += segmentDuration;
                    }

                    var time = new Time(Run[i].Comparisons[Name]);
                    time[method] = cumulative;
                    Run[i].Comparisons[Name] = time;
                }
            }
        }
    }

    public void Generate(ISettings settings)
    {
        _numberOfLatestRunsToInclude = settings.HcpHistorySize;
        _maximumNumberOfBestRunsToInclude = settings.HcpNBestRuns;
        Generate(TimingMethod.RealTime);
        Generate(TimingMethod.GameTime);
    }
}
