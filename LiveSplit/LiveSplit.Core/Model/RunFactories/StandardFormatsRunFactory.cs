using LiveSplit.Model.Comparisons;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using static LiveSplit.UI.SettingsHelper;

namespace LiveSplit.Model.RunFactories
{
    public class StandardFormatsRunFactory : IRunFactory
    {
        [DllImport("kernel32")]
        private unsafe static extern void* LoadLibrary(string dllname);

        [DllImport("kernel32")]
        private unsafe static extern void FreeLibrary(void* handle);

        private sealed unsafe class LibraryUnloader
        {
            internal LibraryUnloader(void* handle)
            {
                this.handle = handle;
            }

            ~LibraryUnloader()
            {
                if (handle != null)
                    FreeLibrary(handle);
            }

            private void* handle;

        } // LibraryUnloader

        private static readonly LibraryUnloader unloader;

        static StandardFormatsRunFactory()
        {
            string path;

            if (IntPtr.Size == 4)
                path = "x86/livesplit_core.dll";
            else
                path = "x64/livesplit_core.dll";

            unsafe
            {
                void* handle = LoadLibrary(path);

                if (handle == null)
                    throw new DllNotFoundException("Unable to find the native livesplit-core library: " + path);

                unloader = new LibraryUnloader(handle);
            }
        }

        public Stream Stream { get; set; }
        public string FilePath { get; set; }

        public StandardFormatsRunFactory(Stream stream = null, string filePath = null)
        {
            Stream = stream;
            FilePath = filePath;
        }

        static TimeSpan ParseTimeSpan(LiveSplitCore.TimeSpanRef timeSpan)
        {
            return TimeSpan.FromSeconds(timeSpan.TotalSeconds());
        }

        static TimeSpan? ParseOptionalTimeSpan(LiveSplitCore.TimeSpanRef timeSpan)
        {
            if (timeSpan == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(timeSpan.TotalSeconds());
        }

        static AtomicDateTime? ParseOptionalAtomicDateTime(LiveSplitCore.AtomicDateTimeRef dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            return new AtomicDateTime(DateTime.Parse(dateTime.ToRfc3339()), dateTime.IsSynchronized());
        }

        static Time ParseTime(LiveSplitCore.TimeRef time)
        {
            return new Time
            {
                RealTime = ParseOptionalTimeSpan(time.RealTime()),
                GameTime = ParseOptionalTimeSpan(time.GameTime()),
            };
        }

        static Image ParseImage(string dataUrl)
        {
            if (!dataUrl.StartsWith("data:;base64,"))
            {
                return null;
            }

            var base64Data = dataUrl.Substring("data:;base64,".Length);
            var binData = Convert.FromBase64String(base64Data);

            // Do not dispose this memory stream, as the Image internally
            // borrows it for saving it out later on. This results in a
            // generic GDI+ error.
            // See https://github.com/LiveSplit/LiveSplit/issues/894
            var stream = new MemoryStream(binData);

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
                    result = LiveSplitCore.Run.ParseFileHandle((long)handle.DangerousGetHandle(), FilePath, !string.IsNullOrEmpty(FilePath));
                }
            }
            if (result == null)
            {
                result = LiveSplitCore.Run.Parse(Stream, FilePath, !string.IsNullOrEmpty(FilePath));
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
                using (var iter = metadata.Variables())
                {
                    LiveSplitCore.RunMetadataVariableRef variable;
                    while ((variable = iter.Next()) != null)
                    {
                        run.Metadata.VariableValueNames.Add(variable.Name(), variable.Value());
                    }
                }

                run.GameIcon = ParseImage(lscRun.GameIcon());
                run.GameName = lscRun.GameName();
                run.CategoryName = lscRun.CategoryName();
                run.Offset = ParseTimeSpan(lscRun.Offset());
                run.AttemptCount = (int)lscRun.AttemptCount();

                var attemptsCount = lscRun.AttemptHistoryLen();
                for (var i = 0; i < attemptsCount; ++i)
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
                for (var i = 0; i < customComparisonsCount; ++i)
                {
                    var comparison = lscRun.CustomComparison(i);
                    if (!run.CustomComparisons.Contains(comparison))
                    {
                        run.CustomComparisons.Add(comparison);
                    }
                }

                var segmentCount = lscRun.Len();
                for (var i = 0; i < segmentCount; ++i)
                {
                    var segment = lscRun.Segment(i);
                    var split = new Segment(segment.Name())
                    {
                        Icon = ParseImage(segment.Icon()),
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
