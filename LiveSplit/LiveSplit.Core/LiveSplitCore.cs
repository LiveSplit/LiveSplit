using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace LiveSplitCore
{
    /// <summary>
    /// The analysis module provides a variety of functions for calculating
    /// information about runs.
    /// </summary>
    public class AnalysisRef
    {
        internal IntPtr ptr;
        internal AnalysisRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The analysis module provides a variety of functions for calculating
    /// information about runs.
    /// </summary>
    public class AnalysisRefMut : AnalysisRef
    {
        internal AnalysisRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The analysis module provides a variety of functions for calculating
    /// information about runs.
    /// </summary>
    public class Analysis : AnalysisRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~Analysis()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Calculates the Sum of Best Segments for the timing method provided. This is
        /// the fastest time possible to complete a run of a category, based on
        /// information collected from all the previous attempts. This often matches up
        /// with the sum of the best segment times of all the segments, but that may not
        /// always be the case, as skipped segments may introduce combined segments that
        /// may be faster than the actual sum of their best segment times. The name is
        /// therefore a bit misleading, but sticks around for historical reasons. You
        /// can choose to do a simple calculation instead, which excludes the Segment
        /// History from the calculation process. If there's an active attempt, you can
        /// choose to take it into account as well. Can return null.
        /// </summary>
        public static TimeSpan CalculateSumOfBest(RunRef run, bool simpleCalculation, bool useCurrentRun, byte method)
        {
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = new TimeSpan(LiveSplitCoreNative.Analysis_calculate_sum_of_best(run.ptr, simpleCalculation, useCurrentRun, method));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Calculates the total playtime of the passed Run.
        /// </summary>
        public static TimeSpan CalculateTotalPlaytimeForRun(RunRef run)
        {
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = new TimeSpan(LiveSplitCoreNative.Analysis_calculate_total_playtime_for_run(run.ptr));
            return result;
        }
        /// <summary>
        /// Calculates the total playtime of the passed Timer.
        /// </summary>
        public static TimeSpan CalculateTotalPlaytimeForTimer(TimerRef timer)
        {
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new TimeSpan(LiveSplitCoreNative.Analysis_calculate_total_playtime_for_timer(timer.ptr));
            return result;
        }
        internal Analysis(IntPtr ptr) : base(ptr) { }
    }

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
        /// Converts this atomic date time into a RFC 2822 formatted date time.
        /// </summary>
        public string ToRfc2822()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.AtomicDateTime_to_rfc2822(this.ptr);
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
    /// The Blank Space Component is simply an empty component that doesn't show
    /// anything other than a background. It mostly serves as padding between other
    /// components.
    /// </summary>
    public class BlankSpaceComponentRef
    {
        internal IntPtr ptr;
        internal BlankSpaceComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Blank Space Component is simply an empty component that doesn't show
    /// anything other than a background. It mostly serves as padding between other
    /// components.
    /// </summary>
    public class BlankSpaceComponentRefMut : BlankSpaceComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.BlankSpaceComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public BlankSpaceComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new BlankSpaceComponentState(LiveSplitCoreNative.BlankSpaceComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal BlankSpaceComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Blank Space Component is simply an empty component that doesn't show
    /// anything other than a background. It mostly serves as padding between other
    /// components.
    /// </summary>
    public class BlankSpaceComponent : BlankSpaceComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.BlankSpaceComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~BlankSpaceComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Blank Space Component.
        /// </summary>
        public BlankSpaceComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.BlankSpaceComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.BlankSpaceComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal BlankSpaceComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class BlankSpaceComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// The size of the component.
        /// </summary>
        public uint Size()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.BlankSpaceComponentState_size(this.ptr);
            return result;
        }
        internal BlankSpaceComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class BlankSpaceComponentStateRefMut : BlankSpaceComponentStateRef
    {
        internal BlankSpaceComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class BlankSpaceComponentState : BlankSpaceComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.BlankSpaceComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~BlankSpaceComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal BlankSpaceComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Component provides information about a run in a way that is easy to
    /// visualize. This type can store any of the components provided by this crate.
    /// </summary>
    public class ComponentRef
    {
        internal IntPtr ptr;
        internal ComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Component provides information about a run in a way that is easy to
    /// visualize. This type can store any of the components provided by this crate.
    /// </summary>
    public class ComponentRefMut : ComponentRef
    {
        internal ComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Component provides information about a run in a way that is easy to
    /// visualize. This type can store any of the components provided by this crate.
    /// </summary>
    public class Component : ComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.Component_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~Component()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal Component(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Current Comparison Component is a component that shows the name of the
    /// comparison that is currently selected to be compared against.
    /// </summary>
    public class CurrentComparisonComponentRef
    {
        internal IntPtr ptr;
        internal CurrentComparisonComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Current Comparison Component is a component that shows the name of the
    /// comparison that is currently selected to be compared against.
    /// </summary>
    public class CurrentComparisonComponentRefMut : CurrentComparisonComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.CurrentComparisonComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.CurrentComparisonComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal CurrentComparisonComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Current Comparison Component is a component that shows the name of the
    /// comparison that is currently selected to be compared against.
    /// </summary>
    public class CurrentComparisonComponent : CurrentComparisonComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.CurrentComparisonComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~CurrentComparisonComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Current Comparison Component.
        /// </summary>
        public CurrentComparisonComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.CurrentComparisonComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.CurrentComparisonComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal CurrentComparisonComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Current Pace Component is a component that shows a prediction of the
    /// current attempt's final time, if the current attempt's pace matches the
    /// chosen comparison for the remainder of the run.
    /// </summary>
    public class CurrentPaceComponentRef
    {
        internal IntPtr ptr;
        internal CurrentPaceComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Current Pace Component is a component that shows a prediction of the
    /// current attempt's final time, if the current attempt's pace matches the
    /// chosen comparison for the remainder of the run.
    /// </summary>
    public class CurrentPaceComponentRefMut : CurrentPaceComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.CurrentPaceComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.CurrentPaceComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal CurrentPaceComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Current Pace Component is a component that shows a prediction of the
    /// current attempt's final time, if the current attempt's pace matches the
    /// chosen comparison for the remainder of the run.
    /// </summary>
    public class CurrentPaceComponent : CurrentPaceComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.CurrentPaceComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~CurrentPaceComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Current Pace Component.
        /// </summary>
        public CurrentPaceComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.CurrentPaceComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.CurrentPaceComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal CurrentPaceComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Delta Component is a component that shows the how far ahead or behind
    /// the current attempt is compared to the chosen comparison.
    /// </summary>
    public class DeltaComponentRef
    {
        internal IntPtr ptr;
        internal DeltaComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Delta Component is a component that shows the how far ahead or behind
    /// the current attempt is compared to the chosen comparison.
    /// </summary>
    public class DeltaComponentRefMut : DeltaComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = LiveSplitCoreNative.DeltaComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer and the layout
        /// settings provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.DeltaComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal DeltaComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Delta Component is a component that shows the how far ahead or behind
    /// the current attempt is compared to the chosen comparison.
    /// </summary>
    public class DeltaComponent : DeltaComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.DeltaComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~DeltaComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Delta Component.
        /// </summary>
        public DeltaComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.DeltaComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.DeltaComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal DeltaComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Detailed Timer Component is a component that shows two timers, one for
    /// the total time of the current attempt and one showing the time of just the
    /// current segment. Other information, like segment times of up to two
    /// comparisons, the segment icon, and the segment's name, can also be shown.
    /// </summary>
    public class DetailedTimerComponentRef
    {
        internal IntPtr ptr;
        internal DetailedTimerComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Detailed Timer Component is a component that shows two timers, one for
    /// the total time of the current attempt and one showing the time of just the
    /// current segment. Other information, like segment times of up to two
    /// comparisons, the segment icon, and the segment's name, can also be shown.
    /// </summary>
    public class DetailedTimerComponentRefMut : DetailedTimerComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer and layout settings
        /// provided.
        /// </summary>
        public DetailedTimerComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = new DetailedTimerComponentState(LiveSplitCoreNative.DetailedTimerComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal DetailedTimerComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Detailed Timer Component is a component that shows two timers, one for
    /// the total time of the current attempt and one showing the time of just the
    /// current segment. Other information, like segment times of up to two
    /// comparisons, the segment icon, and the segment's name, can also be shown.
    /// </summary>
    public class DetailedTimerComponent : DetailedTimerComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.DetailedTimerComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~DetailedTimerComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Detailed Timer Component.
        /// </summary>
        public DetailedTimerComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.DetailedTimerComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.DetailedTimerComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal DetailedTimerComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class DetailedTimerComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// The time shown by the component's main timer without the fractional part.
        /// </summary>
        public string TimerTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_timer_time(this.ptr);
            return result;
        }
        /// <summary>
        /// The fractional part of the time shown by the main timer (including the dot).
        /// </summary>
        public string TimerFraction()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_timer_fraction(this.ptr);
            return result;
        }
        /// <summary>
        /// The semantic coloring information the main timer's time carries.
        /// </summary>
        public string TimerSemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_timer_semantic_color(this.ptr);
            return result;
        }
        /// <summary>
        /// The time shown by the component's segment timer without the fractional part.
        /// </summary>
        public string SegmentTimerTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_segment_timer_time(this.ptr);
            return result;
        }
        /// <summary>
        /// The fractional part of the time shown by the segment timer (including the
        /// dot).
        /// </summary>
        public string SegmentTimerFraction()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_segment_timer_fraction(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns whether the first comparison is visible.
        /// </summary>
        public bool Comparison1Visible()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison1_visible(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns the name of the first comparison. You may not call this if the first
        /// comparison is not visible.
        /// </summary>
        public string Comparison1Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison1_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the time of the first comparison. You may not call this if the first
        /// comparison is not visible.
        /// </summary>
        public string Comparison1Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison1_time(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns whether the second comparison is visible.
        /// </summary>
        public bool Comparison2Visible()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison2_visible(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns the name of the second comparison. You may not call this if the
        /// second comparison is not visible.
        /// </summary>
        public string Comparison2Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison2_name(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the time of the second comparison. You may not call this if the
        /// second comparison is not visible.
        /// </summary>
        public string Comparison2Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison2_time(this.ptr);
            return result;
        }
        /// <summary>
        /// The data of the segment's icon. This value is only specified whenever the
        /// icon changes. If you explicitly want to query this value, remount the
        /// component. The buffer itself may be empty. This indicates that there is no
        /// icon.
        /// </summary>
        public IntPtr IconChangePtr()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_icon_change_ptr(this.ptr);
            return result;
        }
        /// <summary>
        /// The length of the data of the segment's icon.
        /// </summary>
        public long IconChangeLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.DetailedTimerComponentState_icon_change_len(this.ptr);
            return result;
        }
        /// <summary>
        /// The name of the segment. This may be null if it's not supposed to be
        /// visualized.
        /// </summary>
        public string SegmentName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_segment_name(this.ptr);
            return result;
        }
        internal DetailedTimerComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class DetailedTimerComponentStateRefMut : DetailedTimerComponentStateRef
    {
        internal DetailedTimerComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class DetailedTimerComponentState : DetailedTimerComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.DetailedTimerComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~DetailedTimerComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal DetailedTimerComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// With a Fuzzy List, you can implement a fuzzy searching algorithm. The list
    /// stores all the items that can be searched for. With the `search` method you
    /// can then execute the actual fuzzy search which returns a list of all the
    /// elements found. This can be used to implement searching in a list of games.
    /// </summary>
    public class FuzzyListRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Searches for the pattern provided in the list. A list of all the
        /// matching elements is returned. The returned list has a maximum amount of
        /// elements provided to this method.
        /// </summary>
        public string Search(string pattern, long max)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.FuzzyList_search(this.ptr, pattern, (UIntPtr)max);
            return result;
        }
        internal FuzzyListRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// With a Fuzzy List, you can implement a fuzzy searching algorithm. The list
    /// stores all the items that can be searched for. With the `search` method you
    /// can then execute the actual fuzzy search which returns a list of all the
    /// elements found. This can be used to implement searching in a list of games.
    /// </summary>
    public class FuzzyListRefMut : FuzzyListRef
    {
        /// <summary>
        /// Adds a new element to the list.
        /// </summary>
        public void Push(string text)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.FuzzyList_push(this.ptr, text);
        }
        internal FuzzyListRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// With a Fuzzy List, you can implement a fuzzy searching algorithm. The list
    /// stores all the items that can be searched for. With the `search` method you
    /// can then execute the actual fuzzy search which returns a list of all the
    /// elements found. This can be used to implement searching in a list of games.
    /// </summary>
    public class FuzzyList : FuzzyListRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.FuzzyList_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~FuzzyList()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Fuzzy List.
        /// </summary>
        public FuzzyList() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.FuzzyList_new();
        }
        internal FuzzyList(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The general settings of the layout that apply to all components.
    /// </summary>
    public class GeneralLayoutSettingsRef
    {
        internal IntPtr ptr;
        internal GeneralLayoutSettingsRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The general settings of the layout that apply to all components.
    /// </summary>
    public class GeneralLayoutSettingsRefMut : GeneralLayoutSettingsRef
    {
        internal GeneralLayoutSettingsRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The general settings of the layout that apply to all components.
    /// </summary>
    public class GeneralLayoutSettings : GeneralLayoutSettingsRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.GeneralLayoutSettings_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~GeneralLayoutSettings()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a default general layout settings configuration.
        /// </summary>
        public static GeneralLayoutSettings Default()
        {
            var result = new GeneralLayoutSettings(LiveSplitCoreNative.GeneralLayoutSettings_default());
            return result;
        }
        internal GeneralLayoutSettings(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Graph Component visualizes how far the current attempt has been ahead or
    /// behind the chosen comparison throughout the whole attempt. All the
    /// individual deltas are shown as points in a graph.
    /// </summary>
    public class GraphComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = LiveSplitCoreNative.GraphComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer and layout settings
        /// provided.
        /// </summary>
        public GraphComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = new GraphComponentState(LiveSplitCoreNative.GraphComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal GraphComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Graph Component visualizes how far the current attempt has been ahead or
    /// behind the chosen comparison throughout the whole attempt. All the
    /// individual deltas are shown as points in a graph.
    /// </summary>
    public class GraphComponentRefMut : GraphComponentRef
    {
        internal GraphComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Graph Component visualizes how far the current attempt has been ahead or
    /// behind the chosen comparison throughout the whole attempt. All the
    /// individual deltas are shown as points in a graph.
    /// </summary>
    public class GraphComponent : GraphComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.GraphComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~GraphComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Graph Component.
        /// </summary>
        public GraphComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.GraphComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.GraphComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal GraphComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// All the coordinates are in the range 0..1.
    /// </summary>
    public class GraphComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Returns the amount of points to visualize. Connect all of them to visualize
        /// the graph. If the live delta is active, the last point is to be interpreted
        /// as a preview of the next split that is about to happen. Use the partial fill
        /// color to visualize the region beneath that graph segment.
        /// </summary>
        public long PointsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.GraphComponentState_points_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the x coordinate of the point specified. You may not provide an out
        /// of bounds index.
        /// </summary>
        public float PointX(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_point_x(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// Returns the y coordinate of the point specified. You may not provide an out
        /// of bounds index.
        /// </summary>
        public float PointY(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_point_y(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// Describes whether the segment the point specified is visualizing achieved a
        /// new best segment time. Use the best segment color for it, in that case. You
        /// may not provide an out of bounds index.
        /// </summary>
        public bool PointIsBestSegment(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_point_is_best_segment(this.ptr, (UIntPtr)index) != 0;
            return result;
        }
        /// <summary>
        /// Describes how many horizontal grid lines to visualize.
        /// </summary>
        public long HorizontalGridLinesLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.GraphComponentState_horizontal_grid_lines_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the y coordinate of the horizontal grid line specified. You may not
        /// provide an out of bounds index.
        /// </summary>
        public float HorizontalGridLine(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_horizontal_grid_line(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// Describes how many vertical grid lines to visualize.
        /// </summary>
        public long VerticalGridLinesLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.GraphComponentState_vertical_grid_lines_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the x coordinate of the vertical grid line specified. You may not
        /// provide an out of bounds index.
        /// </summary>
        public float VerticalGridLine(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_vertical_grid_line(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// The y coordinate that separates the region that shows the times that are
        /// ahead of the comparison and those that are behind.
        /// </summary>
        public float Middle()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_middle(this.ptr);
            return result;
        }
        /// <summary>
        /// If the live delta is active, the last point is to be interpreted as a
        /// preview of the next split that is about to happen. Use the partial fill
        /// color to visualize the region beneath that graph segment.
        /// </summary>
        public bool IsLiveDeltaActive()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_is_live_delta_active(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Describes whether the graph is flipped vertically. For visualizing the
        /// graph, this usually doesn't need to be interpreted, as this information is
        /// entirely encoded into the other variables.
        /// </summary>
        public bool IsFlipped()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_is_flipped(this.ptr) != 0;
            return result;
        }
        internal GraphComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// All the coordinates are in the range 0..1.
    /// </summary>
    public class GraphComponentStateRefMut : GraphComponentStateRef
    {
        internal GraphComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// All the coordinates are in the range 0..1.
    /// </summary>
    public class GraphComponentState : GraphComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.GraphComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~GraphComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal GraphComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The configuration to use for a Hotkey System. It describes with keys to use
    /// as hotkeys for the different actions.
    /// </summary>
    public class HotkeyConfigRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes generic description of the settings available for the hotkey
        /// configuration and their current values as JSON.
        /// </summary>
        public string SettingsDescriptionAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.HotkeyConfig_settings_description_as_json(this.ptr);
            return result;
        }
        /// <summary>
        /// Encodes the hotkey configuration as JSON.
        /// </summary>
        public string AsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.HotkeyConfig_as_json(this.ptr);
            return result;
        }
        internal HotkeyConfigRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The configuration to use for a Hotkey System. It describes with keys to use
    /// as hotkeys for the different actions.
    /// </summary>
    public class HotkeyConfigRefMut : HotkeyConfigRef
    {
        /// <summary>
        /// Sets a setting's value by its index to the given value.
        /// 
        /// false is returned if a hotkey is already in use by a different action.
        /// 
        /// This panics if the type of the value to be set is not compatible with the
        /// type of the setting's value. A panic can also occur if the index of the
        /// setting provided is out of bounds.
        /// </summary>
        public bool SetValue(long index, SettingValue value)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (value.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("value");
            }
            var result = LiveSplitCoreNative.HotkeyConfig_set_value(this.ptr, (UIntPtr)index, value.ptr) != 0;
            value.ptr = IntPtr.Zero;
            return result;
        }
        internal HotkeyConfigRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The configuration to use for a Hotkey System. It describes with keys to use
    /// as hotkeys for the different actions.
    /// </summary>
    public class HotkeyConfig : HotkeyConfigRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.HotkeyConfig_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~HotkeyConfig()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Hotkey Configuration with default settings.
        /// </summary>
        public HotkeyConfig() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.HotkeyConfig_new();
        }
        /// <summary>
        /// Parses a hotkey configuration from the given JSON description. null is
        /// returned if it couldn't be parsed.
        /// </summary>
        public static HotkeyConfig ParseJson(string settings)
        {
            var result = new HotkeyConfig(LiveSplitCoreNative.HotkeyConfig_parse_json(settings));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Attempts to parse a hotkey configuration from a given file. null is
        /// returned it couldn't be parsed. This will not close the file descriptor /
        /// handle.
        /// </summary>
        public static HotkeyConfig ParseFileHandle(long handle)
        {
            var result = new HotkeyConfig(LiveSplitCoreNative.HotkeyConfig_parse_file_handle(handle));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal HotkeyConfig(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// With a Hotkey System the runner can use hotkeys on their keyboard to control
    /// the Timer. The hotkeys are global, so the application doesn't need to be in
    /// focus. The behavior of the hotkeys depends on the platform and is stubbed
    /// out on platforms that don't support hotkeys. You can turn off a Hotkey
    /// System temporarily. By default the Hotkey System is activated.
    /// </summary>
    public class HotkeySystemRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Deactivates the Hotkey System. No hotkeys will go through until it gets
        /// activated again. If it's already deactivated, nothing happens.
        /// </summary>
        public void Deactivate()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.HotkeySystem_deactivate(this.ptr);
        }
        /// <summary>
        /// Activates a previously deactivated Hotkey System. If it's already
        /// active, nothing happens.
        /// </summary>
        public void Activate()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.HotkeySystem_activate(this.ptr);
        }
        /// <summary>
        /// Returns the hotkey configuration currently in use by the Hotkey System.
        /// </summary>
        public HotkeyConfig Config()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new HotkeyConfig(LiveSplitCoreNative.HotkeySystem_config(this.ptr));
            return result;
        }
        internal HotkeySystemRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// With a Hotkey System the runner can use hotkeys on their keyboard to control
    /// the Timer. The hotkeys are global, so the application doesn't need to be in
    /// focus. The behavior of the hotkeys depends on the platform and is stubbed
    /// out on platforms that don't support hotkeys. You can turn off a Hotkey
    /// System temporarily. By default the Hotkey System is activated.
    /// </summary>
    public class HotkeySystemRefMut : HotkeySystemRef
    {
        /// <summary>
        /// Applies a new hotkey configuration to the Hotkey System. Each hotkey is
        /// changed to the one specified in the configuration. This operation may fail
        /// if you provide a hotkey configuration where a hotkey is used for multiple
        /// operations. Returns false if the operation failed.
        /// </summary>
        public bool SetConfig(HotkeyConfig config)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (config.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("config");
            }
            var result = LiveSplitCoreNative.HotkeySystem_set_config(this.ptr, config.ptr) != 0;
            config.ptr = IntPtr.Zero;
            return result;
        }
        internal HotkeySystemRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// With a Hotkey System the runner can use hotkeys on their keyboard to control
    /// the Timer. The hotkeys are global, so the application doesn't need to be in
    /// focus. The behavior of the hotkeys depends on the platform and is stubbed
    /// out on platforms that don't support hotkeys. You can turn off a Hotkey
    /// System temporarily. By default the Hotkey System is activated.
    /// </summary>
    public class HotkeySystem : HotkeySystemRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.HotkeySystem_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~HotkeySystem()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Hotkey System for a Timer with the default hotkeys.
        /// </summary>
        public static HotkeySystem New(SharedTimer sharedTimer)
        {
            if (sharedTimer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("sharedTimer");
            }
            var result = new HotkeySystem(LiveSplitCoreNative.HotkeySystem_new(sharedTimer.ptr));
            sharedTimer.ptr = IntPtr.Zero;
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new Hotkey System for a Timer with a custom configuration for the
        /// hotkeys.
        /// </summary>
        public static HotkeySystem WithConfig(SharedTimer sharedTimer, HotkeyConfig config)
        {
            if (sharedTimer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("sharedTimer");
            }
            if (config.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("config");
            }
            var result = new HotkeySystem(LiveSplitCoreNative.HotkeySystem_with_config(sharedTimer.ptr, config.ptr));
            sharedTimer.ptr = IntPtr.Zero;
            config.ptr = IntPtr.Zero;
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal HotkeySystem(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for a key value based component.
    /// </summary>
    public class KeyValueComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// The key to visualize.
        /// </summary>
        public string Key()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.KeyValueComponentState_key(this.ptr);
            return result;
        }
        /// <summary>
        /// The value to visualize.
        /// </summary>
        public string Value()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.KeyValueComponentState_value(this.ptr);
            return result;
        }
        /// <summary>
        /// The semantic coloring information the value carries.
        /// </summary>
        public string SemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.KeyValueComponentState_semantic_color(this.ptr);
            return result;
        }
        internal KeyValueComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for a key value based component.
    /// </summary>
    public class KeyValueComponentStateRefMut : KeyValueComponentStateRef
    {
        internal KeyValueComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for a key value based component.
    /// </summary>
    public class KeyValueComponentState : KeyValueComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.KeyValueComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~KeyValueComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal KeyValueComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Layout allows you to combine multiple components together to visualize a
    /// variety of information the runner is interested in.
    /// </summary>
    public class LayoutRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Clones the layout.
        /// </summary>
        public Layout Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Layout(LiveSplitCoreNative.Layout_clone(this.ptr));
            return result;
        }
        /// <summary>
        /// Encodes the settings of the layout as JSON.
        /// </summary>
        public string SettingsAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Layout_settings_as_json(this.ptr);
            return result;
        }
        internal LayoutRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Layout allows you to combine multiple components together to visualize a
    /// variety of information the runner is interested in.
    /// </summary>
    public class LayoutRefMut : LayoutRef
    {
        /// <summary>
        /// Calculates and returns the layout's state based on the timer provided.
        /// </summary>
        public LayoutState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new LayoutState(LiveSplitCoreNative.Layout_state(this.ptr, timer.ptr));
            return result;
        }
        /// <summary>
        /// Calculates the layout's state based on the timer provided and encodes it as
        /// JSON. You can use this to visualize all of the components of a layout.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.Layout_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Adds a new component to the end of the layout.
        /// </summary>
        public void Push(Component component)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (component.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("component");
            }
            LiveSplitCoreNative.Layout_push(this.ptr, component.ptr);
            component.ptr = IntPtr.Zero;
        }
        /// <summary>
        /// Scrolls up all the components in the layout that can be scrolled up.
        /// </summary>
        public void ScrollUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Layout_scroll_up(this.ptr);
        }
        /// <summary>
        /// Scrolls down all the components in the layout that can be scrolled down.
        /// </summary>
        public void ScrollDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Layout_scroll_down(this.ptr);
        }
        /// <summary>
        /// Remounts all the components as if they were freshly initialized. Some
        /// components may only provide some information whenever it changes or when
        /// their state is first queried. Remounting returns this information again,
        /// whenever the layout's state is queried the next time.
        /// </summary>
        public void Remount()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Layout_remount(this.ptr);
        }
        internal LayoutRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Layout allows you to combine multiple components together to visualize a
    /// variety of information the runner is interested in.
    /// </summary>
    public class Layout : LayoutRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.Layout_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~Layout()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new empty layout with no components.
        /// </summary>
        public Layout() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.Layout_new();
        }
        /// <summary>
        /// Creates a new default layout that contains a default set of components
        /// in order to provide a good default layout for runners. Which components
        /// are provided by this and how they are configured may change in the
        /// future.
        /// </summary>
        public static Layout DefaultLayout()
        {
            var result = new Layout(LiveSplitCoreNative.Layout_default_layout());
            return result;
        }
        /// <summary>
        /// Parses a layout from the given JSON description of its settings. null is
        /// returned if it couldn't be parsed.
        /// </summary>
        public static Layout ParseJson(string settings)
        {
            var result = new Layout(LiveSplitCoreNative.Layout_parse_json(settings));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Attempts to parse a layout from a given file. null is returned it couldn't
        /// be parsed. This will not close the file descriptor / handle.
        /// </summary>
        public static Layout ParseFileHandle(long handle)
        {
            var result = new Layout(LiveSplitCoreNative.Layout_parse_file_handle(handle));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Parses a layout saved by the original LiveSplit. This is lossy, as not
        /// everything can be converted completely. null is returned if it couldn't be
        /// parsed at all.
        /// </summary>
        public static Layout ParseOriginalLivesplit(IntPtr data, long length)
        {
            var result = new Layout(LiveSplitCoreNative.Layout_parse_original_livesplit(data, (UIntPtr)length));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal Layout(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Layout Editor allows modifying Layouts while ensuring all the different
    /// invariants of the Layout objects are upheld no matter what kind of
    /// operations are being applied. It provides the current state of the editor as
    /// state objects that can be visualized by any kind of User Interface.
    /// </summary>
    public class LayoutEditorRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the Layout Editor's state as JSON in order to visualize it.
        /// </summary>
        public string StateAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.LayoutEditor_state_as_json(this.ptr);
            return result;
        }
        internal LayoutEditorRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Layout Editor allows modifying Layouts while ensuring all the different
    /// invariants of the Layout objects are upheld no matter what kind of
    /// operations are being applied. It provides the current state of the editor as
    /// state objects that can be visualized by any kind of User Interface.
    /// </summary>
    public class LayoutEditorRefMut : LayoutEditorRef
    {
        /// <summary>
        /// Encodes the layout's state as JSON based on the timer provided. You can use
        /// this to visualize all of the components of a layout, while it is still being
        /// edited by the Layout Editor.
        /// </summary>
        public string LayoutStateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.LayoutEditor_layout_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Selects the component with the given index in order to modify its
        /// settings. Only a single component is selected at any given time. You may
        /// not provide an invalid index.
        /// </summary>
        public void Select(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_select(this.ptr, (UIntPtr)index);
        }
        /// <summary>
        /// Adds the component provided to the end of the layout. The newly added
        /// component becomes the selected component.
        /// </summary>
        public void AddComponent(Component component)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (component.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("component");
            }
            LiveSplitCoreNative.LayoutEditor_add_component(this.ptr, component.ptr);
            component.ptr = IntPtr.Zero;
        }
        /// <summary>
        /// Removes the currently selected component, unless there's only one
        /// component in the layout. The next component becomes the selected
        /// component. If there's none, the previous component becomes the selected
        /// component instead.
        /// </summary>
        public void RemoveComponent()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_remove_component(this.ptr);
        }
        /// <summary>
        /// Moves the selected component up, unless the first component is selected.
        /// </summary>
        public void MoveComponentUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_move_component_up(this.ptr);
        }
        /// <summary>
        /// Moves the selected component down, unless the last component is
        /// selected.
        /// </summary>
        public void MoveComponentDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_move_component_down(this.ptr);
        }
        /// <summary>
        /// Moves the selected component to the index provided. You may not provide
        /// an invalid index.
        /// </summary>
        public void MoveComponent(long dstIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_move_component(this.ptr, (UIntPtr)dstIndex);
        }
        /// <summary>
        /// Duplicates the currently selected component. The copy gets placed right
        /// after the selected component and becomes the newly selected component.
        /// </summary>
        public void DuplicateComponent()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_duplicate_component(this.ptr);
        }
        /// <summary>
        /// Sets a setting's value of the selected component by its setting index
        /// to the given value.
        /// 
        /// This panics if the type of the value to be set is not compatible with
        /// the type of the setting's value. A panic can also occur if the index of
        /// the setting provided is out of bounds.
        /// </summary>
        public void SetComponentSettingsValue(long index, SettingValue value)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (value.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("value");
            }
            LiveSplitCoreNative.LayoutEditor_set_component_settings_value(this.ptr, (UIntPtr)index, value.ptr);
            value.ptr = IntPtr.Zero;
        }
        /// <summary>
        /// Sets a setting's value of the general settings by its setting index to
        /// the given value.
        /// 
        /// This panics if the type of the value to be set is not compatible with
        /// the type of the setting's value. A panic can also occur if the index of
        /// the setting provided is out of bounds.
        /// </summary>
        public void SetGeneralSettingsValue(long index, SettingValue value)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (value.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("value");
            }
            LiveSplitCoreNative.LayoutEditor_set_general_settings_value(this.ptr, (UIntPtr)index, value.ptr);
            value.ptr = IntPtr.Zero;
        }
        internal LayoutEditorRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Layout Editor allows modifying Layouts while ensuring all the different
    /// invariants of the Layout objects are upheld no matter what kind of
    /// operations are being applied. It provides the current state of the editor as
    /// state objects that can be visualized by any kind of User Interface.
    /// </summary>
    public class LayoutEditor : LayoutEditorRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~LayoutEditor()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Layout Editor that modifies the Layout provided. Creation of
        /// the Layout Editor fails when a Layout with no components is provided. In
        /// that case null is returned instead.
        /// </summary>
        public static LayoutEditor New(Layout layout)
        {
            if (layout.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layout");
            }
            var result = new LayoutEditor(LiveSplitCoreNative.LayoutEditor_new(layout.ptr));
            layout.ptr = IntPtr.Zero;
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Closes the Layout Editor and gives back access to the modified Layout. In
        /// case you want to implement a Cancel Button, just dispose the Layout object
        /// you get here.
        /// </summary>
        public Layout Close()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Layout(LiveSplitCoreNative.LayoutEditor_close(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal LayoutEditor(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for an entire
    /// layout. Use this with care, as invalid usage will result in a panic.
    /// 
    /// Specifically, you should avoid doing the following:
    /// 
    /// - Using out of bounds indices.
    /// - Using the wrong getter function on the wrong type of component.
    /// </summary>
    public class LayoutStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Gets the number of Components in the Layout State.
        /// </summary>
        public long Len()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.LayoutState_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns a string describing the type of the Component at the specified
        /// index.
        /// </summary>
        public string ComponentType(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.LayoutState_component_type(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// Gets the Blank Space component state at the specified index.
        /// </summary>
        public BlankSpaceComponentStateRef ComponentAsBlankSpace(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new BlankSpaceComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_blank_space(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Detailed Timer component state at the specified index.
        /// </summary>
        public DetailedTimerComponentStateRef ComponentAsDetailedTimer(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new DetailedTimerComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_detailed_timer(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Graph component state at the specified index.
        /// </summary>
        public GraphComponentStateRef ComponentAsGraph(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new GraphComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_graph(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Key Value component state at the specified index.
        /// </summary>
        public KeyValueComponentStateRef ComponentAsKeyValue(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new KeyValueComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_key_value(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Separator component state at the specified index.
        /// </summary>
        public SeparatorComponentStateRef ComponentAsSeparator(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SeparatorComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_separator(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Splits component state at the specified index.
        /// </summary>
        public SplitsComponentStateRef ComponentAsSplits(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SplitsComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_splits(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Text component state at the specified index.
        /// </summary>
        public TextComponentStateRef ComponentAsText(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TextComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_text(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Timer component state at the specified index.
        /// </summary>
        public TimerComponentStateRef ComponentAsTimer(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_timer(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Gets the Title component state at the specified index.
        /// </summary>
        public TitleComponentStateRef ComponentAsTitle(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TitleComponentStateRef(LiveSplitCoreNative.LayoutState_component_as_title(this.ptr, (UIntPtr)index));
            return result;
        }
        internal LayoutStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for an entire
    /// layout. Use this with care, as invalid usage will result in a panic.
    /// 
    /// Specifically, you should avoid doing the following:
    /// 
    /// - Using out of bounds indices.
    /// - Using the wrong getter function on the wrong type of component.
    /// </summary>
    public class LayoutStateRefMut : LayoutStateRef
    {
        internal LayoutStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for an entire
    /// layout. Use this with care, as invalid usage will result in a panic.
    /// 
    /// Specifically, you should avoid doing the following:
    /// 
    /// - Using out of bounds indices.
    /// - Using the wrong getter function on the wrong type of component.
    /// </summary>
    public class LayoutState : LayoutStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.LayoutState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~LayoutState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal LayoutState(IntPtr ptr) : base(ptr) { }
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
    /// The PB Chance Component is a component that shows how likely it is to beat
    /// the Personal Best. If there is no active attempt it shows the general chance
    /// of beating the Personal Best. During an attempt it actively changes based on
    /// how well the attempt is going.
    /// </summary>
    public class PbChanceComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.PbChanceComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.PbChanceComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal PbChanceComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The PB Chance Component is a component that shows how likely it is to beat
    /// the Personal Best. If there is no active attempt it shows the general chance
    /// of beating the Personal Best. During an attempt it actively changes based on
    /// how well the attempt is going.
    /// </summary>
    public class PbChanceComponentRefMut : PbChanceComponentRef
    {
        internal PbChanceComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The PB Chance Component is a component that shows how likely it is to beat
    /// the Personal Best. If there is no active attempt it shows the general chance
    /// of beating the Personal Best. During an attempt it actively changes based on
    /// how well the attempt is going.
    /// </summary>
    public class PbChanceComponent : PbChanceComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.PbChanceComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~PbChanceComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new PB Chance Component.
        /// </summary>
        public PbChanceComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.PbChanceComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.PbChanceComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal PbChanceComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Possible Time Save Component is a component that shows how much time the
    /// chosen comparison could've saved for the current segment, based on the Best
    /// Segments. This component also allows showing the Total Possible Time Save
    /// for the remainder of the current attempt.
    /// </summary>
    public class PossibleTimeSaveComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.PossibleTimeSaveComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.PossibleTimeSaveComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal PossibleTimeSaveComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Possible Time Save Component is a component that shows how much time the
    /// chosen comparison could've saved for the current segment, based on the Best
    /// Segments. This component also allows showing the Total Possible Time Save
    /// for the remainder of the current attempt.
    /// </summary>
    public class PossibleTimeSaveComponentRefMut : PossibleTimeSaveComponentRef
    {
        internal PossibleTimeSaveComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Possible Time Save Component is a component that shows how much time the
    /// chosen comparison could've saved for the current segment, based on the Best
    /// Segments. This component also allows showing the Total Possible Time Save
    /// for the remainder of the current attempt.
    /// </summary>
    public class PossibleTimeSaveComponent : PossibleTimeSaveComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.PossibleTimeSaveComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~PossibleTimeSaveComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Possible Time Save Component.
        /// </summary>
        public PossibleTimeSaveComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.PossibleTimeSaveComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.PossibleTimeSaveComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal PossibleTimeSaveComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Describes a potential clean up that could be applied. You can query a
    /// message describing the details of this potential clean up. A potential clean
    /// up can then be turned into an actual clean up in order to apply it to the
    /// Run.
    /// </summary>
    public class PotentialCleanUpRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the message describing the potential clean up that can be applied
        /// to a Run.
        /// </summary>
        public string Message()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PotentialCleanUp_message(this.ptr);
            return result;
        }
        internal PotentialCleanUpRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// Describes a potential clean up that could be applied. You can query a
    /// message describing the details of this potential clean up. A potential clean
    /// up can then be turned into an actual clean up in order to apply it to the
    /// Run.
    /// </summary>
    public class PotentialCleanUpRefMut : PotentialCleanUpRef
    {
        internal PotentialCleanUpRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Describes a potential clean up that could be applied. You can query a
    /// message describing the details of this potential clean up. A potential clean
    /// up can then be turned into an actual clean up in order to apply it to the
    /// Run.
    /// </summary>
    public class PotentialCleanUp : PotentialCleanUpRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.PotentialCleanUp_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~PotentialCleanUp()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal PotentialCleanUp(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Previous Segment Component is a component that shows how much time was
    /// saved or lost during the previous segment based on the chosen comparison.
    /// Additionally, the potential time save for the previous segment can be
    /// displayed. This component switches to a `Live Segment` view that shows
    /// active time loss whenever the runner is losing time on the current segment.
    /// </summary>
    public class PreviousSegmentComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = LiveSplitCoreNative.PreviousSegmentComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer and the layout
        /// settings provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.PreviousSegmentComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal PreviousSegmentComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Previous Segment Component is a component that shows how much time was
    /// saved or lost during the previous segment based on the chosen comparison.
    /// Additionally, the potential time save for the previous segment can be
    /// displayed. This component switches to a `Live Segment` view that shows
    /// active time loss whenever the runner is losing time on the current segment.
    /// </summary>
    public class PreviousSegmentComponentRefMut : PreviousSegmentComponentRef
    {
        internal PreviousSegmentComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Previous Segment Component is a component that shows how much time was
    /// saved or lost during the previous segment based on the chosen comparison.
    /// Additionally, the potential time save for the previous segment can be
    /// displayed. This component switches to a `Live Segment` view that shows
    /// active time loss whenever the runner is losing time on the current segment.
    /// </summary>
    public class PreviousSegmentComponent : PreviousSegmentComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.PreviousSegmentComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~PreviousSegmentComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Previous Segment Component.
        /// </summary>
        public PreviousSegmentComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.PreviousSegmentComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.PreviousSegmentComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal PreviousSegmentComponent(IntPtr ptr) : base(ptr) { }
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
        public long GameIconLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_game_icon_len(this.ptr);
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
        public long Len()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_len(this.ptr);
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
        public SegmentRef Segment(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentRef(LiveSplitCoreNative.Run_segment(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Returns the amount attempt history elements are stored in this Run.
        /// </summary>
        public long AttemptHistoryLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_attempt_history_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the an attempt history element by its index. This does not store
        /// the actual segment times, just the overall attempt information. Information
        /// about the individual segments is stored within each segment. You may not
        /// provide an out of bounds index.
        /// </summary>
        public AttemptRef AttemptHistoryIndex(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new AttemptRef(LiveSplitCoreNative.Run_attempt_history_index(this.ptr, (UIntPtr)index));
            return result;
        }
        /// <summary>
        /// Saves a Run as a LiveSplit splits file (*.lss). If the run is actively in
        /// use by a timer, use the appropriate method on the timer instead, in order to
        /// properly save the current attempt as well.
        /// </summary>
        public string SaveAsLss()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_save_as_lss(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the amount of custom comparisons stored in this Run.
        /// </summary>
        public long CustomComparisonsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_custom_comparisons_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses a custom comparison stored in this Run by its index. This includes
        /// `Personal Best` but excludes all the other Comparison Generators. You may
        /// not provide an out of bounds index.
        /// </summary>
        public string CustomComparison(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_custom_comparison(this.ptr, (UIntPtr)index);
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
        /// parser for the file format detected. A path to the splits file can be
        /// provided, which helps saving the splits file again later. Additionally you
        /// need to specify if additional files, like external images are allowed to be
        /// loaded. If you are using livesplit-core in a server-like environment, set
        /// this to false. Only client-side applications should set this to true.
        /// </summary>
        public static ParseRunResult Parse(IntPtr data, long length, string path, bool loadFiles)
        {
            var result = new ParseRunResult(LiveSplitCoreNative.Run_parse(data, (UIntPtr)length, path, loadFiles));
            return result;
        }
        /// <summary>
        /// Attempts to parse a splits file from a file by invoking the corresponding
        /// parser for the file format detected. A path to the splits file can be
        /// provided, which helps saving the splits file again later. Additionally you
        /// need to specify if additional files, like external images are allowed to be
        /// loaded. If you are using livesplit-core in a server-like environment, set
        /// this to false. Only client-side applications should set this to true. On
        /// Unix you pass a file descriptor to this function. On Windows you pass a file
        /// handle to this function. The file descriptor / handle does not get closed.
        /// </summary>
        public static ParseRunResult ParseFileHandle(long handle, string path, bool loadFiles)
        {
            var result = new ParseRunResult(LiveSplitCoreNative.Run_parse_file_handle(handle, path, loadFiles));
            return result;
        }
        public static ParseRunResult Parse(Stream stream, string path, bool loadFiles)
        {
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            IntPtr pnt = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, pnt, data.Length);
                return Parse(pnt, data.Length, path, loadFiles);
            }
            finally
            {
                Marshal.FreeHGlobal(pnt);
            }
        }
        internal Run(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Run Editor allows modifying Runs while ensuring that all the different
    /// invariants of the Run objects are upheld no matter what kind of operations
    /// are being applied to the Run. It provides the current state of the editor as
    /// state objects that can be visualized by any kind of User Interface.
    /// </summary>
    public class RunEditorRef
    {
        internal IntPtr ptr;
        internal RunEditorRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Run Editor allows modifying Runs while ensuring that all the different
    /// invariants of the Run objects are upheld no matter what kind of operations
    /// are being applied to the Run. It provides the current state of the editor as
    /// state objects that can be visualized by any kind of User Interface.
    /// </summary>
    public class RunEditorRefMut : RunEditorRef
    {
        /// <summary>
        /// Calculates the Run Editor's state and encodes it as
        /// JSON in order to visualize it.
        /// </summary>
        public string StateAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_state_as_json(this.ptr);
            return result;
        }
        /// <summary>
        /// Selects a different timing method for being modified.
        /// </summary>
        public void SelectTimingMethod(byte method)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_select_timing_method(this.ptr, method);
        }
        /// <summary>
        /// Unselects the segment with the given index. If it's not selected or the
        /// index is out of bounds, nothing happens. The segment is not unselected,
        /// when it is the only segment that is selected. If the active segment is
        /// unselected, the most recently selected segment remaining becomes the
        /// active segment.
        /// </summary>
        public void Unselect(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_unselect(this.ptr, (UIntPtr)index);
        }
        /// <summary>
        /// In addition to the segments that are already selected, the segment with
        /// the given index is being selected. The segment chosen also becomes the
        /// active segment.
        /// 
        /// This panics if the index of the segment provided is out of bounds.
        /// </summary>
        public void SelectAdditionally(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_select_additionally(this.ptr, (UIntPtr)index);
        }
        /// <summary>
        /// Selects the segment with the given index. All other segments are
        /// unselected. The segment chosen also becomes the active segment.
        /// 
        /// This panics if the index of the segment provided is out of bounds.
        /// </summary>
        public void SelectOnly(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_select_only(this.ptr, (UIntPtr)index);
        }
        /// <summary>
        /// Sets the name of the game.
        /// </summary>
        public void SetGameName(string game)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_game_name(this.ptr, game);
        }
        /// <summary>
        /// Sets the name of the category.
        /// </summary>
        public void SetCategoryName(string category)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_category_name(this.ptr, category);
        }
        /// <summary>
        /// Parses and sets the timer offset from the string provided. The timer
        /// offset specifies the time, the timer starts at when starting a new
        /// attempt.
        /// </summary>
        public bool ParseAndSetOffset(string offset)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_parse_and_set_offset(this.ptr, offset) != 0;
            return result;
        }
        /// <summary>
        /// Parses and sets the attempt count from the string provided. Changing
        /// this has no affect on the attempt history or the segment history. This
        /// number is mostly just a visual number for the runner.
        /// </summary>
        public bool ParseAndSetAttemptCount(string attempts)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_parse_and_set_attempt_count(this.ptr, attempts) != 0;
            return result;
        }
        /// <summary>
        /// Sets the game's icon.
        /// </summary>
        public void SetGameIcon(IntPtr data, long length)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_game_icon(this.ptr, data, (UIntPtr)length);
        }
        /// <summary>
        /// Removes the game's icon.
        /// </summary>
        public void RemoveGameIcon()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_game_icon(this.ptr);
        }
        /// <summary>
        /// Sets the speedrun.com Run ID of the run. You need to ensure that the
        /// record on speedrun.com matches up with the Personal Best of this run.
        /// This may be empty if there's no association.
        /// </summary>
        public void SetRunId(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_run_id(this.ptr, name);
        }
        /// <summary>
        /// Sets the name of the region this game is from. This may be empty if it's
        /// not specified.
        /// </summary>
        public void SetRegionName(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_region_name(this.ptr, name);
        }
        /// <summary>
        /// Sets the name of the platform this game is run on. This may be empty if
        /// it's not specified.
        /// </summary>
        public void SetPlatformName(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_platform_name(this.ptr, name);
        }
        /// <summary>
        /// Specifies whether this speedrun is done on an emulator. Keep in mind
        /// that false may also mean that this information is simply not known.
        /// </summary>
        public void SetEmulatorUsage(bool usesEmulator)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_emulator_usage(this.ptr, usesEmulator);
        }
        /// <summary>
        /// Sets the speedrun.com variable with the name specified to the value specified. A
        /// variable is an arbitrary key value pair storing additional information
        /// about the category. An example of this may be whether Amiibos are used
        /// in this category. If the variable doesn't exist yet, it is being
        /// inserted.
        /// </summary>
        public void SetSpeedrunComVariable(string name, string value)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_speedrun_com_variable(this.ptr, name, value);
        }
        /// <summary>
        /// Removes the speedrun.com variable with the name specified.
        /// </summary>
        public void RemoveSpeedrunComVariable(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_speedrun_com_variable(this.ptr, name);
        }
        /// <summary>
        /// Adds a new permanent custom variable. If there's a temporary variable with
        /// the same name, it gets turned into a permanent variable and its value stays.
        /// If a permanent variable with the name already exists, nothing happens.
        /// </summary>
        public void AddCustomVariable(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_add_custom_variable(this.ptr, name);
        }
        /// <summary>
        /// Sets the value of a custom variable with the name specified. If the custom
        /// variable does not exist, or is not a permanent variable, nothing happens.
        /// </summary>
        public void SetCustomVariable(string name, string value)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_custom_variable(this.ptr, name, value);
        }
        /// <summary>
        /// Removes the custom variable with the name specified. If the custom variable
        /// does not exist, or is not a permanent variable, nothing happens.
        /// </summary>
        public void RemoveCustomVariable(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_custom_variable(this.ptr, name);
        }
        /// <summary>
        /// Resets all the Metadata Information.
        /// </summary>
        public void ClearMetadata()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_clear_metadata(this.ptr);
        }
        /// <summary>
        /// Inserts a new empty segment above the active segment and adjusts the
        /// Run's history information accordingly. The newly created segment is then
        /// the only selected segment and also the active segment.
        /// </summary>
        public void InsertSegmentAbove()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_insert_segment_above(this.ptr);
        }
        /// <summary>
        /// Inserts a new empty segment below the active segment and adjusts the
        /// Run's history information accordingly. The newly created segment is then
        /// the only selected segment and also the active segment.
        /// </summary>
        public void InsertSegmentBelow()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_insert_segment_below(this.ptr);
        }
        /// <summary>
        /// Removes all the selected segments, unless all of them are selected. The
        /// run's information is automatically adjusted properly. The next
        /// not-to-be-removed segment after the active segment becomes the new
        /// active segment. If there's none, then the next not-to-be-removed segment
        /// before the active segment, becomes the new active segment.
        /// </summary>
        public void RemoveSegments()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_segments(this.ptr);
        }
        /// <summary>
        /// Moves all the selected segments up, unless the first segment is
        /// selected. The run's information is automatically adjusted properly. The
        /// active segment stays the active segment.
        /// </summary>
        public void MoveSegmentsUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_move_segments_up(this.ptr);
        }
        /// <summary>
        /// Moves all the selected segments down, unless the last segment is
        /// selected. The run's information is automatically adjusted properly. The
        /// active segment stays the active segment.
        /// </summary>
        public void MoveSegmentsDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_move_segments_down(this.ptr);
        }
        /// <summary>
        /// Sets the icon of the active segment.
        /// </summary>
        public void ActiveSetIcon(IntPtr data, long length)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_active_set_icon(this.ptr, data, (UIntPtr)length);
        }
        /// <summary>
        /// Removes the icon of the active segment.
        /// </summary>
        public void ActiveRemoveIcon()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_active_remove_icon(this.ptr);
        }
        /// <summary>
        /// Sets the name of the active segment.
        /// </summary>
        public void ActiveSetName(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_active_set_name(this.ptr, name);
        }
        /// <summary>
        /// Parses a split time from a string and sets it for the active segment with
        /// the chosen timing method.
        /// </summary>
        public bool ActiveParseAndSetSplitTime(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_active_parse_and_set_split_time(this.ptr, time) != 0;
            return result;
        }
        /// <summary>
        /// Parses a segment time from a string and sets it for the active segment with
        /// the chosen timing method.
        /// </summary>
        public bool ActiveParseAndSetSegmentTime(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_active_parse_and_set_segment_time(this.ptr, time) != 0;
            return result;
        }
        /// <summary>
        /// Parses a best segment time from a string and sets it for the active segment
        /// with the chosen timing method.
        /// </summary>
        public bool ActiveParseAndSetBestSegmentTime(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_active_parse_and_set_best_segment_time(this.ptr, time) != 0;
            return result;
        }
        /// <summary>
        /// Parses a comparison time for the provided comparison and sets it for the
        /// active active segment with the chosen timing method.
        /// </summary>
        public bool ActiveParseAndSetComparisonTime(string comparison, string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_active_parse_and_set_comparison_time(this.ptr, comparison, time) != 0;
            return result;
        }
        /// <summary>
        /// Adds a new custom comparison. It can't be added if it starts with
        /// `[Race]` or already exists.
        /// </summary>
        public bool AddComparison(string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_add_comparison(this.ptr, comparison) != 0;
            return result;
        }
        /// <summary>
        /// Imports the Personal Best from the provided run as a comparison. The
        /// comparison can't be added if its name starts with `[Race]` or it already
        /// exists.
        /// </summary>
        public bool ImportComparison(RunRef run, string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = LiveSplitCoreNative.RunEditor_import_comparison(this.ptr, run.ptr, comparison) != 0;
            return result;
        }
        /// <summary>
        /// Removes the chosen custom comparison. You can't remove a Comparison
        /// Generator's Comparison or the Personal Best.
        /// </summary>
        public void RemoveComparison(string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_comparison(this.ptr, comparison);
        }
        /// <summary>
        /// Renames a comparison. The comparison can't be renamed if the new name of
        /// the comparison starts with `[Race]` or it already exists.
        /// </summary>
        public bool RenameComparison(string oldName, string newName)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_rename_comparison(this.ptr, oldName, newName) != 0;
            return result;
        }
        /// <summary>
        /// Reorders the custom comparisons by moving the comparison with the source
        /// index specified to the destination index specified. Returns false if one
        /// of the indices is invalid. The indices are based on the comparison names of
        /// the Run Editor's state.
        /// </summary>
        public bool MoveComparison(long srcIndex, long dstIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_move_comparison(this.ptr, (UIntPtr)srcIndex, (UIntPtr)dstIndex) != 0;
            return result;
        }
        /// <summary>
        /// Parses a goal time and generates a custom goal comparison based on the
        /// parsed value. The comparison's times are automatically balanced based on the
        /// runner's history such that it roughly represents what split times for the
        /// goal time would roughly look like. Since it is populated by the runner's
        /// history, only goal times within the sum of the best segments and the sum of
        /// the worst segments are supported. Everything else is automatically capped by
        /// that range. The comparison is only populated for the selected timing method.
        /// The other timing method's comparison times are not modified by this, so you
        /// can call this again with the other timing method to generate the comparison
        /// times for both timing methods.
        /// </summary>
        public bool ParseAndGenerateGoalComparison(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_parse_and_generate_goal_comparison(this.ptr, time) != 0;
            return result;
        }
        /// <summary>
        /// Clears out the Attempt History and the Segment Histories of all the
        /// segments.
        /// </summary>
        public void ClearHistory()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_clear_history(this.ptr);
        }
        /// <summary>
        /// Clears out the Attempt History, the Segment Histories, all the times,
        /// sets the Attempt Count to 0 and clears the speedrun.com run id
        /// association. All Custom Comparisons other than `Personal Best` are
        /// deleted as well.
        /// </summary>
        public void ClearTimes()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_clear_times(this.ptr);
        }
        /// <summary>
        /// Creates a Sum of Best Cleaner which allows you to interactively remove
        /// potential issues in the segment history that lead to an inaccurate Sum
        /// of Best. If you skip a split, whenever you will do the next split, the
        /// combined segment time might be faster than the sum of the individual
        /// best segments. The Sum of Best Cleaner will point out all of these and
        /// allows you to delete them individually if any of them seem wrong.
        /// </summary>
        public SumOfBestCleaner CleanSumOfBest()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SumOfBestCleaner(LiveSplitCoreNative.RunEditor_clean_sum_of_best(this.ptr));
            return result;
        }
        internal RunEditorRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Run Editor allows modifying Runs while ensuring that all the different
    /// invariants of the Run objects are upheld no matter what kind of operations
    /// are being applied to the Run. It provides the current state of the editor as
    /// state objects that can be visualized by any kind of User Interface.
    /// </summary>
    public class RunEditor : RunEditorRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                ptr = IntPtr.Zero;
            }
        }
        ~RunEditor()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Run Editor that modifies the Run provided. Creation of the Run
        /// Editor fails when a Run with no segments is provided. If a Run object with
        /// no segments is provided, the Run Editor creation fails and null is
        /// returned.
        /// </summary>
        public static RunEditor New(Run run)
        {
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = new RunEditor(LiveSplitCoreNative.RunEditor_new(run.ptr));
            run.ptr = IntPtr.Zero;
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Closes the Run Editor and gives back access to the modified Run object. In
        /// case you want to implement a Cancel Button, just dispose the Run object you
        /// get here.
        /// </summary>
        public Run Close()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Run(LiveSplitCoreNative.RunEditor_close(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal RunEditor(IntPtr ptr) : base(ptr) { }
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
        public long IconLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Segment_icon_len(this.ptr);
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
    /// The Segment Time Component is a component that shows the time for the current
    /// segment in a comparison of your choosing. If no comparison is specified it
    /// uses the timer's current comparison.
    /// </summary>
    public class SegmentTimeComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.SegmentTimeComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.SegmentTimeComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal SegmentTimeComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Segment Time Component is a component that shows the time for the current
    /// segment in a comparison of your choosing. If no comparison is specified it
    /// uses the timer's current comparison.
    /// </summary>
    public class SegmentTimeComponentRefMut : SegmentTimeComponentRef
    {
        internal SegmentTimeComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Segment Time Component is a component that shows the time for the current
    /// segment in a comparison of your choosing. If no comparison is specified it
    /// uses the timer's current comparison.
    /// </summary>
    public class SegmentTimeComponent : SegmentTimeComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SegmentTimeComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SegmentTimeComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Segment Time Component.
        /// </summary>
        public SegmentTimeComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SegmentTimeComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.SegmentTimeComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal SegmentTimeComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Separator Component is a simple component that only serves to render
    /// separators between components.
    /// </summary>
    public class SeparatorComponentRef
    {
        internal IntPtr ptr;
        internal SeparatorComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Separator Component is a simple component that only serves to render
    /// separators between components.
    /// </summary>
    public class SeparatorComponentRefMut : SeparatorComponentRef
    {
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public SeparatorComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new SeparatorComponentState(LiveSplitCoreNative.SeparatorComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal SeparatorComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Separator Component is a simple component that only serves to render
    /// separators between components.
    /// </summary>
    public class SeparatorComponent : SeparatorComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SeparatorComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SeparatorComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Separator Component.
        /// </summary>
        public SeparatorComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SeparatorComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.SeparatorComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal SeparatorComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class SeparatorComponentStateRef
    {
        internal IntPtr ptr;
        internal SeparatorComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class SeparatorComponentStateRefMut : SeparatorComponentStateRef
    {
        internal SeparatorComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class SeparatorComponentState : SeparatorComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SeparatorComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SeparatorComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SeparatorComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Describes a setting's value. Such a value can be of a variety of different
    /// types.
    /// </summary>
    public class SettingValueRef
    {
        internal IntPtr ptr;
        internal SettingValueRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// Describes a setting's value. Such a value can be of a variety of different
    /// types.
    /// </summary>
    public class SettingValueRefMut : SettingValueRef
    {
        internal SettingValueRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// Describes a setting's value. Such a value can be of a variety of different
    /// types.
    /// </summary>
    public class SettingValue : SettingValueRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SettingValue_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SettingValue()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new setting value from a boolean value.
        /// </summary>
        public static SettingValue FromBool(bool value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_bool(value));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from an unsigned integer.
        /// </summary>
        public static SettingValue FromUint(uint value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_uint(value));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from a signed integer.
        /// </summary>
        public static SettingValue FromInt(int value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_int(value));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from a string.
        /// </summary>
        public static SettingValue FromString(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_string(value));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from a string that has the type `optional string`.
        /// </summary>
        public static SettingValue FromOptionalString(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_string(value));
            return result;
        }
        /// <summary>
        /// Creates a new empty setting value that has the type `optional string`.
        /// </summary>
        public static SettingValue FromOptionalEmptyString()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_empty_string());
            return result;
        }
        /// <summary>
        /// Creates a new setting value from a floating point number.
        /// </summary>
        public static SettingValue FromFloat(double value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_float(value));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from an accuracy name. If it doesn't match a
        /// known accuracy, null is returned.
        /// </summary>
        public static SettingValue FromAccuracy(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_accuracy(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new setting value from a digits format name. If it doesn't match a
        /// known digits format, null is returned.
        /// </summary>
        public static SettingValue FromDigitsFormat(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_digits_format(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new setting value from a timing method name with the type
        /// `optional timing method`. If it doesn't match a known timing method, null
        /// is returned.
        /// </summary>
        public static SettingValue FromOptionalTimingMethod(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_timing_method(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new empty setting value with the type `optional timing method`.
        /// </summary>
        public static SettingValue FromOptionalEmptyTimingMethod()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_empty_timing_method());
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the color provided as RGBA.
        /// </summary>
        public static SettingValue FromColor(float r, float g, float b, float a)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_color(r, g, b, a));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the color provided as RGBA with the type
        /// `optional color`.
        /// </summary>
        public static SettingValue FromOptionalColor(float r, float g, float b, float a)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_color(r, g, b, a));
            return result;
        }
        /// <summary>
        /// Creates a new empty setting value with the type `optional color`.
        /// </summary>
        public static SettingValue FromOptionalEmptyColor()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_empty_color());
            return result;
        }
        /// <summary>
        /// Creates a new setting value that is a transparent gradient.
        /// </summary>
        public static SettingValue FromTransparentGradient()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_transparent_gradient());
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the vertical gradient provided as two RGBA colors.
        /// </summary>
        public static SettingValue FromVerticalGradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_vertical_gradient(r1, g1, b1, a1, r2, g2, b2, a2));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the horizontal gradient provided as two RGBA colors.
        /// </summary>
        public static SettingValue FromHorizontalGradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_horizontal_gradient(r1, g1, b1, a1, r2, g2, b2, a2));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the alternating gradient provided as two RGBA colors.
        /// </summary>
        public static SettingValue FromAlternatingGradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_alternating_gradient(r1, g1, b1, a1, r2, g2, b2, a2));
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the alignment name provided. If it doesn't
        /// match a known alignment, null is returned.
        /// </summary>
        public static SettingValue FromAlignment(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_alignment(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the column start with name provided. If it
        /// doesn't match a known column start with, null is returned.
        /// </summary>
        public static SettingValue FromColumnStartWith(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_column_start_with(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the column update with name provided. If it
        /// doesn't match a known column update with, null is returned.
        /// </summary>
        public static SettingValue FromColumnUpdateWith(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_column_update_with(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the column update trigger. If it doesn't
        /// match a known column update trigger, null is returned.
        /// </summary>
        public static SettingValue FromColumnUpdateTrigger(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_column_update_trigger(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Creates a new setting value from the layout direction. If it doesn't
        /// match a known layout direction, null is returned.
        /// </summary>
        public static SettingValue FromLayoutDirection(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_layout_direction(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal SettingValue(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Shared Timer that can be used to share a single timer object with multiple
    /// owners.
    /// </summary>
    public class SharedTimerRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Creates a new shared timer handle that shares the same timer. The inner
        /// timer object only gets disposed when the final handle gets disposed.
        /// </summary>
        public SharedTimer Share()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SharedTimer(LiveSplitCoreNative.SharedTimer_share(this.ptr));
            return result;
        }
        /// <summary>
        /// Requests read access to the timer that is being shared. This blocks the
        /// thread as long as there is an active write lock. Dispose the read lock when
        /// you are done using the timer.
        /// </summary>
        public TimerReadLock Read()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerReadLock(LiveSplitCoreNative.SharedTimer_read(this.ptr));
            return result;
        }
        /// <summary>
        /// Requests write access to the timer that is being shared. This blocks the
        /// thread as long as there are active write or read locks. Dispose the write
        /// lock when you are done using the timer.
        /// </summary>
        public TimerWriteLock Write()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerWriteLock(LiveSplitCoreNative.SharedTimer_write(this.ptr));
            return result;
        }
        /// <summary>
        /// Replaces the timer that is being shared by the timer provided. This blocks
        /// the thread as long as there are active write or read locks. Everyone who is
        /// sharing the old timer will share the provided timer after successful
        /// completion.
        /// </summary>
        public void ReplaceInner(Timer timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            LiveSplitCoreNative.SharedTimer_replace_inner(this.ptr, timer.ptr);
            timer.ptr = IntPtr.Zero;
        }
        public void ReadWith(Action<TimerRef> action)
        {
            using (var timerLock = Read())
            {
                action(timerLock.Timer());
            }
        }
        public void WriteWith(Action<TimerRefMut> action)
        {
            using (var timerLock = Write())
            {
                action(timerLock.Timer());
            }
        }
        internal SharedTimerRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Shared Timer that can be used to share a single timer object with multiple
    /// owners.
    /// </summary>
    public class SharedTimerRefMut : SharedTimerRef
    {
        internal SharedTimerRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Shared Timer that can be used to share a single timer object with multiple
    /// owners.
    /// </summary>
    public class SharedTimer : SharedTimerRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SharedTimer_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SharedTimer()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SharedTimer(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Splits Component is the main component for visualizing all the split
    /// times. Each segment is shown in a tabular fashion showing the segment icon,
    /// segment name, the delta compared to the chosen comparison, and the split
    /// time. The list provides scrolling functionality, so not every segment needs
    /// to be shown all the time.
    /// </summary>
    public class SplitsComponentRef
    {
        internal IntPtr ptr;
        internal SplitsComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Splits Component is the main component for visualizing all the split
    /// times. Each segment is shown in a tabular fashion showing the segment icon,
    /// segment name, the delta compared to the chosen comparison, and the split
    /// time. The list provides scrolling functionality, so not every segment needs
    /// to be shown all the time.
    /// </summary>
    public class SplitsComponentRefMut : SplitsComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = LiveSplitCoreNative.SplitsComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer and layout settings
        /// provided.
        /// </summary>
        public SplitsComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = new SplitsComponentState(LiveSplitCoreNative.SplitsComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        /// <summary>
        /// Scrolls up the window of the segments that are shown. Doesn't move the
        /// scroll window if it reaches the top of the segments.
        /// </summary>
        public void ScrollUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_scroll_up(this.ptr);
        }
        /// <summary>
        /// Scrolls down the window of the segments that are shown. Doesn't move the
        /// scroll window if it reaches the bottom of the segments.
        /// </summary>
        public void ScrollDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_scroll_down(this.ptr);
        }
        /// <summary>
        /// The amount of segments to show in the list at any given time. If this is
        /// set to 0, all the segments are shown. If this is set to a number lower
        /// than the total amount of segments, only a certain window of all the
        /// segments is shown. This window can scroll up or down.
        /// </summary>
        public void SetVisualSplitCount(long count)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_visual_split_count(this.ptr, (UIntPtr)count);
        }
        /// <summary>
        /// If there's more segments than segments that are shown, the window
        /// showing the segments automatically scrolls up and down when the current
        /// segment changes. This count determines the minimum number of future
        /// segments to be shown in this scrolling window when it automatically
        /// scrolls.
        /// </summary>
        public void SetSplitPreviewCount(long count)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_split_preview_count(this.ptr, (UIntPtr)count);
        }
        /// <summary>
        /// If not every segment is shown in the scrolling window of segments, then
        /// this determines whether the final segment is always to be shown, as it
        /// contains valuable information about the total duration of the chosen
        /// comparison, which is often the runner's Personal Best.
        /// </summary>
        public void SetAlwaysShowLastSplit(bool alwaysShowLastSplit)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_always_show_last_split(this.ptr, alwaysShowLastSplit);
        }
        /// <summary>
        /// If the last segment is to always be shown, this determines whether to
        /// show a more pronounced separator in front of the last segment, if it is
        /// not directly adjacent to the segment shown right before it in the
        /// scrolling window.
        /// </summary>
        public void SetSeparatorLastSplit(bool separatorLastSplit)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_separator_last_split(this.ptr, separatorLastSplit);
        }
        internal SplitsComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Splits Component is the main component for visualizing all the split
    /// times. Each segment is shown in a tabular fashion showing the segment icon,
    /// segment name, the delta compared to the chosen comparison, and the split
    /// time. The list provides scrolling functionality, so not every segment needs
    /// to be shown all the time.
    /// </summary>
    public class SplitsComponent : SplitsComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SplitsComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SplitsComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Splits Component.
        /// </summary>
        public SplitsComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SplitsComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.SplitsComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal SplitsComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object that describes a single segment's information to visualize.
    /// </summary>
    public class SplitsComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Describes whether a more pronounced separator should be shown in front of
        /// the last segment provided.
        /// </summary>
        public bool FinalSeparatorShown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_final_separator_shown(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns the amount of segments to visualize.
        /// </summary>
        public long Len()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_len(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the amount of icon changes that happened in this state object.
        /// </summary>
        public long IconChangeCount()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_icon_change_count(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the index of the segment of the icon change with the specified
        /// index. This is based on the index in the run, not on the index of the
        /// SplitState in the State object. The corresponding index is the index field
        /// of the SplitState object. You may not provide an out of bounds index.
        /// </summary>
        public long IconChangeSegmentIndex(long iconChangeIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_icon_change_segment_index(this.ptr, (UIntPtr)iconChangeIndex);
            return result;
        }
        /// <summary>
        /// The icon data of the segment of the icon change with the specified index.
        /// The buffer may be empty. This indicates that there is no icon. You may not
        /// provide an out of bounds index.
        /// </summary>
        public IntPtr IconChangeIconPtr(long iconChangeIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_icon_change_icon_ptr(this.ptr, (UIntPtr)iconChangeIndex);
            return result;
        }
        /// <summary>
        /// The length of the icon data of the segment of the icon change with the
        /// specified index.
        /// </summary>
        public long IconChangeIconLen(long iconChangeIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_icon_change_icon_len(this.ptr, (UIntPtr)iconChangeIndex);
            return result;
        }
        /// <summary>
        /// The name of the segment with the specified index. You may not provide an out
        /// of bounds index.
        /// </summary>
        public string Name(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_name(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// The amount of columns to visualize for the segment with the specified index.
        /// The columns are specified from right to left. You may not provide an out of
        /// bounds index. The amount of columns to visualize may differ from segment to
        /// segment.
        /// </summary>
        public long ColumnsLen(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_columns_len(this.ptr, (UIntPtr)index);
            return result;
        }
        /// <summary>
        /// The column's value to show for the split and column with the specified
        /// index. The columns are specified from right to left. You may not provide an
        /// out of bounds index.
        /// </summary>
        public string ColumnValue(long index, long columnIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_column_value(this.ptr, (UIntPtr)index, (UIntPtr)columnIndex);
            return result;
        }
        /// <summary>
        /// The semantic coloring information the column's value carries of the segment
        /// and column with the specified index. The columns are specified from right to
        /// left. You may not provide an out of bounds index.
        /// </summary>
        public string ColumnSemanticColor(long index, long columnIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_column_semantic_color(this.ptr, (UIntPtr)index, (UIntPtr)columnIndex);
            return result;
        }
        /// <summary>
        /// Describes if the segment with the specified index is the segment the active
        /// attempt is currently on.
        /// </summary>
        public bool IsCurrentSplit(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_is_current_split(this.ptr, (UIntPtr)index) != 0;
            return result;
        }
        internal SplitsComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object that describes a single segment's information to visualize.
    /// </summary>
    public class SplitsComponentStateRefMut : SplitsComponentStateRef
    {
        internal SplitsComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object that describes a single segment's information to visualize.
    /// </summary>
    public class SplitsComponentState : SplitsComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SplitsComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SplitsComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SplitsComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Sum of Best Cleaner allows you to interactively remove potential issues in
    /// the Segment History that lead to an inaccurate Sum of Best. If you skip a
    /// split, whenever you get to the next split, the combined segment time might
    /// be faster than the sum of the individual best segments. The Sum of Best
    /// Cleaner will point out all of occurrences of this and allows you to delete
    /// them individually if any of them seem wrong.
    /// </summary>
    public class SumOfBestCleanerRef
    {
        internal IntPtr ptr;
        internal SumOfBestCleanerRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Sum of Best Cleaner allows you to interactively remove potential issues in
    /// the Segment History that lead to an inaccurate Sum of Best. If you skip a
    /// split, whenever you get to the next split, the combined segment time might
    /// be faster than the sum of the individual best segments. The Sum of Best
    /// Cleaner will point out all of occurrences of this and allows you to delete
    /// them individually if any of them seem wrong.
    /// </summary>
    public class SumOfBestCleanerRefMut : SumOfBestCleanerRef
    {
        /// <summary>
        /// Returns the next potential clean up. If there are no more potential
        /// clean ups, null is returned.
        /// </summary>
        public PotentialCleanUp NextPotentialCleanUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new PotentialCleanUp(LiveSplitCoreNative.SumOfBestCleaner_next_potential_clean_up(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Applies a clean up to the Run.
        /// </summary>
        public void Apply(PotentialCleanUp cleanUp)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (cleanUp.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("cleanUp");
            }
            LiveSplitCoreNative.SumOfBestCleaner_apply(this.ptr, cleanUp.ptr);
            cleanUp.ptr = IntPtr.Zero;
        }
        internal SumOfBestCleanerRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Sum of Best Cleaner allows you to interactively remove potential issues in
    /// the Segment History that lead to an inaccurate Sum of Best. If you skip a
    /// split, whenever you get to the next split, the combined segment time might
    /// be faster than the sum of the individual best segments. The Sum of Best
    /// Cleaner will point out all of occurrences of this and allows you to delete
    /// them individually if any of them seem wrong.
    /// </summary>
    public class SumOfBestCleaner : SumOfBestCleanerRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SumOfBestCleaner_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SumOfBestCleaner()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SumOfBestCleaner(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Sum of Best Segments Component shows the fastest possible time to
    /// complete a run of this category, based on information collected from all the
    /// previous attempts. This often matches up with the sum of the best segment
    /// times of all the segments, but that may not always be the case, as skipped
    /// segments may introduce combined segments that may be faster than the actual
    /// sum of their best segment times. The name is therefore a bit misleading, but
    /// sticks around for historical reasons.
    /// </summary>
    public class SumOfBestComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.SumOfBestComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.SumOfBestComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal SumOfBestComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Sum of Best Segments Component shows the fastest possible time to
    /// complete a run of this category, based on information collected from all the
    /// previous attempts. This often matches up with the sum of the best segment
    /// times of all the segments, but that may not always be the case, as skipped
    /// segments may introduce combined segments that may be faster than the actual
    /// sum of their best segment times. The name is therefore a bit misleading, but
    /// sticks around for historical reasons.
    /// </summary>
    public class SumOfBestComponentRefMut : SumOfBestComponentRef
    {
        internal SumOfBestComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Sum of Best Segments Component shows the fastest possible time to
    /// complete a run of this category, based on information collected from all the
    /// previous attempts. This often matches up with the sum of the best segment
    /// times of all the segments, but that may not always be the case, as skipped
    /// segments may introduce combined segments that may be faster than the actual
    /// sum of their best segment times. The name is therefore a bit misleading, but
    /// sticks around for historical reasons.
    /// </summary>
    public class SumOfBestComponent : SumOfBestComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SumOfBestComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SumOfBestComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Sum of Best Segments Component.
        /// </summary>
        public SumOfBestComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SumOfBestComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.SumOfBestComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal SumOfBestComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Text Component simply visualizes any given text. This can either be a
    /// single centered text, or split up into a left and right text, which is
    /// suitable for a situation where you have a label and a value.
    /// </summary>
    public class TextComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.TextComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state.
        /// </summary>
        public TextComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new TextComponentState(LiveSplitCoreNative.TextComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal TextComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Text Component simply visualizes any given text. This can either be a
    /// single centered text, or split up into a left and right text, which is
    /// suitable for a situation where you have a label and a value.
    /// </summary>
    public class TextComponentRefMut : TextComponentRef
    {
        /// <summary>
        /// Sets the centered text. If the current mode is split, it is switched to
        /// centered mode.
        /// </summary>
        public void SetCenter(string text)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.TextComponent_set_center(this.ptr, text);
        }
        /// <summary>
        /// Sets the left text. If the current mode is centered, it is switched to
        /// split mode, with the right text being empty.
        /// </summary>
        public void SetLeft(string text)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.TextComponent_set_left(this.ptr, text);
        }
        /// <summary>
        /// Sets the right text. If the current mode is centered, it is switched to
        /// split mode, with the left text being empty.
        /// </summary>
        public void SetRight(string text)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.TextComponent_set_right(this.ptr, text);
        }
        internal TextComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Text Component simply visualizes any given text. This can either be a
    /// single centered text, or split up into a left and right text, which is
    /// suitable for a situation where you have a label and a value.
    /// </summary>
    public class TextComponent : TextComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TextComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TextComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Text Component.
        /// </summary>
        public TextComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TextComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.TextComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal TextComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TextComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the left part of the text. If the text isn't split up, an empty
        /// string is returned instead.
        /// </summary>
        public string Left()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_left(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the right part of the text. If the text isn't split up, an empty
        /// string is returned instead.
        /// </summary>
        public string Right()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_right(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the centered text. If the text isn't centered, an empty string is
        /// returned instead.
        /// </summary>
        public string Center()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_center(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns whether the text is split up into a left and right part.
        /// </summary>
        public bool IsSplit()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_is_split(this.ptr) != 0;
            return result;
        }
        internal TextComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TextComponentStateRefMut : TextComponentStateRef
    {
        internal TextComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TextComponentState : TextComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TextComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TextComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal TextComponentState(IntPtr ptr) : base(ptr) { }
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
        public static TimeSpan Parse(string text)
        {
            var result = new TimeSpan(LiveSplitCoreNative.TimeSpan_parse(text));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal TimeSpan(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Timer provides all the capabilities necessary for doing speedrun attempts.
    /// </summary>
    public class TimerRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Returns the currently selected Timing Method.
        /// </summary>
        public byte CurrentTimingMethod()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_current_timing_method(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns the current comparison that is being compared against. This may
        /// be a custom comparison or one of the Comparison Generators.
        /// </summary>
        public string CurrentComparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_current_comparison(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns whether Game Time is currently initialized. Game Time
        /// automatically gets uninitialized for each new attempt.
        /// </summary>
        public bool IsGameTimeInitialized()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_is_game_time_initialized(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns whether the Game Timer is currently paused. If the Game Timer is
        /// not paused, it automatically increments similar to Real Time.
        /// </summary>
        public bool IsGameTimePaused()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_is_game_time_paused(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Accesses the loading times. Loading times are defined as Game Time - Real Time.
        /// </summary>
        public TimeSpanRef LoadingTimes()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Timer_loading_times(this.ptr));
            return result;
        }
        /// <summary>
        /// Returns the current Timer Phase.
        /// </summary>
        public byte CurrentPhase()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_current_phase(this.ptr);
            return result;
        }
        /// <summary>
        /// Accesses the Run in use by the Timer.
        /// </summary>
        public RunRef GetRun()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunRef(LiveSplitCoreNative.Timer_get_run(this.ptr));
            return result;
        }
        /// <summary>
        /// Saves the Run in use by the Timer as a LiveSplit splits file (*.lss).
        /// </summary>
        public string SaveAsLss()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_save_as_lss(this.ptr);
            return result;
        }
        /// <summary>
        /// Prints out debug information representing the whole state of the Timer. This
        /// is being written to stdout.
        /// </summary>
        public void PrintDebug()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_print_debug(this.ptr);
        }
        /// <summary>
        /// Returns the current time of the Timer. The Game Time is null if the Game
        /// Time has not been initialized.
        /// </summary>
        public TimeRef CurrentTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Timer_current_time(this.ptr));
            return result;
        }
        internal TimerRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Timer provides all the capabilities necessary for doing speedrun attempts.
    /// </summary>
    public class TimerRefMut : TimerRef
    {
        /// <summary>
        /// Replaces the Run object used by the Timer with the Run object provided. If
        /// the Run provided contains no segments, it can't be used for timing and is
        /// not being modified. Otherwise the Run that was in use by the Timer gets
        /// stored in the Run object provided. Before the Run is returned, the current
        /// attempt is reset and the splits are being updated depending on the
        /// `update_splits` parameter. The return value indicates whether the Run got
        /// replaced successfully.
        /// </summary>
        public bool ReplaceRun(RunRefMut run, bool updateSplits)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = LiveSplitCoreNative.Timer_replace_run(this.ptr, run.ptr, updateSplits) != 0;
            return result;
        }
        /// <summary>
        /// Sets the Run object used by the Timer with the Run object provided. If the
        /// Run provided contains no segments, it can't be used for timing and gets
        /// returned again. If the Run object can be set, the original Run object in use
        /// by the Timer is disposed by this method and null is returned.
        /// </summary>
        public Run SetRun(Run run)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = new Run(LiveSplitCoreNative.Timer_set_run(this.ptr, run.ptr));
            run.ptr = IntPtr.Zero;
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Starts the Timer if there is no attempt in progress. If that's not the
        /// case, nothing happens.
        /// </summary>
        public void Start()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_start(this.ptr);
        }
        /// <summary>
        /// If an attempt is in progress, stores the current time as the time of the
        /// current split. The attempt ends if the last split time is stored.
        /// </summary>
        public void Split()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_split(this.ptr);
        }
        /// <summary>
        /// Starts a new attempt or stores the current time as the time of the
        /// current split. The attempt ends if the last split time is stored.
        /// </summary>
        public void SplitOrStart()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_split_or_start(this.ptr);
        }
        /// <summary>
        /// Skips the current split if an attempt is in progress and the
        /// current split is not the last split.
        /// </summary>
        public void SkipSplit()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_skip_split(this.ptr);
        }
        /// <summary>
        /// Removes the split time from the last split if an attempt is in progress
        /// and there is a previous split. The Timer Phase also switches to
        /// `Running` if it previously was `Ended`.
        /// </summary>
        public void UndoSplit()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_undo_split(this.ptr);
        }
        /// <summary>
        /// Resets the current attempt if there is one in progress. If the splits
        /// are to be updated, all the information of the current attempt is stored
        /// in the Run's history. Otherwise the current attempt's information is
        /// discarded.
        /// </summary>
        public void Reset(bool updateSplits)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_reset(this.ptr, updateSplits);
        }
        /// <summary>
        /// Resets the current attempt if there is one in progress. The splits are
        /// updated such that the current attempt's split times are being stored as
        /// the new Personal Best.
        /// </summary>
        public void ResetAndSetAttemptAsPb()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_reset_and_set_attempt_as_pb(this.ptr);
        }
        /// <summary>
        /// Pauses an active attempt that is not paused.
        /// </summary>
        public void Pause()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_pause(this.ptr);
        }
        /// <summary>
        /// Resumes an attempt that is paused.
        /// </summary>
        public void Resume()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_resume(this.ptr);
        }
        /// <summary>
        /// Toggles an active attempt between `Paused` and `Running`.
        /// </summary>
        public void TogglePause()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_toggle_pause(this.ptr);
        }
        /// <summary>
        /// Toggles an active attempt between `Paused` and `Running` or starts an
        /// attempt if there's none in progress.
        /// </summary>
        public void TogglePauseOrStart()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_toggle_pause_or_start(this.ptr);
        }
        /// <summary>
        /// Removes all the pause times from the current time. If the current
        /// attempt is paused, it also resumes that attempt. Additionally, if the
        /// attempt is finished, the final split time is adjusted to not include the
        /// pause times as well.
        /// 
        /// # Warning
        /// 
        /// This behavior is not entirely optimal, as generally only the final split
        /// time is modified, while all other split times are left unmodified, which
        /// may not be what actually happened during the run.
        /// </summary>
        public void UndoAllPauses()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_undo_all_pauses(this.ptr);
        }
        /// <summary>
        /// Sets the current Timing Method to the Timing Method provided.
        /// </summary>
        public void SetCurrentTimingMethod(byte method)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_set_current_timing_method(this.ptr, method);
        }
        /// <summary>
        /// Switches the current comparison to the next comparison in the list.
        /// </summary>
        public void SwitchToNextComparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_switch_to_next_comparison(this.ptr);
        }
        /// <summary>
        /// Switches the current comparison to the previous comparison in the list.
        /// </summary>
        public void SwitchToPreviousComparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_switch_to_previous_comparison(this.ptr);
        }
        /// <summary>
        /// Initializes Game Time for the current attempt. Game Time automatically
        /// gets uninitialized for each new attempt.
        /// </summary>
        public void InitializeGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_initialize_game_time(this.ptr);
        }
        /// <summary>
        /// Deinitializes Game Time for the current attempt.
        /// </summary>
        public void DeinitializeGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_deinitialize_game_time(this.ptr);
        }
        /// <summary>
        /// Pauses the Game Timer such that it doesn't automatically increment
        /// similar to Real Time.
        /// </summary>
        public void PauseGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_pause_game_time(this.ptr);
        }
        /// <summary>
        /// Resumes the Game Timer such that it automatically increments similar to
        /// Real Time, starting from the Game Time it was paused at.
        /// </summary>
        public void ResumeGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_resume_game_time(this.ptr);
        }
        /// <summary>
        /// Sets the Game Time to the time specified. This also works if the Game
        /// Time is paused, which can be used as a way of updating the Game Timer
        /// periodically without it automatically moving forward. This ensures that
        /// the Game Timer never shows any time that is not coming from the game.
        /// </summary>
        public void SetGameTime(TimeSpanRef time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (time.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("time");
            }
            LiveSplitCoreNative.Timer_set_game_time(this.ptr, time.ptr);
        }
        /// <summary>
        /// Instead of setting the Game Time directly, this method can be used to
        /// just specify the amount of time the game has been loading. The Game Time
        /// is then automatically determined by Real Time - Loading Times.
        /// </summary>
        public void SetLoadingTimes(TimeSpanRef time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (time.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("time");
            }
            LiveSplitCoreNative.Timer_set_loading_times(this.ptr, time.ptr);
        }
        /// <summary>
        /// Marks the Run as unmodified, so that it is known that all the changes
        /// have been saved.
        /// </summary>
        public void MarkAsUnmodified()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_mark_as_unmodified(this.ptr);
        }
        internal TimerRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Timer provides all the capabilities necessary for doing speedrun attempts.
    /// </summary>
    public class Timer : TimerRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.Timer_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~Timer()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Timer based on a Run object storing all the information
        /// about the splits. The Run object needs to have at least one segment, so
        /// that the Timer can store the final time. If a Run object with no
        /// segments is provided, the Timer creation fails and null is returned.
        /// </summary>
        public static Timer New(Run run)
        {
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            var result = new Timer(LiveSplitCoreNative.Timer_new(run.ptr));
            run.ptr = IntPtr.Zero;
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        /// <summary>
        /// Consumes the Timer and creates a Shared Timer that can be shared across
        /// multiple threads with multiple owners.
        /// </summary>
        public SharedTimer IntoShared()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SharedTimer(LiveSplitCoreNative.Timer_into_shared(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        /// <summary>
        /// Takes out the Run from the Timer and resets the current attempt if there
        /// is one in progress. If the splits are to be updated, all the information
        /// of the current attempt is stored in the Run's history. Otherwise the
        /// current attempt's information is discarded.
        /// </summary>
        public Run IntoRun(bool updateSplits)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Run(LiveSplitCoreNative.Timer_into_run(this.ptr, updateSplits));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal Timer(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Timer Component is a component that shows the total time of the current
    /// attempt as a digital clock. The color of the time shown is based on a how
    /// well the current attempt is doing compared to the chosen comparison.
    /// </summary>
    public class TimerComponentRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = LiveSplitCoreNative.TimerComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer and the layout
        /// settings provided.
        /// </summary>
        public TimerComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            if (layoutSettings.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layoutSettings");
            }
            var result = new TimerComponentState(LiveSplitCoreNative.TimerComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal TimerComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Timer Component is a component that shows the total time of the current
    /// attempt as a digital clock. The color of the time shown is based on a how
    /// well the current attempt is doing compared to the chosen comparison.
    /// </summary>
    public class TimerComponentRefMut : TimerComponentRef
    {
        internal TimerComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Timer Component is a component that shows the total time of the current
    /// attempt as a digital clock. The color of the time shown is based on a how
    /// well the current attempt is doing compared to the chosen comparison.
    /// </summary>
    public class TimerComponent : TimerComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TimerComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TimerComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Timer Component.
        /// </summary>
        public TimerComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TimerComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.TimerComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal TimerComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TimerComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// The time shown by the component without the fractional part.
        /// </summary>
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimerComponentState_time(this.ptr);
            return result;
        }
        /// <summary>
        /// The fractional part of the time shown (including the dot).
        /// </summary>
        public string Fraction()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimerComponentState_fraction(this.ptr);
            return result;
        }
        /// <summary>
        /// The semantic coloring information the time carries.
        /// </summary>
        public string SemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimerComponentState_semantic_color(this.ptr);
            return result;
        }
        internal TimerComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TimerComponentStateRefMut : TimerComponentStateRef
    {
        internal TimerComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TimerComponentState : TimerComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TimerComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TimerComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal TimerComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Timer Read Lock allows temporary read access to a timer. Dispose this to
    /// release the read lock.
    /// </summary>
    public class TimerReadLockRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// Accesses the timer.
        /// </summary>
        public TimerRef Timer()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerRef(LiveSplitCoreNative.TimerReadLock_timer(this.ptr));
            return result;
        }
        internal TimerReadLockRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Timer Read Lock allows temporary read access to a timer. Dispose this to
    /// release the read lock.
    /// </summary>
    public class TimerReadLockRefMut : TimerReadLockRef
    {
        internal TimerReadLockRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Timer Read Lock allows temporary read access to a timer. Dispose this to
    /// release the read lock.
    /// </summary>
    public class TimerReadLock : TimerReadLockRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TimerReadLock_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TimerReadLock()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal TimerReadLock(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Timer Write Lock allows temporary write access to a timer. Dispose this to
    /// release the write lock.
    /// </summary>
    public class TimerWriteLockRef
    {
        internal IntPtr ptr;
        internal TimerWriteLockRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// A Timer Write Lock allows temporary write access to a timer. Dispose this to
    /// release the write lock.
    /// </summary>
    public class TimerWriteLockRefMut : TimerWriteLockRef
    {
        /// <summary>
        /// Accesses the timer.
        /// </summary>
        public TimerRefMut Timer()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerRefMut(LiveSplitCoreNative.TimerWriteLock_timer(this.ptr));
            return result;
        }
        internal TimerWriteLockRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// A Timer Write Lock allows temporary write access to a timer. Dispose this to
    /// release the write lock.
    /// </summary>
    public class TimerWriteLock : TimerWriteLockRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TimerWriteLock_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TimerWriteLock()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal TimerWriteLock(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Title Component is a component that shows the name of the game and the
    /// category that is being run. Additionally, the game icon, the attempt count,
    /// and the total number of successfully finished runs can be shown.
    /// </summary>
    public class TitleComponentRef
    {
        internal IntPtr ptr;
        internal TitleComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Title Component is a component that shows the name of the game and the
    /// category that is being run. Additionally, the game icon, the attempt count,
    /// and the total number of successfully finished runs can be shown.
    /// </summary>
    public class TitleComponentRefMut : TitleComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.TitleComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public TitleComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new TitleComponentState(LiveSplitCoreNative.TitleComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal TitleComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Title Component is a component that shows the name of the game and the
    /// category that is being run. Additionally, the game icon, the attempt count,
    /// and the total number of successfully finished runs can be shown.
    /// </summary>
    public class TitleComponent : TitleComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TitleComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TitleComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Title Component.
        /// </summary>
        public TitleComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TitleComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.TitleComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal TitleComponent(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TitleComponentStateRef
    {
        internal IntPtr ptr;
        /// <summary>
        /// The data of the game's icon. This value is only specified whenever the icon
        /// changes. If you explicitly want to query this value, remount the component.
        /// The buffer may be empty. This indicates that there is no icon. If no change
        /// occurred, null is returned instead.
        /// </summary>
        public IntPtr IconChangePtr()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_icon_change_ptr(this.ptr);
            return result;
        }
        /// <summary>
        /// The length of the game's icon data.
        /// </summary>
        public long IconChangeLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.TitleComponentState_icon_change_len(this.ptr);
            return result;
        }
        /// <summary>
        /// The first title line to show. This is either the game's name, or a
        /// combination of the game's name and the category.
        /// </summary>
        public string Line1()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_line1(this.ptr);
            return result;
        }
        /// <summary>
        /// By default the category name is shown on the second line. Based on the
        /// settings, it can however instead be shown in a single line together with
        /// the game name. In that case null is returned instead.
        /// </summary>
        public string Line2()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_line2(this.ptr);
            return result;
        }
        /// <summary>
        /// Specifies whether the title should centered or aligned to the left
        /// instead.
        /// </summary>
        public bool IsCentered()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_is_centered(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns whether the amount of successfully finished attempts is supposed to
        /// be shown.
        /// </summary>
        public bool ShowsFinishedRuns()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_shows_finished_runs(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns the amount of successfully finished attempts.
        /// </summary>
        public uint FinishedRuns()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_finished_runs(this.ptr);
            return result;
        }
        /// <summary>
        /// Returns whether the amount of total attempts is supposed to be shown.
        /// </summary>
        public bool ShowsAttempts()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_shows_attempts(this.ptr) != 0;
            return result;
        }
        /// <summary>
        /// Returns the amount of total attempts.
        /// </summary>
        public uint Attempts()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_attempts(this.ptr);
            return result;
        }
        internal TitleComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TitleComponentStateRefMut : TitleComponentStateRef
    {
        internal TitleComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The state object describes the information to visualize for this component.
    /// </summary>
    public class TitleComponentState : TitleComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TitleComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TitleComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal TitleComponentState(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Total Playtime Component is a component that shows the total amount of
    /// time that the current category has been played for.
    /// </summary>
    public class TotalPlaytimeComponentRef
    {
        internal IntPtr ptr;
        internal TotalPlaytimeComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    /// <summary>
    /// The Total Playtime Component is a component that shows the total amount of
    /// time that the current category has been played for.
    /// </summary>
    public class TotalPlaytimeComponentRefMut : TotalPlaytimeComponentRef
    {
        /// <summary>
        /// Encodes the component's state information as JSON.
        /// </summary>
        public string StateAsJson(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = LiveSplitCoreNative.TotalPlaytimeComponent_state_as_json(this.ptr, timer.ptr);
            return result;
        }
        /// <summary>
        /// Calculates the component's state based on the timer provided.
        /// </summary>
        public KeyValueComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new KeyValueComponentState(LiveSplitCoreNative.TotalPlaytimeComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal TotalPlaytimeComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

    /// <summary>
    /// The Total Playtime Component is a component that shows the total amount of
    /// time that the current category has been played for.
    /// </summary>
    public class TotalPlaytimeComponent : TotalPlaytimeComponentRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TotalPlaytimeComponent_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TotalPlaytimeComponent()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Creates a new Total Playtime Component.
        /// </summary>
        public TotalPlaytimeComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TotalPlaytimeComponent_new();
        }
        /// <summary>
        /// Converts the component into a generic component suitable for using with a
        /// layout.
        /// </summary>
        public Component IntoGeneric()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Component(LiveSplitCoreNative.TotalPlaytimeComponent_into_generic(this.ptr));
            this.ptr = IntPtr.Zero;
            return result;
        }
        internal TotalPlaytimeComponent(IntPtr ptr) : base(ptr) { }
    }

    public static class LiveSplitCoreNative
    {
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Analysis_calculate_sum_of_best(IntPtr run, bool simple_calculation, bool use_current_run, byte method);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Analysis_calculate_total_playtime_for_run(IntPtr run);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Analysis_calculate_total_playtime_for_timer(IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AtomicDateTime_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte AtomicDateTime_is_synchronized(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString AtomicDateTime_to_rfc2822(IntPtr self);
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
        public static extern IntPtr BlankSpaceComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BlankSpaceComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BlankSpaceComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString BlankSpaceComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BlankSpaceComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BlankSpaceComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint BlankSpaceComponentState_size(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Component_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CurrentComparisonComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CurrentComparisonComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CurrentComparisonComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString CurrentComparisonComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CurrentComparisonComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CurrentPaceComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CurrentPaceComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CurrentPaceComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString CurrentPaceComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CurrentPaceComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DeltaComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeltaComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DeltaComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DeltaComponent_state_as_json(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DeltaComponent_state(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DetailedTimerComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetailedTimerComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DetailedTimerComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponent_state_as_json(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DetailedTimerComponent_state(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetailedTimerComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_timer_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_timer_fraction(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_timer_semantic_color(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_segment_timer_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_segment_timer_fraction(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte DetailedTimerComponentState_comparison1_visible(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_comparison1_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_comparison1_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte DetailedTimerComponentState_comparison2_visible(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_comparison2_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_comparison2_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DetailedTimerComponentState_icon_change_ptr(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr DetailedTimerComponentState_icon_change_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_segment_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FuzzyList_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FuzzyList_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString FuzzyList_search(IntPtr self, LSCoreString pattern, UIntPtr max);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FuzzyList_push(IntPtr self, LSCoreString text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GeneralLayoutSettings_default();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GeneralLayoutSettings_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GraphComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GraphComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GraphComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString GraphComponent_state_as_json(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GraphComponent_state(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GraphComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr GraphComponentState_points_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GraphComponentState_point_x(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GraphComponentState_point_y(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte GraphComponentState_point_is_best_segment(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr GraphComponentState_horizontal_grid_lines_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GraphComponentState_horizontal_grid_line(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr GraphComponentState_vertical_grid_lines_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GraphComponentState_vertical_grid_line(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GraphComponentState_middle(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte GraphComponentState_is_live_delta_active(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte GraphComponentState_is_flipped(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HotkeyConfig_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HotkeyConfig_parse_json(LSCoreString settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HotkeyConfig_parse_file_handle(long handle);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HotkeyConfig_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString HotkeyConfig_settings_description_as_json(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString HotkeyConfig_as_json(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte HotkeyConfig_set_value(IntPtr self, UIntPtr index, IntPtr value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HotkeySystem_new(IntPtr shared_timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HotkeySystem_with_config(IntPtr shared_timer, IntPtr config);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HotkeySystem_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HotkeySystem_deactivate(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HotkeySystem_activate(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HotkeySystem_config(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte HotkeySystem_set_config(IntPtr self, IntPtr config);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyValueComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString KeyValueComponentState_key(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString KeyValueComponentState_value(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString KeyValueComponentState_semantic_color(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_default_layout();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_parse_json(LSCoreString settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_parse_file_handle(long handle);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_parse_original_livesplit(IntPtr data, UIntPtr length);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Layout_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Layout_settings_as_json(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Layout_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Layout_push(IntPtr self, IntPtr component);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Layout_scroll_up(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Layout_scroll_down(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Layout_remount(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutEditor_new(IntPtr layout);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutEditor_close(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString LayoutEditor_state_as_json(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString LayoutEditor_layout_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_select(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_add_component(IntPtr self, IntPtr component);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_remove_component(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_move_component_up(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_move_component_down(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_move_component(IntPtr self, UIntPtr dst_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_duplicate_component(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_set_component_settings_value(IntPtr self, UIntPtr index, IntPtr value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutEditor_set_general_settings_value(IntPtr self, UIntPtr index, IntPtr value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LayoutState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr LayoutState_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString LayoutState_component_type(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_blank_space(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_detailed_timer(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_graph(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_key_value(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_separator(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_splits(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_text(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_timer(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LayoutState_component_as_title(IntPtr self, UIntPtr index);
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
        public static extern IntPtr PbChanceComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PbChanceComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PbChanceComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PbChanceComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PbChanceComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PossibleTimeSaveComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PossibleTimeSaveComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PossibleTimeSaveComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PossibleTimeSaveComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PossibleTimeSaveComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PotentialCleanUp_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PotentialCleanUp_message(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PreviousSegmentComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PreviousSegmentComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PreviousSegmentComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PreviousSegmentComponent_state_as_json(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PreviousSegmentComponent_state(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_parse(IntPtr data, UIntPtr length, LSCoreString path, bool load_files);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_parse_file_handle(long handle, LSCoreString path, bool load_files);
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
        public static extern UIntPtr Run_attempt_history_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_attempt_history_index(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_save_as_lss(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr Run_custom_comparisons_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_custom_comparison(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_auto_splitter_settings(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_push_segment(IntPtr self, IntPtr segment);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_set_game_name(IntPtr self, LSCoreString game);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_set_category_name(IntPtr self, LSCoreString category);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_mark_as_modified(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunEditor_new(IntPtr run);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunEditor_close(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunEditor_state_as_json(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_select_timing_method(IntPtr self, byte method);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_unselect(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_select_additionally(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_select_only(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_game_name(IntPtr self, LSCoreString game);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_category_name(IntPtr self, LSCoreString category);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_parse_and_set_offset(IntPtr self, LSCoreString offset);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_parse_and_set_attempt_count(IntPtr self, LSCoreString attempts);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_game_icon(IntPtr self, IntPtr data, UIntPtr length);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_game_icon(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_run_id(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_region_name(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_platform_name(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_emulator_usage(IntPtr self, bool uses_emulator);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_speedrun_com_variable(IntPtr self, LSCoreString name, LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_speedrun_com_variable(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_add_custom_variable(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_custom_variable(IntPtr self, LSCoreString name, LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_custom_variable(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_clear_metadata(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_insert_segment_above(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_insert_segment_below(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_segments(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_move_segments_up(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_move_segments_down(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_active_set_icon(IntPtr self, IntPtr data, UIntPtr length);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_active_remove_icon(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_active_set_name(IntPtr self, LSCoreString name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_active_parse_and_set_split_time(IntPtr self, LSCoreString time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_active_parse_and_set_segment_time(IntPtr self, LSCoreString time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_active_parse_and_set_best_segment_time(IntPtr self, LSCoreString time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_active_parse_and_set_comparison_time(IntPtr self, LSCoreString comparison, LSCoreString time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_add_comparison(IntPtr self, LSCoreString comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_import_comparison(IntPtr self, IntPtr run, LSCoreString comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_comparison(IntPtr self, LSCoreString comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_rename_comparison(IntPtr self, LSCoreString old_name, LSCoreString new_name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_move_comparison(IntPtr self, UIntPtr src_index, UIntPtr dst_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_parse_and_generate_goal_comparison(IntPtr self, LSCoreString time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_clear_history(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_clear_times(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunEditor_clean_sum_of_best(IntPtr self);
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
        public static extern IntPtr SegmentTimeComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SegmentTimeComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SegmentTimeComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SegmentTimeComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SegmentTimeComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SeparatorComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SeparatorComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SeparatorComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SeparatorComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SeparatorComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_bool(bool value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_uint(uint value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_int(int value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_string(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_string(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_empty_string();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_float(double value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_accuracy(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_digits_format(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_timing_method(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_empty_timing_method();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_color(float r, float g, float b, float a);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_color(float r, float g, float b, float a);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_empty_color();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_transparent_gradient();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_vertical_gradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_horizontal_gradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_alternating_gradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_alignment(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_column_start_with(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_column_update_with(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_column_update_trigger(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_layout_direction(LSCoreString value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SettingValue_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SharedTimer_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SharedTimer_share(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SharedTimer_read(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SharedTimer_write(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SharedTimer_replace_inner(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SplitsComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SplitsComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponent_state_as_json(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SplitsComponent_state(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_scroll_up(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_scroll_down(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_set_visual_split_count(IntPtr self, UIntPtr count);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_set_split_preview_count(IntPtr self, UIntPtr count);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_set_always_show_last_split(IntPtr self, bool always_show_last_split);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponent_set_separator_last_split(IntPtr self, bool separator_last_split);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplitsComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte SplitsComponentState_final_separator_shown(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SplitsComponentState_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SplitsComponentState_icon_change_count(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SplitsComponentState_icon_change_segment_index(IntPtr self, UIntPtr icon_change_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SplitsComponentState_icon_change_icon_ptr(IntPtr self, UIntPtr icon_change_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SplitsComponentState_icon_change_icon_len(IntPtr self, UIntPtr icon_change_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_name(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr SplitsComponentState_columns_len(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_column_value(IntPtr self, UIntPtr index, UIntPtr column_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_column_semantic_color(IntPtr self, UIntPtr index, UIntPtr column_index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte SplitsComponentState_is_current_split(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SumOfBestCleaner_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SumOfBestCleaner_next_potential_clean_up(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SumOfBestCleaner_apply(IntPtr self, IntPtr clean_up);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SumOfBestComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SumOfBestComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SumOfBestComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SumOfBestComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SumOfBestComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TextComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TextComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TextComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TextComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_set_center(IntPtr self, LSCoreString text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_set_left(IntPtr self, LSCoreString text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_set_right(IntPtr self, LSCoreString text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TextComponentState_left(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TextComponentState_right(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TextComponentState_center(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TextComponentState_is_split(IntPtr self);
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
        public static extern IntPtr TimeSpan_parse(LSCoreString text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimeSpan_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimeSpan_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern double TimeSpan_total_seconds(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_new(IntPtr run);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_into_shared(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_into_run(IntPtr self, bool update_splits);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Timer_current_timing_method(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Timer_current_comparison(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Timer_is_game_time_initialized(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Timer_is_game_time_paused(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_loading_times(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Timer_current_phase(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_get_run(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Timer_save_as_lss(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_print_debug(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_current_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Timer_replace_run(IntPtr self, IntPtr run, bool update_splits);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Timer_set_run(IntPtr self, IntPtr run);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_start(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_split(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_split_or_start(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_skip_split(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_undo_split(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_reset(IntPtr self, bool update_splits);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_reset_and_set_attempt_as_pb(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_pause(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_resume(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_toggle_pause(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_toggle_pause_or_start(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_undo_all_pauses(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_set_current_timing_method(IntPtr self, byte method);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_switch_to_next_comparison(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_switch_to_previous_comparison(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_initialize_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_deinitialize_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_pause_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_resume_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_set_game_time(IntPtr self, IntPtr time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_set_loading_times(IntPtr self, IntPtr time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_mark_as_unmodified(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimerComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimerComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimerComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TimerComponent_state_as_json(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimerComponent_state(IntPtr self, IntPtr timer, IntPtr layout_settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimerComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TimerComponentState_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TimerComponentState_fraction(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TimerComponentState_semantic_color(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimerReadLock_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimerReadLock_timer(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimerWriteLock_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TimerWriteLock_timer(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TitleComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TitleComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TitleComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TitleComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TitleComponent_state(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TitleComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TitleComponentState_icon_change_ptr(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr TitleComponentState_icon_change_len(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TitleComponentState_line1(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TitleComponentState_line2(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TitleComponentState_is_centered(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TitleComponentState_shows_finished_runs(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TitleComponentState_finished_runs(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TitleComponentState_shows_attempts(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TitleComponentState_attempts(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TotalPlaytimeComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TotalPlaytimeComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TotalPlaytimeComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TotalPlaytimeComponent_state_as_json(IntPtr self, IntPtr timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TotalPlaytimeComponent_state(IntPtr self, IntPtr timer);
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
