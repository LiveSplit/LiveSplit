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
        // Get the number of segments in the run
        int segmentCount = Run.Count;
        if (segmentCount == 0)
        {
            // If there are no segments, nothing to generate
            return;
        }

        // Clear reusable collections for this pass
        ResetReusableCollections();

        // Initialize per-segment collections and reset segment comparison values
        var validSegmentTimes = InitializeSegmentTimes(segmentCount, method);

        // Gather valid attempts and their segment times, populating collections
        var runData = CollectValidRuns(method, segmentCount, validSegmentTimes);

        // If no valid runs were found, exit early
        if (_totalTimes.Count == 0)
        {
            return;
        }

        // Select the best runs (lowest total times) for HCP calculation
        SelectBestRuns();

        // Calculate the average total time from the best runs
        int bestRunsCount = _bestRunIndices.Count;
        double avgTotal = bestRunsCount > 0
            ? _bestRunIndices.Average(index => _totalTimes[index])
            : 0;

        // If the average is not positive, exit early
        if (avgTotal <= 0)
        {
            return;
        }

        // Calculate HCP times for each segment and track which segments have valid data
        (double[] segmentHCPTimes, bool[] hasValidTimes) = CalculateSegmentHCPs(segmentCount, validSegmentTimes);

        // Sum the HCP times for all valid segments
        double totalHCPSegmentTime = segmentHCPTimes.Where(time => time > 0).Sum();

        // If there are valid segment times, scale and apply them to the run's comparison data
        if (totalHCPSegmentTime > 0)
        {
            ApplyHCPComparisons(segmentCount, avgTotal, totalHCPSegmentTime, segmentHCPTimes, hasValidTimes, method);
        }
    }

    private void ResetReusableCollections()
    {
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
    }

    private Dictionary<int, List<double>> InitializeSegmentTimes(int segmentCount, TimingMethod method)
    {
        var validSegmentTimes = new Dictionary<int, List<double>>(segmentCount);
        for (int i = 0; i < segmentCount; i++)
        {
            var time = new Time(Run[i].Comparisons[Name]);
            time[method] = null;
            Run[i].Comparisons[Name] = time;

            validSegmentTimes[i] = new List<double>(_numberOfLatestRunsToInclude);
        }

        return validSegmentTimes;
    }

    private List<(int AttemptIndex, double TotalTime, List<double> SegmentTimes)> CollectValidRuns(
        TimingMethod method,
        int segmentCount,
        Dictionary<int, List<double>> validSegmentTimes)
    {
        IList<Attempt> attemptHistory = Run.AttemptHistory;
        int attemptCount = attemptHistory.Count;
        int lastSegmentIndex = segmentCount - 1;
        ISegment lastSegment = Run[lastSegmentIndex];

        var runData = new List<(int AttemptIndex, double TotalTime, List<double> SegmentTimes)>();
        int validAttemptsFound = 0;
        for (int i = attemptCount - 1; i >= 0 && validAttemptsFound < _numberOfLatestRunsToInclude; i--)
        {
            Attempt attempt = attemptHistory[i];

            if (!lastSegment.SegmentHistory.TryGetValue(attempt.Index, out Time lastSegmentTime) ||
                lastSegmentTime[method] == null)
            {
                continue;
            }

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
                continue;
            }

            if (totalTime <= 0)
            {
                continue;
            }

            _totalTimes.Add(totalTime);

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

        return runData;
    }

    private void SelectBestRuns()
    {
        var totalTimeWithIndices = _totalTimes
            .Select((time, index) => (Index: index, Time: time))
            .OrderBy(t => t.Time)
            .Take(_maximumNumberOfBestRunsToInclude)
            .ToList();

        totalTimeWithIndices.Sort((a, b) => a.Time.CompareTo(b.Time));
        _bestRunIndices.Clear();
        _bestRunIndices.AddRange(totalTimeWithIndices.Select(t => t.Index));
    }

    private (double[] segmentHCPTimes, bool[] hasValidTimes) CalculateSegmentHCPs(
        int segmentCount,
        Dictionary<int, List<double>> validSegmentTimes)
    {
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
                segmentHCPTimes[segIndex] = bestSegmentTimes.Average();
                hasValidTimes[segIndex] = true;
            }
            else
            {
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

        return (segmentHCPTimes, hasValidTimes);
    }

    private void ApplyHCPComparisons(
        int segmentCount,
        double avgTotal,
        double totalHCPSegmentTime,
        double[] segmentHCPTimes,
        bool[] hasValidTimes,
        TimingMethod method)
    {
        TimeSpan? cumulative = TimeSpan.Zero;
        double scaleFactor = avgTotal / totalHCPSegmentTime;

        for (int i = 0; i < segmentCount; i++)
        {
            if (hasValidTimes[i])
            {
                if (cumulative != null)
                {
                    var segmentDuration = TimeSpan.FromSeconds(segmentHCPTimes[i] * scaleFactor);
                    cumulative += segmentDuration;
                }

                var time = new Time(Run[i].Comparisons[Name]);
                time[method] = cumulative;
                Run[i].Comparisons[Name] = time;
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
