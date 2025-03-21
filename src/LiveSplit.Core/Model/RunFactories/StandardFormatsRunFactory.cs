using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

using LiveSplit.Model.Comparisons;
using LiveSplit.Options;

using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Model.RunFactories;

public class StandardFormatsRunFactory : IRunFactory
{
    public Stream Stream { get; set; }
    public string FilePath { get; set; }

    public StandardFormatsRunFactory(Stream stream = null, string filePath = "")
    {
        Stream = stream;
        FilePath = filePath;
    }

    private static TimeSpan ParseTimeSpan(LiveSplitCore.TimeSpanRef timeSpan)
    {
        long wholeSeconds = timeSpan.WholeSeconds();
        int subsecNanoseconds = timeSpan.SubsecNanoseconds();
        const long NANOS_PER_SEC = 1_000_000_000;
        const long NANOS_PER_TICK = NANOS_PER_SEC / TimeSpan.TicksPerSecond;

        long totalTicks = (wholeSeconds * TimeSpan.TicksPerSecond) + (subsecNanoseconds / NANOS_PER_TICK);
        return new TimeSpan(totalTicks);
    }

    private static TimeSpan? ParseOptionalTimeSpan(LiveSplitCore.TimeSpanRef timeSpan)
    {
        if (timeSpan == null)
        {
            return null;
        }

        return ParseTimeSpan(timeSpan);
    }

    private static AtomicDateTime? ParseOptionalAtomicDateTime(LiveSplitCore.AtomicDateTimeRef dateTime)
    {
        if (dateTime == null)
        {
            return null;
        }

        DateTime utcDateTime = DateTime.Parse(dateTime.ToRfc3339(), CultureInfo.InvariantCulture).ToUniversalTime();
        return new AtomicDateTime(utcDateTime, dateTime.IsSynchronized());
    }

    private static Time ParseTime(LiveSplitCore.TimeRef time)
    {
        return new Time
        {
            RealTime = ParseOptionalTimeSpan(time.RealTime()),
            GameTime = ParseOptionalTimeSpan(time.GameTime()),
        };
    }

    private static Image ParseImage(IntPtr imagePtr, ulong length)
    {
        if (length == 0)
        {
            return null;
        }

        byte[] buffer = new byte[length];
        Marshal.Copy(imagePtr, buffer, 0, buffer.Length);

        // Do not dispose this memory stream, as the Image internally
        // borrows it for saving it out later on. This results in a
        // generic GDI+ error.
        // See https://github.com/LiveSplit/LiveSplit/issues/894
        var stream = new MemoryStream(buffer);

        try
        {
            return Image.FromStream(stream);
        }
        catch (Exception ex)
        {
            // .NET Framework's Image doesn't support newer image formats
            // such as AVIF, WEBP, and JPEG XL. These may be used in
            // LiveSplit One. So we need to fall back to an empty image
            // instead of erroring out.
            Log.Error(ex);
            stream.Dispose();
            return null;
        }
    }

