using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace LiveSplitCore
{
    /// <summary>
    /// An Atomic Date Time represents a UTC Date Time that tries to be as close to
    /// an atomic clock as possible.
    /// </summary>
    public class AtomicDateTimeRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Represents whether the date time is actually properly derived from an
        /// atomic clock. If the synchronization with the atomic clock didn't happen
        /// yet or failed, this is set to false.
        /// </summary>
        public bool IsSynchronized()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.AtomicDateTime_is_synchronized(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Converts this atomic date time into a RFC 3339 formatted date time.
        /// </summary>
        public string ToRfc3339()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.AtomicDateTime_to_rfc3339(this.ptr);
            return result;
        }
        internal AtomicDateTimeRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// An Atomic Date Time represents a UTC Date Time that tries to be as close to
    /// an atomic clock as possible.
    /// </summary>
    public class AtomicDateTimeRefMut : AtomicDateTimeRef
    {
        internal AtomicDateTimeRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An Atomic Date Time represents a UTC Date Time that tries to be as close to
    /// an atomic clock as possible.
    /// </summary>
    public class AtomicDateTime : AtomicDateTimeRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.AtomicDateTime_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~AtomicDateTime()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal AtomicDateTime(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An Attempt describes information about an attempt to run a specific category
    /// by a specific runner in the past. Every time a new attempt is started and
    /// then reset, an Attempt describing general information about it is created.
    /// </summary>
    public class AttemptRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the unique index of the attempt. This index is unique for the
        /// Run, not for all of them.
        /// </summary>
        public int Index()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Attempt_index(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the split time of the last segment. If the attempt got reset
        /// early and didn't finish, this may be empty.
        /// </summary>
        public TimeRef Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Attempt_time(this.ptr));
            return result;
        }
        /// <summary>
        /// Accesses the amount of time the attempt has been paused for. If it is not
        /// known, this returns null. This means that it may not necessarily be
        /// possible to differentiate whether a Run has not been paused or it simply
        /// wasn't stored.
        /// </summary>
        public TimeSpanRef PauseTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Attempt_pause_time(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Accesses the point in time the attempt was started at. This returns null
        /// if this information is not known.
        /// </summary>
        public AtomicDateTime Started()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new AtomicDateTime(LiveSplitCoreNative.Attempt_started(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Accesses the point in time the attempt was ended at. This returns null if
        /// this information is not known.
        /// </summary>
        public AtomicDateTime Ended()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new AtomicDateTime(LiveSplitCoreNative.Attempt_ended(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal AttemptRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// An Attempt describes information about an attempt to run a specific category
    /// by a specific runner in the past. Every time a new attempt is started and
    /// then reset, an Attempt describing general information about it is created.
    /// </summary>
    public class AttemptRefMut : AttemptRef
    {
        internal AttemptRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An Attempt describes information about an attempt to run a specific category
    /// by a specific runner in the past. Every time a new attempt is started and
    /// then reset, an Attempt describing general information about it is created.
    /// </summary>
    public class Attempt : AttemptRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~Attempt()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal Attempt(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Localization bindings.
    /// </summary>
    public class LangRef
    {
        internal IntPtr ptr;
        internal LangRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// Localization bindings.
    /// </summary>
    public class LangRefMut : LangRef
    {
        internal LangRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Localization bindings.
    /// </summary>
    public class Lang : LangRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~Lang()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Parses a locale string into a language.
        /// </summary>
        public static byte ParseLocale(string locale)
        {
            var result = LiveSplitCoreNative.Lang_parse_locale(locale);
            return result;
        }
        /// <summary>
        /// Parses a language name into a language.
        /// </summary>
        public static byte FromName(string name)
        {
            var result = LiveSplitCoreNative.Lang_from_name(name);
            return result;
        }
        /// <summary>
        /// Returns the localized display name for a language.
        /// </summary>
        public static string Name(byte lang)
        {
            var result = LiveSplitCoreNative.Lang_name(lang);
            return result;
        }
        internal Lang(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Linked Layout associates a Layout with a Run. If the Run has a Linked
    /// Layout, it is supposed to be visualized with the Layout that is linked with
    /// it.
    /// </summary>
    public class LinkedLayoutRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Checks whether the linked layout is the default layout.
        /// </summary>
        public bool IsDefault()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.LinkedLayout_is_default(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns the path of the linked layout, if it's not the default layout.
        /// </summary>
        public string Path()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.LinkedLayout_path(this.ptr);
            return result;
        }
        internal LinkedLayoutRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Linked Layout associates a Layout with a Run. If the Run has a Linked
    /// Layout, it is supposed to be visualized with the Layout that is linked with
    /// it.
    /// </summary>
    public class LinkedLayoutRefMut : LinkedLayoutRef
    {
        internal LinkedLayoutRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Linked Layout associates a Layout with a Run. If the Run has a Linked
    /// Layout, it is supposed to be visualized with the Layout that is linked with
    /// it.
    /// </summary>
    public class LinkedLayout : LinkedLayoutRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.LinkedLayout_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~LinkedLayout()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Linked Layout with the path specified. If the path is empty,
        /// the default layout is used instead.
        /// </summary>
        public LinkedLayout(string path) : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.LinkedLayout_new(path);
        }
        internal LinkedLayout(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A run parsed by the Composite Parser. This contains the Run itself and
    /// information about which parser parsed it.
    /// </summary>
    public class ParseRunResultRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Returns true if the Run got parsed successfully. false is returned otherwise.
        /// </summary>
        public bool ParsedSuccessfully()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.ParseRunResult_parsed_successfully(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Accesses the name of the Parser that parsed the Run. You may not call this
        /// if the Run wasn't parsed successfully.
        /// </summary>
        public string TimerKind()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.ParseRunResult_timer_kind(this.ptr);
            return result;
        }
        /// <summary>
        /// Checks whether the Parser parsed a generic timer. Since a generic timer can
        /// have any name, it may clash with the specific timer formats that
        /// livesplit-core supports. With this function you can determine if a generic
        /// timer format was parsed, instead of one of the more specific timer formats.
        /// </summary>
        public bool IsGenericTimer()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.ParseRunResult_is_generic_timer(this.ptr) != 0;
            return result;
        }
        internal ParseRunResultRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A run parsed by the Composite Parser. This contains the Run itself and
    /// information about which parser parsed it.
    /// </summary>
    public class ParseRunResultRefMut : ParseRunResultRef
    {
        internal ParseRunResultRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A run parsed by the Composite Parser. This contains the Run itself and
    /// information about which parser parsed it.
    /// </summary>
    public class ParseRunResult : ParseRunResultRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.ParseRunResult_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~ParseRunResult()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Moves the actual Run object out of the Result. You may not call this if the
        /// Run wasn't parsed successfully.
        /// </summary>
        public Run Unwrap()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Run(LiveSplitCoreNative.ParseRunResult_unwrap(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal ParseRunResult(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Run stores the split times for a specific game and category of a runner.
    /// </summary>
    public class RunRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Clones the Run object.
        /// </summary>
        public Run Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Run(LiveSplitCoreNative.Run_clone(this.ptr));
            return result;
        }
        /// <summary>
        /// Accesses the name of the game this Run is for.
        /// </summary>
        public string GameName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_game_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the game icon's data. If there is no game icon, this returns an
        /// empty buffer.
        /// </summary>
        public IntPtr GameIconPtr()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_game_icon_ptr(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the amount of bytes the game icon's data takes up.
        /// </summary>
        public ulong GameIconLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_game_icon_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the name of the category this Run is for.
        /// </summary>
        public string CategoryName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_category_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns a file name (without the extension) suitable for this Run that
        /// is built the following way:
        ///
        /// Game Name - Category Name
        ///
        /// If either is empty, the dash is omitted. Special characters that cause
        /// problems in file names are also omitted. If an extended category name is
        /// used, the variables of the category are appended in a parenthesis.
        /// </summary>
        public string ExtendedFileName(bool useExtendedCategoryName)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_extended_file_name(this.ptr, useExtendedCategoryName);
            return result;
        }
        /// <summary>
        /// Returns a name suitable for this Run that is built the following way:
        ///
        /// Game Name - Category Name
        ///
        /// If either is empty, the dash is omitted. If an extended category name is
        /// used, the variables of the category are appended in a parenthesis.
        /// </summary>
        public string ExtendedName(bool useExtendedCategoryName)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_extended_name(this.ptr, useExtendedCategoryName);
            return result;
        }
        /// <summary>
        /// Returns an extended category name that possibly includes the region,
        /// platform and variables, depending on the arguments provided. An extended
        /// category name may look like this:
        ///
        /// Any% (No Tuner, JPN, Wii Emulator)
        /// </summary>
        public string ExtendedCategoryName(bool showRegion, bool showPlatform, bool showVariables)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_extended_category_name(this.ptr, showRegion, showPlatform, showVariables);
            return result;
        }
        /// <summary>
        /// Returns the amount of runs that have been attempted with these splits.
        /// </summary>
        public uint AttemptCount()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_attempt_count(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses additional metadata of this Run, like the platform and region
        /// of the game.
        /// </summary>
        public RunMetadataRef Metadata()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataRef(LiveSplitCoreNative.Run_metadata(this.ptr));
            return result;
        }
        /// <summary>
        /// Accesses the time an attempt of this Run should start at.
        /// </summary>
        public TimeSpanRef Offset()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Run_offset(this.ptr));
            return result;
        }
        /// <summary>
        /// Returns the amount of segments stored in this Run.
        /// </summary>
        public ulong Len()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns whether the Run has been modified and should be saved so that the
        /// changes don't get lost.
        /// </summary>
        public bool HasBeenModified()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_has_been_modified(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Accesses a certain segment of this Run. You may not provide an out of bounds
        /// index.
        /// </summary>
        public SegmentRef Segment(ulong index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentRef(LiveSplitCoreNative.Run_segment(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Returns the amount of native segment groups stored in this Run.
        /// </summary>
        public ulong SegmentGroupsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_segment_groups_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses a native segment group stored in this Run by its index. You may not
        /// provide an out of bounds index.
        /// </summary>
        public SegmentGroupRef SegmentGroup(ulong index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentGroupRef(LiveSplitCoreNative.Run_segment_group(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Returns the amount of segments in this Run.
        /// </summary>
        public ulong SegmentsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_segments_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the amount attempt history elements are stored in this Run.
        /// </summary>
        public ulong AttemptHistoryLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_attempt_history_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the an attempt history element by its index. This does not store
        /// the actual segment times, just the overall attempt information. Information
        /// about the individual segments is stored within each segment. You may not
        /// provide an out of bounds index.
        /// </summary>
        public AttemptRef AttemptHistoryIndex(ulong index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new AttemptRef(LiveSplitCoreNative.Run_attempt_history_index(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Returns the amount of custom comparisons stored in this Run.
        /// </summary>
        public ulong CustomComparisonsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_custom_comparisons_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses a custom comparison stored in this Run by its index. This includes
        /// `Personal Best` but excludes all the other Comparison Generators. You may
        /// not provide an out of bounds index.
        /// </summary>
        public string CustomComparison(ulong index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_custom_comparison(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// Returns the amount of total comparisons stored in this Run.
        /// </summary>
        public ulong ComparisonsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Run_comparisons_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses a comparison stored in this Run by its index. This includes both
        /// custom comparisons as well as all the Comparison Generators. You may not
        /// provide an out of bounds index.
        /// </summary>
        public string Comparison(ulong index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_comparison(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// Accesses the Auto Splitter Settings that are encoded as XML.
        /// </summary>
        public string AutoSplitterSettings()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_auto_splitter_settings(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the linked layout of this Run. If a Layout is linked, it is
        /// supposed to be loaded to visualize the Run.
        /// </summary>
        public LinkedLayout LinkedLayout()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new LinkedLayout(LiveSplitCoreNative.Run_linked_layout(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal RunRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Run stores the split times for a specific game and category of a runner.
    /// </summary>
    public class RunRefMut : RunRef
    {
        /// <summary>
        /// Pushes the segment provided to the end of the list of segments of this Run.
        /// </summary>
        public void PushSegment(Segment segment)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (segment.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("segment");
            }
            LiveSplitCoreNative.Run_push_segment(this.ptr, segment.ptr);
            segment.ptr = IntPtr.Zero;
        }
        /// <summary>
        /// Sets the name of the game this Run is for.
        /// </summary>
        public void SetGameName(string game)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Run_set_game_name(this.ptr, game);
        }
        /// <summary>
        /// Sets the name of the category this Run is for.
        /// </summary>
        public void SetCategoryName(string category)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Run_set_category_name(this.ptr, category);
        }
        /// <summary>
        /// Marks the Run as modified, so that it is known that there are changes
        /// that should be saved.
        /// </summary>
        public void MarkAsModified()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Run_mark_as_modified(this.ptr);
        }
        internal RunRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Run stores the split times for a specific game and category of a runner.
    /// </summary>
    public class Run : RunRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.Run_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~Run()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Run object with no segments.
        /// </summary>
        public Run() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.Run_new();
        }
        /// <summary>
        /// Attempts to parse a splits file from an array by invoking the corresponding
        /// parser for the file format detected. Additionally you can provide the path
        /// of the splits file so additional files, like external images, can be loaded.
        /// If you are using livesplit-core in a server-like environment, set this to
        /// null. Only client-side applications should provide a path here. Unlike the
        /// normal parsing function, it also fixes problems in the Run, such as
        /// decreasing times and missing information.
        /// </summary>
        public static ParseRunResult Parse(IntPtr data, ulong length, string loadFilesPath)
        {
            var result = new ParseRunResult(LiveSplitCoreNative.Run_parse((IntPtr)data, (UIntPtr)length, loadFilesPath));
            return result;
        }
        /// <summary>
        /// Attempts to parse a splits file from a file by invoking the corresponding
        /// parser for the file format detected. Additionally you can provide the path
        /// of the splits file so additional files, like external images, can be loaded.
        /// If you are using livesplit-core in a server-like environment, set this to
        /// null. Only client-side applications should provide a path here. Unlike the
        /// normal parsing function, it also fixes problems in the Run, such as
        /// decreasing times and missing information. On Unix you pass a file descriptor
        /// to this function. On Windows you pass a file handle to this function. The
        /// file descriptor / handle does not get closed.
        /// </summary>
        public static ParseRunResult ParseFileHandle(long handle, string loadFilesPath)
        {
            var result = new ParseRunResult(LiveSplitCoreNative.Run_parse_file_handle(handle, loadFilesPath));
            return result;
        }
        public static ParseRunResult Parse(Stream stream, string loadFilesPath)
        {
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            IntPtr pnt = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, pnt, data.Length);
                return Parse(pnt, (ulong)data.Length, loadFilesPath);
            }
            finally
            {
                Marshal.FreeHGlobal(pnt);
            }
        }
        internal Run(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Run Metadata stores additional information about a run, like the
    /// platform and region of the game. All of this information is optional.
    /// </summary>
    public class RunMetadataRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the speedrun.com Run ID of the run. This Run ID specify which
        /// Record on speedrun.com this run is associated with. This should be
        /// changed once the Personal Best doesn't match up with that record
        /// anymore. This may be empty if there's no association.
        /// </summary>
        public string RunId()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_run_id(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the name of the platform this game is run on. This may be empty
        /// if it's not specified.
        /// </summary>
        public string PlatformName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_platform_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns true if this speedrun is done on an emulator. However false
        /// may also indicate that this information is simply not known.
        /// </summary>
        public bool UsesEmulator()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_uses_emulator(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Accesses the name of the region this game is from. This may be empty if
        /// it's not specified.
        /// </summary>
        public string RegionName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_region_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns an iterator iterating over all the speedrun.com variables and their
        /// values that have been specified.
        /// </summary>
        public RunMetadataSpeedrunComVariablesIter SpeedrunComVariables()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataSpeedrunComVariablesIter(LiveSplitCoreNative.RunMetadata_speedrun_com_variables(this.ptr));
            return result;
        }
        /// <summary>
        /// Returns an iterator iterating over all the custom variables and their
        /// values. This includes both temporary and permanent variables.
        /// </summary>
        public RunMetadataCustomVariablesIter CustomVariables()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataCustomVariablesIter(LiveSplitCoreNative.RunMetadata_custom_variables(this.ptr));
            return result;
        }
        internal RunMetadataRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Run Metadata stores additional information about a run, like the
    /// platform and region of the game. All of this information is optional.
    /// </summary>
    public class RunMetadataRefMut : RunMetadataRef
    {
        internal RunMetadataRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Run Metadata stores additional information about a run, like the
    /// platform and region of the game. All of this information is optional.
    /// </summary>
    public class RunMetadata : RunMetadataRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadata()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadata(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A custom variable is a key value pair storing additional information about a
    /// run. Unlike the speedrun.com variables, these can be fully custom and don't
    /// need to correspond to anything on speedrun.com. Permanent custom variables
    /// can be specified by the runner. Additionally auto splitters or other sources
    /// may provide temporary custom variables that are not stored in the splits
    /// files.
    /// </summary>
    public class RunMetadataCustomVariableRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the name of this custom variable.
        /// </summary>
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataCustomVariable_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the value of this custom variable.
        /// </summary>
        public string Value()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataCustomVariable_value(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns true if the custom variable is permanent. Permanent variables get
        /// stored in the splits file and are visible in the run editor. Temporary
        /// variables are not.
        /// </summary>
        public bool IsPermanent()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataCustomVariable_is_permanent(this.ptr) != 0;
            return result;
        }
        internal RunMetadataCustomVariableRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A custom variable is a key value pair storing additional information about a
    /// run. Unlike the speedrun.com variables, these can be fully custom and don't
    /// need to correspond to anything on speedrun.com. Permanent custom variables
    /// can be specified by the runner. Additionally auto splitters or other sources
    /// may provide temporary custom variables that are not stored in the splits
    /// files.
    /// </summary>
    public class RunMetadataCustomVariableRefMut : RunMetadataCustomVariableRef
    {
        internal RunMetadataCustomVariableRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A custom variable is a key value pair storing additional information about a
    /// run. Unlike the speedrun.com variables, these can be fully custom and don't
    /// need to correspond to anything on speedrun.com. Permanent custom variables
    /// can be specified by the runner. Additionally auto splitters or other sources
    /// may provide temporary custom variables that are not stored in the splits
    /// files.
    /// </summary>
    public class RunMetadataCustomVariable : RunMetadataCustomVariableRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.RunMetadataCustomVariable_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadataCustomVariable()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadataCustomVariable(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An iterator iterating over all the custom variables and their values
    /// that have been specified.
    /// </summary>
    public class RunMetadataCustomVariablesIterRef
    {
        internal IntPtr ptr;
        internal RunMetadataCustomVariablesIterRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// An iterator iterating over all the custom variables and their values
    /// that have been specified.
    /// </summary>
    public class RunMetadataCustomVariablesIterRefMut : RunMetadataCustomVariablesIterRef
    {
        /// <summary>
        /// Accesses the next custom variable. Returns null if there are no more
        /// variables.
        /// </summary>
        public RunMetadataCustomVariableRef Next()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataCustomVariableRef(LiveSplitCoreNative.RunMetadataCustomVariablesIter_next(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal RunMetadataCustomVariablesIterRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An iterator iterating over all the custom variables and their values
    /// that have been specified.
    /// </summary>
    public class RunMetadataCustomVariablesIter : RunMetadataCustomVariablesIterRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.RunMetadataCustomVariablesIter_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadataCustomVariablesIter()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadataCustomVariablesIter(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A speedrun.com variable is an arbitrary key value pair storing additional
    /// information about the category. An example of this may be whether Amiibos
    /// are used in the category.
    /// </summary>
    public class RunMetadataSpeedrunComVariableRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the name of this speedrun.com variable.
        /// </summary>
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataSpeedrunComVariable_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the value of this speedrun.com variable.
        /// </summary>
        public string Value()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataSpeedrunComVariable_value(this.ptr);
            return result;
        }
        internal RunMetadataSpeedrunComVariableRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A speedrun.com variable is an arbitrary key value pair storing additional
    /// information about the category. An example of this may be whether Amiibos
    /// are used in the category.
    /// </summary>
    public class RunMetadataSpeedrunComVariableRefMut : RunMetadataSpeedrunComVariableRef
    {
        internal RunMetadataSpeedrunComVariableRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A speedrun.com variable is an arbitrary key value pair storing additional
    /// information about the category. An example of this may be whether Amiibos
    /// are used in the category.
    /// </summary>
    public class RunMetadataSpeedrunComVariable : RunMetadataSpeedrunComVariableRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.RunMetadataSpeedrunComVariable_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadataSpeedrunComVariable()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadataSpeedrunComVariable(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An iterator iterating over all the speedrun.com variables and their values
    /// that have been specified.
    /// </summary>
    public class RunMetadataSpeedrunComVariablesIterRef
    {
        internal IntPtr ptr;
        internal RunMetadataSpeedrunComVariablesIterRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// An iterator iterating over all the speedrun.com variables and their values
    /// that have been specified.
    /// </summary>
    public class RunMetadataSpeedrunComVariablesIterRefMut : RunMetadataSpeedrunComVariablesIterRef
    {
        /// <summary>
        /// Accesses the next speedrun.com variable. Returns null if there are no more
        /// variables.
        /// </summary>
        public RunMetadataSpeedrunComVariableRef Next()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataSpeedrunComVariableRef(LiveSplitCoreNative.RunMetadataSpeedrunComVariablesIter_next(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal RunMetadataSpeedrunComVariablesIterRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// An iterator iterating over all the speedrun.com variables and their values
    /// that have been specified.
    /// </summary>
    public class RunMetadataSpeedrunComVariablesIter : RunMetadataSpeedrunComVariablesIterRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.RunMetadataSpeedrunComVariablesIter_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadataSpeedrunComVariablesIter()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadataSpeedrunComVariablesIter(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Segment describes a point in a speedrun that is suitable for storing a
    /// split time. This stores the name of that segment, an icon, the split times
    /// of different comparisons, and a history of segment times.
    /// </summary>
    public class SegmentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the name of the segment.
        /// </summary>
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Segment_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the segment icon's data. If there is no segment icon, this returns
        /// an empty buffer.
        /// </summary>
        public IntPtr IconPtr()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Segment_icon_ptr(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the amount of bytes the segment icon's data takes up.
        /// </summary>
        public ulong IconLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.Segment_icon_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the specified comparison's time. If there's none for this
        /// comparison, an empty time is being returned (but not stored in the
        /// segment).
        /// </summary>
        public TimeRef Comparison(string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Segment_comparison(this.ptr, comparison));
            return result;
        }
        /// <summary>
        /// Accesses the split time of the Personal Best for this segment. If it
        /// doesn't exist, an empty time is returned.
        /// </summary>
        public TimeRef PersonalBestSplitTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Segment_personal_best_split_time(this.ptr));
            return result;
        }
        /// <summary>
        /// Accesses the Best Segment Time.
        /// </summary>
        public TimeRef BestSegmentTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Segment_best_segment_time(this.ptr));
            return result;
        }
        /// <summary>
        /// Accesses the Segment History of this segment.
        /// </summary>
        public SegmentHistoryRef SegmentHistory()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentHistoryRef(LiveSplitCoreNative.Segment_segment_history(this.ptr));
            return result;
        }
        internal SegmentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Segment describes a point in a speedrun that is suitable for storing a
    /// split time. This stores the name of that segment, an icon, the split times
    /// of different comparisons, and a history of segment times.
    /// </summary>
    public class SegmentRefMut : SegmentRef
    {
        internal SegmentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Segment describes a point in a speedrun that is suitable for storing a
    /// split time. This stores the name of that segment, an icon, the split times
    /// of different comparisons, and a history of segment times.
    /// </summary>
    public class Segment : SegmentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.Segment_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~Segment()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Segment with the name given.
        /// </summary>
        public Segment(string name) : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.Segment_new(name);
        }
        internal Segment(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Segment Group describes a contiguous range of segments that forms a
    /// one-level group.
    /// </summary>
    public class SegmentGroupRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the inclusive start index of the segment group.
        /// </summary>
        public ulong Start()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.SegmentGroup_start(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the exclusive end index of the segment group.
        /// </summary>
        public ulong End()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.SegmentGroup_end(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the explicit name of the segment group. If the group uses the
        /// final segment's name instead, an empty string is returned.
        /// </summary>
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SegmentGroup_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the explicit icon's data. If the group uses the final segment's
        /// icon instead, an empty buffer is returned.
        /// </summary>
        public IntPtr IconPtr()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SegmentGroup_icon_ptr(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the amount of bytes the explicit icon's data takes up.
        /// </summary>
        public ulong IconLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (ulong)LiveSplitCoreNative.SegmentGroup_icon_len(this.ptr);
            return result;
        }
        internal SegmentGroupRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Segment Group describes a contiguous range of segments that forms a
    /// one-level group.
    /// </summary>
    public class SegmentGroupRefMut : SegmentGroupRef
    {
        internal SegmentGroupRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Segment Group describes a contiguous range of segments that forms a
    /// one-level group.
    /// </summary>
    public class SegmentGroup : SegmentGroupRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~SegmentGroup()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SegmentGroup(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Stores the segment times achieved for a certain segment. Each segment is
    /// tagged with an index. Only segment times with an index larger than 0 are
    /// considered times actually achieved by the runner, while the others are
    /// artifacts of route changes and similar algorithmic changes.
    /// </summary>
    public class SegmentHistoryRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Iterates over all the segment times and their indices.
        /// </summary>
        public SegmentHistoryIter Iter()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentHistoryIter(LiveSplitCoreNative.SegmentHistory_iter(this.ptr));
            return result;
        }
        internal SegmentHistoryRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// Stores the segment times achieved for a certain segment. Each segment is
    /// tagged with an index. Only segment times with an index larger than 0 are
    /// considered times actually achieved by the runner, while the others are
    /// artifacts of route changes and similar algorithmic changes.
    /// </summary>
    public class SegmentHistoryRefMut : SegmentHistoryRef
    {
        internal SegmentHistoryRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Stores the segment times achieved for a certain segment. Each segment is
    /// tagged with an index. Only segment times with an index larger than 0 are
    /// considered times actually achieved by the runner, while the others are
    /// artifacts of route changes and similar algorithmic changes.
    /// </summary>
    public class SegmentHistory : SegmentHistoryRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~SegmentHistory()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SegmentHistory(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A segment time achieved for a segment. It is tagged with an index. Only
    /// segment times with an index larger than 0 are considered times actually
    /// achieved by the runner, while the others are artifacts of route changes and
    /// similar algorithmic changes.
    /// </summary>
    public class SegmentHistoryElementRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the index of the segment history element.
        /// </summary>
        public int Index()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SegmentHistoryElement_index(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the segment time of the segment history element.
        /// </summary>
        public TimeRef Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.SegmentHistoryElement_time(this.ptr));
            return result;
        }
        internal SegmentHistoryElementRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A segment time achieved for a segment. It is tagged with an index. Only
    /// segment times with an index larger than 0 are considered times actually
    /// achieved by the runner, while the others are artifacts of route changes and
    /// similar algorithmic changes.
    /// </summary>
    public class SegmentHistoryElementRefMut : SegmentHistoryElementRef
    {
        internal SegmentHistoryElementRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A segment time achieved for a segment. It is tagged with an index. Only
    /// segment times with an index larger than 0 are considered times actually
    /// achieved by the runner, while the others are artifacts of route changes and
    /// similar algorithmic changes.
    /// </summary>
    public class SegmentHistoryElement : SegmentHistoryElementRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~SegmentHistoryElement()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SegmentHistoryElement(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Iterates over all the segment times of a segment and their indices.
    /// </summary>
    public class SegmentHistoryIterRef
    {
        internal IntPtr ptr;
        internal SegmentHistoryIterRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// Iterates over all the segment times of a segment and their indices.
    /// </summary>
    public class SegmentHistoryIterRefMut : SegmentHistoryIterRef
    {
        /// <summary>
        /// Accesses the next Segment History element. Returns null if there are no
        /// more elements.
        /// </summary>
        public SegmentHistoryElementRef Next()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentHistoryElementRef(LiveSplitCoreNative.SegmentHistoryIter_next(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal SegmentHistoryIterRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Iterates over all the segment times of a segment and their indices.
    /// </summary>
    public class SegmentHistoryIter : SegmentHistoryIterRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SegmentHistoryIter_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SegmentHistoryIter()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SegmentHistoryIter(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A time that can store a Real Time and a Game Time. Both of them are
    /// optional.
    /// </summary>
    public class TimeRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Clones the time.
        /// </summary>
        public Time Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Time(LiveSplitCoreNative.Time_clone(this.ptr));
            return result;
        }
        /// <summary>
        /// The Real Time value. This may be null if this time has no Real Time value.
        /// </summary>
        public TimeSpanRef RealTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Time_real_time(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// The Game Time value. This may be null if this time has no Game Time value.
        /// </summary>
        public TimeSpanRef GameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Time_game_time(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Access the time's value for the timing method specified.
        /// </summary>
        public TimeSpanRef Index(byte timingMethod)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Time_index(this.ptr, timingMethod));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal TimeRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A time that can store a Real Time and a Game Time. Both of them are
    /// optional.
    /// </summary>
    public class TimeRefMut : TimeRef
    {
        internal TimeRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A time that can store a Real Time and a Game Time. Both of them are
    /// optional.
    /// </summary>
    public class Time : TimeRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.Time_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~Time()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal Time(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Time Span represents a certain span of time.
    /// </summary>
    public class TimeSpanRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Clones the Time Span.
        /// </summary>
        public TimeSpan Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpan(LiveSplitCoreNative.TimeSpan_clone(this.ptr));
            return result;
        }
        /// <summary>
        /// Returns the total amount of seconds (including decimals) this Time Span
        /// represents.
        /// </summary>
        public double TotalSeconds()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimeSpan_total_seconds(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the total amount of whole seconds (excluding decimals) this Time
        /// Span represents.
        /// </summary>
        public long WholeSeconds()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimeSpan_whole_seconds(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the number of nanoseconds past the last full second that makes up
        /// the Time Span.
        /// </summary>
        public int SubsecNanoseconds()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimeSpan_subsec_nanoseconds(this.ptr);
            return result;
        }
        internal TimeSpanRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Time Span represents a certain span of time.
    /// </summary>
    public class TimeSpanRefMut : TimeSpanRef
    {
        /// <summary>
        /// Changes a Time Span by adding a Time Span onto it.
        /// </summary>
        public void AddAssign(TimeSpanRef other)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (other.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("other");
            }
            LiveSplitCoreNative.TimeSpan_add_assign(this.ptr, other.ptr);
        }
        internal TimeSpanRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Time Span represents a certain span of time.
    /// </summary>
    public class TimeSpan : TimeSpanRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TimeSpan_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TimeSpan()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Time Span from a given amount of seconds.
        /// </summary>
        public static TimeSpan FromSeconds(double seconds)
        {
            var result = new TimeSpan(LiveSplitCoreNative.TimeSpan_from_seconds(seconds));
            return result;
        }
        /// <summary>
        /// Parses a Time Span from a string. Returns null if the time can't be
        /// parsed.
        /// </summary>
        public static TimeSpan Parse(string text, byte lang)
        {
            var result = new TimeSpan(LiveSplitCoreNative.TimeSpan_parse(text, lang));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal TimeSpan(IntPtr ptr) : base(ptr) { }
    }

    public static class LiveSplitCoreNative
    {
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AtomicDateTime_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte AtomicDateTime_is_synchronized(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString AtomicDateTime_to_rfc3339(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Attempt_index(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Attempt_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Attempt_pause_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Attempt_started(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Attempt_ended(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Lang_parse_locale(LSCoreString locale);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Lang_from_name(LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Lang_name(byte lang);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LinkedLayout_new(LSCoreString path);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinkedLayout_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte LinkedLayout_is_default(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString LinkedLayout_path(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ParseRunResult_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ParseRunResult_unwrap(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ParseRunResult_parsed_successfully(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString ParseRunResult_timer_kind(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ParseRunResult_is_generic_timer(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_parse(IntPtr data, UIntPtr length, LSCoreString load_files_path);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_parse_file_handle(long handle, LSCoreString load_files_path);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_game_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_game_icon_ptr(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_game_icon_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_category_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_extended_file_name(IntPtr self, bool use_extended_category_name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_extended_name(IntPtr self, bool use_extended_category_name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_extended_category_name(IntPtr self, bool show_region, bool show_platform, bool show_variables);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Run_attempt_count(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_metadata(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_offset(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Run_has_been_modified(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_segment(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_segment_groups_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_segment_group(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_segments_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_attempt_history_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_attempt_history_index(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_custom_comparisons_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_custom_comparison(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_comparisons_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_comparison(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_auto_splitter_settings(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_linked_layout(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_push_segment(IntPtr self, IntPtr segment);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_set_game_name(IntPtr self, LSCoreString game);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_set_category_name(IntPtr self, LSCoreString category);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_mark_as_modified(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadata_run_id(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadata_platform_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunMetadata_uses_emulator(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadata_region_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunMetadata_speedrun_com_variables(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunMetadata_custom_variables(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunMetadataCustomVariable_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadataCustomVariable_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadataCustomVariable_value(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunMetadataCustomVariable_is_permanent(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunMetadataCustomVariablesIter_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunMetadataCustomVariablesIter_next(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunMetadataSpeedrunComVariable_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadataSpeedrunComVariable_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadataSpeedrunComVariable_value(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunMetadataSpeedrunComVariablesIter_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunMetadataSpeedrunComVariablesIter_next(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_new(LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Segment_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Segment_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_icon_ptr(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Segment_icon_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_comparison(IntPtr self, LSCoreString comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_personal_best_split_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_best_segment_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_segment_history(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SegmentGroup_start(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SegmentGroup_end(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SegmentGroup_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SegmentGroup_icon_ptr(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SegmentGroup_icon_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SegmentHistory_iter(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SegmentHistoryElement_index(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SegmentHistoryElement_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SegmentHistoryIter_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SegmentHistoryIter_next(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Time_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Time_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Time_real_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Time_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Time_index(IntPtr self, byte timing_method);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimeSpan_from_seconds(double seconds);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimeSpan_parse(LSCoreString text, byte lang);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimeSpan_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimeSpan_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern double TimeSpan_total_seconds(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern long TimeSpan_whole_seconds(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TimeSpan_subsec_nanoseconds(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimeSpan_add_assign(IntPtr self, IntPtr other);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr get_buf_len();
    }

    public class LSCoreString : SafeHandle
    {
        private bool needToFree;

        public LSCoreString() : base(IntPtr.Zero, false) { }

        public override bool IsInvalid
        {
            get { return false; }
        }

        public static implicit operator LSCoreString(string managedString)
        {
            LSCoreString lsCoreString = new LSCoreString();

            int len = Encoding.UTF8.GetByteCount(managedString);
            byte[] buffer = new byte[len + 1];
            Encoding.UTF8.GetBytes(managedString, 0, managedString.Length, buffer, 0);
            IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);

            lsCoreString.SetHandle(nativeUtf8);
            lsCoreString.needToFree = true;
            return lsCoreString;
        }

        /// Unsafely assumes that the length can be retrieved from
        /// `get_buf_len`. This is only true for strings that have actually been
        /// retrieved from livesplit-core.
        public static implicit operator string(LSCoreString lSCoreString)
        {
            var handle = lSCoreString.handle;
            if (handle == IntPtr.Zero)
                return null;

            byte[] buffer = new byte[(long)LiveSplitCoreNative.get_buf_len()];
            Marshal.Copy(handle, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        protected override bool ReleaseHandle()
        {
            if (needToFree)
            {
                Marshal.FreeHGlobal(handle);
            }
            return true;
        }
    }
}
