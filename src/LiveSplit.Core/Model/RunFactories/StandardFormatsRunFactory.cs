using LiveSplit.Model.Comparisons;
using LiveSplit.UI;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Model.RunFactories
{
    public class StandardFormatsRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }
        public string FilePath { get; set; }

        public StandardFormatsRunFactory(Stream stream = null, string filePath = "")
        {
            Stream = stream;
            FilePath = filePath;
        }

        static TimeSpan ParseTimeSpan(LiveSplitCore.TimeSpanRef timeSpan)
        {
            var wholeSeconds = timeSpan.WholeSeconds();
            var subsecNanoseconds = timeSpan.SubsecNanoseconds();
            const long NANOS_PER_SEC = 1_000_000_000;
            const long NANOS_PER_TICK = NANOS_PER_SEC / TimeSpan.TicksPerSecond;

            var totalTicks = wholeSeconds * TimeSpan.TicksPerSecond + subsecNanoseconds / NANOS_PER_TICK;
            return new TimeSpan(totalTicks);
        }

        static TimeSpan? ParseOptionalTimeSpan(LiveSplitCore.TimeSpanRef timeSpan)
        {
            if (timeSpan == null)
            {
                return null;
            }
            return ParseTimeSpan(timeSpan);
        }

        static AtomicDateTime? ParseOptionalAtomicDateTime(LiveSplitCore.AtomicDateTimeRef dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            var utcDateTime = DateTime.Parse(dateTime.ToRfc3339(), CultureInfo.InvariantCulture).ToUniversalTime();
            return new AtomicDateTime(utcDateTime, dateTime.IsSynchronized());
        }

        static Time ParseTime(LiveSplitCore.TimeRef time)
        {
            return new Time
            {
                RealTime = ParseOptionalTimeSpan(time.RealTime()),
                GameTime = ParseOptionalTimeSpan(time.GameTime()),
            };
        }

        static Image ParseImage(IntPtr imagePtr, ulong length)
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

            return Image.FromStream(stream);
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            LiveSplitCore.ParseRunResult result = null;
            if (Stream is FileStream)
            {
                var handle = (Stream as FileStream).SafeFileHandle;
                if (!handle.IsInvalid)
                {
                    result = LiveSplitCore.Run.ParseFileHandle((long)handle.DangerousGetHandle(), FilePath);
                }
            }
            if (result == null)
            {
                result = LiveSplitCore.Run.Parse(Stream, FilePath);
            }

            if (!result.ParsedSuccessfully())
                throw new Exception();

            var timerKind = result.TimerKind();

            using (var lscRun = result.Unwrap())
            {
                var run = new Run(factory);

                var metadata = lscRun.Metadata();
                run.Metadata.RunID = metadata.RunId();
                run.Metadata.PlatformName = metadata.PlatformName();
                run.Metadata.UsesEmulator = metadata.UsesEmulator();
                run.Metadata.RegionName = metadata.RegionName();
                using (var iter = metadata.SpeedrunComVariables())
                {
                    LiveSplitCore.RunMetadataSpeedrunComVariableRef variable;
                    while ((variable = iter.Next()) != null)
                    {
                        run.Metadata.VariableValueNames.Add(variable.Name(), variable.Value());
                    }
                }

                run.GameIcon = ParseImage(lscRun.GameIconPtr(), lscRun.GameIconLen());
                run.GameName = lscRun.GameName();
                run.CategoryName = lscRun.CategoryName();
                run.Offset = ParseTimeSpan(lscRun.Offset());
                run.AttemptCount = (int)lscRun.AttemptCount();

                var linkedLayout = lscRun.LinkedLayout();
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

                var attemptsCount = lscRun.AttemptHistoryLen();
                for (var i = 0ul; i < attemptsCount; ++i)
                {
                    var attempt = lscRun.AttemptHistoryIndex(i);
                    run.AttemptHistory.Add(new Attempt(
                        attempt.Index(),
                        ParseTime(attempt.Time()),
                        ParseOptionalAtomicDateTime(attempt.Started()),
                        ParseOptionalAtomicDateTime(attempt.Ended()),
                        ParseOptionalTimeSpan(attempt.PauseTime())
                    ));
                }

                var customComparisonsCount = lscRun.CustomComparisonsLen();
                for (var i = 0ul; i < customComparisonsCount; ++i)
                {
                    var comparison = lscRun.CustomComparison(i);
                    if (!run.CustomComparisons.Contains(comparison))
                    {
                        run.CustomComparisons.Add(comparison);
                    }
                }

                var segmentCount = lscRun.Len();
                for (var i = 0ul; i < segmentCount; ++i)
                {
                    var segment = lscRun.Segment(i);
                    var split = new Segment(segment.Name())
                    {
                        Icon = ParseImage(segment.IconPtr(), segment.IconLen()),
                        BestSegmentTime = ParseTime(segment.BestSegmentTime()),
                    };

                    foreach (var comparison in run.CustomComparisons)
                    {
                        split.Comparisons[comparison] = ParseTime(segment.Comparison(comparison));
                    }

                    using (var iter = segment.SegmentHistory().Iter())
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
                    run.FilePath = FilePath;

                if (run.Count < 1)
                    throw new Exception("Run factory created a run without at least one segment");

                return run;
            }
        }
    }
}