    public IRun Create(IComparisonGeneratorsFactory factory)
    {
        LiveSplitCore.ParseRunResult result = null;
        if (Stream is FileStream)
        {
            Microsoft.Win32.SafeHandles.SafeFileHandle handle = (Stream as FileStream).SafeFileHandle;
            if (!handle.IsInvalid)
            {
                result = LiveSplitCore.Run.ParseFileHandle((long)handle.DangerousGetHandle(), FilePath);
            }
        }

        result ??= LiveSplitCore.Run.Parse(Stream, FilePath);

        if (!result.ParsedSuccessfully())
        {
            throw new Exception();
        }

        string timerKind = result.TimerKind();

        using LiveSplitCore.Run lscRun = result.Unwrap();
        var run = new Run(factory);

        LiveSplitCore.RunMetadataRef metadata = lscRun.Metadata();
        run.Metadata.RunID = metadata.RunId();
        run.Metadata.PlatformName = metadata.PlatformName();
        run.Metadata.UsesEmulator = metadata.UsesEmulator();
        run.Metadata.RegionName = metadata.RegionName();
        using (LiveSplitCore.RunMetadataSpeedrunComVariablesIter iter = metadata.SpeedrunComVariables())
        {
            LiveSplitCore.RunMetadataSpeedrunComVariableRef variable;
            while ((variable = iter.Next()) != null)
            {
                run.Metadata.VariableValueNames.Add(variable.Name(), variable.Value());
            }
        }

        using (LiveSplitCore.RunMetadataCustomVariablesIter iter = metadata.CustomVariables())
        {
            LiveSplitCore.RunMetadataCustomVariableRef variable;
            while ((variable = iter.Next()) != null)
            {
                run.Metadata.GetOrAddCustomVariable(variable.Name()).AsPermanent().Value = variable.Value();
            }
        }

        run.GameIcon = ParseImage(lscRun.GameIconPtr(), lscRun.GameIconLen());
        run.GameName = lscRun.GameName();
        run.CategoryName = lscRun.CategoryName();
        run.Offset = ParseTimeSpan(lscRun.Offset());
        run.AttemptCount = (int)lscRun.AttemptCount();

        LiveSplitCore.LinkedLayout linkedLayout = lscRun.LinkedLayout();
        if (linkedLayout == null)
        {
            run.LayoutPath = null;
        }
        else if (linkedLayout.IsDefault())
        {
            run.LayoutPath = "?default";
        }
        else
        {
            run.LayoutPath = linkedLayout.Path();
        }

        ulong attemptsCount = lscRun.AttemptHistoryLen();
        for (ulong i = 0ul; i < attemptsCount; ++i)
        {
            LiveSplitCore.AttemptRef attempt = lscRun.AttemptHistoryIndex(i);
            run.AttemptHistory.Add(new Attempt(
                attempt.Index(),
                ParseTime(attempt.Time()),
                ParseOptionalAtomicDateTime(attempt.Started()),
                ParseOptionalAtomicDateTime(attempt.Ended()),
                ParseOptionalTimeSpan(attempt.PauseTime())
            ));
        }

        ulong customComparisonsCount = lscRun.CustomComparisonsLen();
        for (ulong i = 0ul; i < customComparisonsCount; ++i)
        {
            string comparison = lscRun.CustomComparison(i);
            if (!run.CustomComparisons.Contains(comparison))
            {
                run.CustomComparisons.Add(comparison);
            }
        }

        ulong segmentCount = lscRun.Len();
        for (ulong i = 0ul; i < segmentCount; ++i)
        {
            LiveSplitCore.SegmentRef segment = lscRun.Segment(i);
            var split = new Segment(segment.Name())
            {
                Icon = ParseImage(segment.IconPtr(), segment.IconLen()),
                BestSegmentTime = ParseTime(segment.BestSegmentTime()),
            };

            foreach (string comparison in run.CustomComparisons)
            {
                split.Comparisons[comparison] = ParseTime(segment.Comparison(comparison));
            }

            using (LiveSplitCore.SegmentHistoryIter iter = segment.SegmentHistory().Iter())
            {
                LiveSplitCore.SegmentHistoryElementRef element;
                while ((element = iter.Next()) != null)
                {
                    split.SegmentHistory.Add(element.Index(), ParseTime(element.Time()));
                }
            }

            run.Add(split);
        }

        var document = new XmlDocument();
        document.LoadXml($"<AutoSplitterSettings>{lscRun.AutoSplitterSettings()}</AutoSplitterSettings>");
        run.AutoSplitterSettings = document.FirstChild as XmlElement;
        run.AutoSplitterSettings.Attributes.Append(ToAttribute(document, "gameName", run.GameName));

        if (timerKind == "LiveSplit" && !string.IsNullOrEmpty(FilePath))
        {
            run.FilePath = FilePath;
        }

        if (run.Count < 1)
        {
            throw new Exception("Run factory created a run without at least one segment");
        }

        return run;
    }
}
