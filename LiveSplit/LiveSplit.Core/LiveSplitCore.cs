using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace LiveSplitCore
{
    public class AtomicDateTimeRef
    {
        internal IntPtr ptr;
        public bool IsSynchronized()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.AtomicDateTime_is_synchronized(this.ptr) != 0;
            return result;
        }
        public string ToRfc2822()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.AtomicDateTime_to_rfc2822(this.ptr).AsString();
            return result;
        }
        public string ToRfc3339()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.AtomicDateTime_to_rfc3339(this.ptr).AsString();
            return result;
        }
        internal AtomicDateTimeRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class AtomicDateTimeRefMut : AtomicDateTimeRef
    {
        internal AtomicDateTimeRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class AttemptRef
    {
        internal IntPtr ptr;
        public int Index()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Attempt_index(this.ptr);
            return result;
        }
        public TimeRef Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Attempt_time(this.ptr));
            return result;
        }
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

    public class AttemptRefMut : AttemptRef
    {
        internal AttemptRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class BlankSpaceComponentRef
    {
        internal IntPtr ptr;
        internal BlankSpaceComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class BlankSpaceComponentRefMut : BlankSpaceComponentRef
    {
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
            var result = LiveSplitCoreNative.BlankSpaceComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
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
        public BlankSpaceComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.BlankSpaceComponent_new();
        }
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

    public class BlankSpaceComponentStateRef
    {
        internal IntPtr ptr;
        public uint Height()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.BlankSpaceComponentState_height(this.ptr);
            return result;
        }
        internal BlankSpaceComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class BlankSpaceComponentStateRefMut : BlankSpaceComponentStateRef
    {
        internal BlankSpaceComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class ComponentRef
    {
        internal IntPtr ptr;
        internal ComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class ComponentRefMut : ComponentRef
    {
        internal ComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class CurrentComparisonComponentRef
    {
        internal IntPtr ptr;
        internal CurrentComparisonComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class CurrentComparisonComponentRefMut : CurrentComparisonComponentRef
    {
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
            var result = LiveSplitCoreNative.CurrentComparisonComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
        public CurrentComparisonComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new CurrentComparisonComponentState(LiveSplitCoreNative.CurrentComparisonComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal CurrentComparisonComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public CurrentComparisonComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.CurrentComparisonComponent_new();
        }
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

    public class CurrentComparisonComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.CurrentComparisonComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Comparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.CurrentComparisonComponentState_comparison(this.ptr).AsString();
            return result;
        }
        internal CurrentComparisonComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class CurrentComparisonComponentStateRefMut : CurrentComparisonComponentStateRef
    {
        internal CurrentComparisonComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class CurrentComparisonComponentState : CurrentComparisonComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.CurrentComparisonComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~CurrentComparisonComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal CurrentComparisonComponentState(IntPtr ptr) : base(ptr) { }
    }

    public class CurrentPaceComponentRef
    {
        internal IntPtr ptr;
        internal CurrentPaceComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class CurrentPaceComponentRefMut : CurrentPaceComponentRef
    {
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
            var result = LiveSplitCoreNative.CurrentPaceComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
        public CurrentPaceComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new CurrentPaceComponentState(LiveSplitCoreNative.CurrentPaceComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal CurrentPaceComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public CurrentPaceComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.CurrentPaceComponent_new();
        }
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

    public class CurrentPaceComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.CurrentPaceComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.CurrentPaceComponentState_time(this.ptr).AsString();
            return result;
        }
        internal CurrentPaceComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class CurrentPaceComponentStateRefMut : CurrentPaceComponentStateRef
    {
        internal CurrentPaceComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class CurrentPaceComponentState : CurrentPaceComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.CurrentPaceComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~CurrentPaceComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal CurrentPaceComponentState(IntPtr ptr) : base(ptr) { }
    }

    public class DeltaComponentRef
    {
        internal IntPtr ptr;
        internal DeltaComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class DeltaComponentRefMut : DeltaComponentRef
    {
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
            var result = LiveSplitCoreNative.DeltaComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr).AsString();
            return result;
        }
        public DeltaComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
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
            var result = new DeltaComponentState(LiveSplitCoreNative.DeltaComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal DeltaComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public DeltaComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.DeltaComponent_new();
        }
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

    public class DeltaComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DeltaComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DeltaComponentState_time(this.ptr).AsString();
            return result;
        }
        public string SemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DeltaComponentState_semantic_color(this.ptr).AsString();
            return result;
        }
        internal DeltaComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class DeltaComponentStateRefMut : DeltaComponentStateRef
    {
        internal DeltaComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class DeltaComponentState : DeltaComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.DeltaComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~DeltaComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal DeltaComponentState(IntPtr ptr) : base(ptr) { }
    }

    public class DetailedTimerComponentRef
    {
        internal IntPtr ptr;
        internal DetailedTimerComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class DetailedTimerComponentRefMut : DetailedTimerComponentRef
    {
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
            var result = LiveSplitCoreNative.DetailedTimerComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr).AsString();
            return result;
        }
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
        public DetailedTimerComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.DetailedTimerComponent_new();
        }
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

    public class DetailedTimerComponentStateRef
    {
        internal IntPtr ptr;
        public string TimerTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_timer_time(this.ptr).AsString();
            return result;
        }
        public string TimerFraction()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_timer_fraction(this.ptr).AsString();
            return result;
        }
        public string TimerSemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_timer_semantic_color(this.ptr).AsString();
            return result;
        }
        public string SegmentTimerTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_segment_timer_time(this.ptr).AsString();
            return result;
        }
        public string SegmentTimerFraction()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_segment_timer_fraction(this.ptr).AsString();
            return result;
        }
        public bool Comparison1Visible()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison1_visible(this.ptr) != 0;
            return result;
        }
        public string Comparison1Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison1_name(this.ptr).AsString();
            return result;
        }
        public string Comparison1Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison1_time(this.ptr).AsString();
            return result;
        }
        public bool Comparison2Visible()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison2_visible(this.ptr) != 0;
            return result;
        }
        public string Comparison2Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison2_name(this.ptr).AsString();
            return result;
        }
        public string Comparison2Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_comparison2_time(this.ptr).AsString();
            return result;
        }
        public string IconChange()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_icon_change(this.ptr).AsString();
            return result;
        }
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.DetailedTimerComponentState_name(this.ptr).AsString();
            return result;
        }
        internal DetailedTimerComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class DetailedTimerComponentStateRefMut : DetailedTimerComponentStateRef
    {
        internal DetailedTimerComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class GeneralLayoutSettingsRef
    {
        internal IntPtr ptr;
        internal GeneralLayoutSettingsRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class GeneralLayoutSettingsRefMut : GeneralLayoutSettingsRef
    {
        internal GeneralLayoutSettingsRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public static GeneralLayoutSettings Default()
        {
            var result = new GeneralLayoutSettings(LiveSplitCoreNative.GeneralLayoutSettings_default());
            return result;
        }
        internal GeneralLayoutSettings(IntPtr ptr) : base(ptr) { }
    }

    public class GraphComponentRef
    {
        internal IntPtr ptr;
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
            var result = LiveSplitCoreNative.GraphComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr).AsString();
            return result;
        }
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

    public class GraphComponentRefMut : GraphComponentRef
    {
        internal GraphComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public GraphComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.GraphComponent_new();
        }
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

    public class GraphComponentStateRef
    {
        internal IntPtr ptr;
        public long PointsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.GraphComponentState_points_len(this.ptr);
            return result;
        }
        public float PointX(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_point_x(this.ptr, (UIntPtr)index);
            return result;
        }
        public float PointY(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_point_y(this.ptr, (UIntPtr)index);
            return result;
        }
        public bool PointIsBestSegment(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_point_is_best_segment(this.ptr, (UIntPtr)index) != 0;
            return result;
        }
        public long HorizontalGridLinesLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.GraphComponentState_horizontal_grid_lines_len(this.ptr);
            return result;
        }
        public float HorizontalGridLine(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_horizontal_grid_line(this.ptr, (UIntPtr)index);
            return result;
        }
        public long VerticalGridLinesLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.GraphComponentState_vertical_grid_lines_len(this.ptr);
            return result;
        }
        public float VerticalGridLine(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_vertical_grid_line(this.ptr, (UIntPtr)index);
            return result;
        }
        public float Middle()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_middle(this.ptr);
            return result;
        }
        public bool IsLiveDeltaActive()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.GraphComponentState_is_live_delta_active(this.ptr) != 0;
            return result;
        }
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

    public class GraphComponentStateRefMut : GraphComponentStateRef
    {
        internal GraphComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class HotkeySystemRef
    {
        internal IntPtr ptr;
        internal HotkeySystemRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class HotkeySystemRefMut : HotkeySystemRef
    {
        internal HotkeySystemRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public HotkeySystem(SharedTimer sharedTimer) : base(IntPtr.Zero)
        {
            if (sharedTimer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("sharedTimer");
            }
            this.ptr = LiveSplitCoreNative.HotkeySystem_new(sharedTimer.ptr);
            sharedTimer.ptr = IntPtr.Zero;
        }
        internal HotkeySystem(IntPtr ptr) : base(ptr) { }
    }

    public class LayoutRef
    {
        internal IntPtr ptr;
        public Layout Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Layout(LiveSplitCoreNative.Layout_clone(this.ptr));
            return result;
        }
        public string SettingsAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Layout_settings_as_json(this.ptr).AsString();
            return result;
        }
        internal LayoutRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class LayoutRefMut : LayoutRef
    {
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
            var result = LiveSplitCoreNative.Layout_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
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
        public void ScrollUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Layout_scroll_up(this.ptr);
        }
        public void ScrollDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Layout_scroll_down(this.ptr);
        }
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
        public Layout() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.Layout_new();
        }
        public static Layout DefaultLayout()
        {
            var result = new Layout(LiveSplitCoreNative.Layout_default_layout());
            return result;
        }
        public static Layout ParseJson(string settings)
        {
            var result = new Layout(LiveSplitCoreNative.Layout_parse_json(settings));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal Layout(IntPtr ptr) : base(ptr) { }
    }

    public class LayoutEditorRef
    {
        internal IntPtr ptr;
        public string StateAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.LayoutEditor_state_as_json(this.ptr).AsString();
            return result;
        }
        internal LayoutEditorRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class LayoutEditorRefMut : LayoutEditorRef
    {
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
            var result = LiveSplitCoreNative.LayoutEditor_layout_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
        public void Select(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_select(this.ptr, (UIntPtr)index);
        }
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
        public void RemoveComponent()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_remove_component(this.ptr);
        }
        public void MoveComponentUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_move_component_up(this.ptr);
        }
        public void MoveComponentDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_move_component_down(this.ptr);
        }
        public void MoveComponent(long dstIndex)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_move_component(this.ptr, (UIntPtr)dstIndex);
        }
        public void DuplicateComponent()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.LayoutEditor_duplicate_component(this.ptr);
        }
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
        public LayoutEditor(Layout layout) : base(IntPtr.Zero)
        {
            if (layout.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("layout");
            }
            this.ptr = LiveSplitCoreNative.LayoutEditor_new(layout.ptr);
            layout.ptr = IntPtr.Zero;
        }
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

    public class ParseRunResultRef
    {
        internal IntPtr ptr;
        public bool ParsedSuccessfully()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.ParseRunResult_parsed_successfully(this.ptr) != 0;
            return result;
        }
        public string TimerKind()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.ParseRunResult_timer_kind(this.ptr).AsString();
            return result;
        }
        internal ParseRunResultRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class ParseRunResultRefMut : ParseRunResultRef
    {
        internal ParseRunResultRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class PossibleTimeSaveComponentRef
    {
        internal IntPtr ptr;
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
            var result = LiveSplitCoreNative.PossibleTimeSaveComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
        public PossibleTimeSaveComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new PossibleTimeSaveComponentState(LiveSplitCoreNative.PossibleTimeSaveComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal PossibleTimeSaveComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class PossibleTimeSaveComponentRefMut : PossibleTimeSaveComponentRef
    {
        internal PossibleTimeSaveComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public PossibleTimeSaveComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.PossibleTimeSaveComponent_new();
        }
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

    public class PossibleTimeSaveComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PossibleTimeSaveComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PossibleTimeSaveComponentState_time(this.ptr).AsString();
            return result;
        }
        internal PossibleTimeSaveComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class PossibleTimeSaveComponentStateRefMut : PossibleTimeSaveComponentStateRef
    {
        internal PossibleTimeSaveComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class PossibleTimeSaveComponentState : PossibleTimeSaveComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.PossibleTimeSaveComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~PossibleTimeSaveComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal PossibleTimeSaveComponentState(IntPtr ptr) : base(ptr) { }
    }

    public class PotentialCleanUpRef
    {
        internal IntPtr ptr;
        public string Message()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PotentialCleanUp_message(this.ptr).AsString();
            return result;
        }
        internal PotentialCleanUpRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class PotentialCleanUpRefMut : PotentialCleanUpRef
    {
        internal PotentialCleanUpRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class PreviousSegmentComponentRef
    {
        internal IntPtr ptr;
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
            var result = LiveSplitCoreNative.PreviousSegmentComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr).AsString();
            return result;
        }
        public PreviousSegmentComponentState State(TimerRef timer, GeneralLayoutSettingsRef layoutSettings)
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
            var result = new PreviousSegmentComponentState(LiveSplitCoreNative.PreviousSegmentComponent_state(this.ptr, timer.ptr, layoutSettings.ptr));
            return result;
        }
        internal PreviousSegmentComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class PreviousSegmentComponentRefMut : PreviousSegmentComponentRef
    {
        internal PreviousSegmentComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public PreviousSegmentComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.PreviousSegmentComponent_new();
        }
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

    public class PreviousSegmentComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PreviousSegmentComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PreviousSegmentComponentState_time(this.ptr).AsString();
            return result;
        }
        public string SemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.PreviousSegmentComponentState_semantic_color(this.ptr).AsString();
            return result;
        }
        internal PreviousSegmentComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class PreviousSegmentComponentStateRefMut : PreviousSegmentComponentStateRef
    {
        internal PreviousSegmentComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class PreviousSegmentComponentState : PreviousSegmentComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.PreviousSegmentComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~PreviousSegmentComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal PreviousSegmentComponentState(IntPtr ptr) : base(ptr) { }
    }

    public class RunRef
    {
        internal IntPtr ptr;
        public Run Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Run(LiveSplitCoreNative.Run_clone(this.ptr));
            return result;
        }
        public string GameName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_game_name(this.ptr).AsString();
            return result;
        }
        public string GameIcon()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_game_icon(this.ptr).AsString();
            return result;
        }
        public string CategoryName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_category_name(this.ptr).AsString();
            return result;
        }
        public string ExtendedFileName(bool useExtendedCategoryName)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_extended_file_name(this.ptr, useExtendedCategoryName).AsString();
            return result;
        }
        public string ExtendedName(bool useExtendedCategoryName)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_extended_name(this.ptr, useExtendedCategoryName).AsString();
            return result;
        }
        public string ExtendedCategoryName(bool showRegion, bool showPlatform, bool showVariables)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_extended_category_name(this.ptr, showRegion, showPlatform, showVariables).AsString();
            return result;
        }
        public uint AttemptCount()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_attempt_count(this.ptr);
            return result;
        }
        public RunMetadataRef Metadata()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataRef(LiveSplitCoreNative.Run_metadata(this.ptr));
            return result;
        }
        public TimeSpanRef Offset()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Run_offset(this.ptr));
            return result;
        }
        public long Len()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_len(this.ptr);
            return result;
        }
        public SegmentRef Segment(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SegmentRef(LiveSplitCoreNative.Run_segment(this.ptr, (UIntPtr)index));
            return result;
        }
        public long AttemptHistoryLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_attempt_history_len(this.ptr);
            return result;
        }
        public AttemptRef AttemptHistoryIndex(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new AttemptRef(LiveSplitCoreNative.Run_attempt_history_index(this.ptr, (UIntPtr)index));
            return result;
        }
        public string SaveAsLss()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_save_as_lss(this.ptr).AsString();
            return result;
        }
        public long CustomComparisonsLen()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.Run_custom_comparisons_len(this.ptr);
            return result;
        }
        public string CustomComparison(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_custom_comparison(this.ptr, (UIntPtr)index).AsString();
            return result;
        }
        public string AutoSplitterSettings()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Run_auto_splitter_settings(this.ptr).AsString();
            return result;
        }
        internal RunRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class RunRefMut : RunRef
    {
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
        public void SetGameName(string game)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Run_set_game_name(this.ptr, game);
        }
        public void SetCategoryName(string category)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Run_set_category_name(this.ptr, category);
        }
        internal RunRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public Run() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.Run_new();
        }
        public static ParseRunResult Parse(IntPtr data, long length, string path, bool loadFiles)
        {
            var result = new ParseRunResult(LiveSplitCoreNative.Run_parse(data, (UIntPtr)length, path, loadFiles));
            return result;
        }
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

    public class RunEditorRef
    {
        internal IntPtr ptr;
        internal RunEditorRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class RunEditorRefMut : RunEditorRef
    {
        public string StateAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_state_as_json(this.ptr).AsString();
            return result;
        }
        public void SelectTimingMethod(byte method)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_select_timing_method(this.ptr, method);
        }
        public void Unselect(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_unselect(this.ptr, (UIntPtr)index);
        }
        public void SelectAdditionally(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_select_additionally(this.ptr, (UIntPtr)index);
        }
        public void SelectOnly(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_select_only(this.ptr, (UIntPtr)index);
        }
        public void SetGameName(string game)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_game_name(this.ptr, game);
        }
        public void SetCategoryName(string category)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_category_name(this.ptr, category);
        }
        public bool ParseAndSetOffset(string offset)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_parse_and_set_offset(this.ptr, offset) != 0;
            return result;
        }
        public bool ParseAndSetAttemptCount(string attempts)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_parse_and_set_attempt_count(this.ptr, attempts) != 0;
            return result;
        }
        public void SetGameIcon(IntPtr data, long length)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_set_game_icon(this.ptr, data, (UIntPtr)length);
        }
        public void RemoveGameIcon()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_game_icon(this.ptr);
        }
        public void InsertSegmentAbove()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_insert_segment_above(this.ptr);
        }
        public void InsertSegmentBelow()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_insert_segment_below(this.ptr);
        }
        public void RemoveSegments()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_segments(this.ptr);
        }
        public void MoveSegmentsUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_move_segments_up(this.ptr);
        }
        public void MoveSegmentsDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_move_segments_down(this.ptr);
        }
        public void SelectedSetIcon(IntPtr data, long length)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_selected_set_icon(this.ptr, data, (UIntPtr)length);
        }
        public void SelectedRemoveIcon()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_selected_remove_icon(this.ptr);
        }
        public void SelectedSetName(string name)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_selected_set_name(this.ptr, name);
        }
        public bool SelectedParseAndSetSplitTime(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_selected_parse_and_set_split_time(this.ptr, time) != 0;
            return result;
        }
        public bool SelectedParseAndSetSegmentTime(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_selected_parse_and_set_segment_time(this.ptr, time) != 0;
            return result;
        }
        public bool SelectedParseAndSetBestSegmentTime(string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_selected_parse_and_set_best_segment_time(this.ptr, time) != 0;
            return result;
        }
        public bool SelectedParseAndSetComparisonTime(string comparison, string time)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_selected_parse_and_set_comparison_time(this.ptr, comparison, time) != 0;
            return result;
        }
        public bool AddComparison(string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_add_comparison(this.ptr, comparison) != 0;
            return result;
        }
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
        public void RemoveComparison(string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_remove_comparison(this.ptr, comparison);
        }
        public bool RenameComparison(string oldName, string newName)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunEditor_rename_comparison(this.ptr, oldName, newName) != 0;
            return result;
        }
        public void ClearHistory()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_clear_history(this.ptr);
        }
        public void ClearTimes()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.RunEditor_clear_times(this.ptr);
        }
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
        public RunEditor(Run run) : base(IntPtr.Zero)
        {
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            this.ptr = LiveSplitCoreNative.RunEditor_new(run.ptr);
            run.ptr = IntPtr.Zero;
        }
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

    public class RunMetadataRef
    {
        internal IntPtr ptr;
        public string RunId()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_run_id(this.ptr).AsString();
            return result;
        }
        public string PlatformName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_platform_name(this.ptr).AsString();
            return result;
        }
        public bool UsesEmulator()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_uses_emulator(this.ptr) != 0;
            return result;
        }
        public string RegionName()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadata_region_name(this.ptr).AsString();
            return result;
        }
        public RunMetadataVariablesIter Variables()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataVariablesIter(LiveSplitCoreNative.RunMetadata_variables(this.ptr));
            return result;
        }
        internal RunMetadataRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class RunMetadataRefMut : RunMetadataRef
    {
        internal RunMetadataRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class RunMetadataVariableRef
    {
        internal IntPtr ptr;
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataVariable_name(this.ptr).AsString();
            return result;
        }
        public string Value()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.RunMetadataVariable_value(this.ptr).AsString();
            return result;
        }
        internal RunMetadataVariableRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class RunMetadataVariableRefMut : RunMetadataVariableRef
    {
        internal RunMetadataVariableRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class RunMetadataVariable : RunMetadataVariableRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.RunMetadataVariable_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadataVariable()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadataVariable(IntPtr ptr) : base(ptr) { }
    }

    public class RunMetadataVariablesIterRef
    {
        internal IntPtr ptr;
        internal RunMetadataVariablesIterRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class RunMetadataVariablesIterRefMut : RunMetadataVariablesIterRef
    {
        public RunMetadataVariableRef Next()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunMetadataVariableRef(LiveSplitCoreNative.RunMetadataVariablesIter_next(this.ptr));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        internal RunMetadataVariablesIterRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class RunMetadataVariablesIter : RunMetadataVariablesIterRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.RunMetadataVariablesIter_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~RunMetadataVariablesIter()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal RunMetadataVariablesIter(IntPtr ptr) : base(ptr) { }
    }

    public class SegmentRef
    {
        internal IntPtr ptr;
        public string Name()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Segment_name(this.ptr).AsString();
            return result;
        }
        public string Icon()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Segment_icon(this.ptr).AsString();
            return result;
        }
        public TimeRef Comparison(string comparison)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Segment_comparison(this.ptr, comparison));
            return result;
        }
        public TimeRef PersonalBestSplitTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Segment_personal_best_split_time(this.ptr));
            return result;
        }
        public TimeRef BestSegmentTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeRef(LiveSplitCoreNative.Segment_best_segment_time(this.ptr));
            return result;
        }
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

    public class SegmentRefMut : SegmentRef
    {
        internal SegmentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public Segment(string name) : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.Segment_new(name);
        }
        internal Segment(IntPtr ptr) : base(ptr) { }
    }

    public class SegmentHistoryRef
    {
        internal IntPtr ptr;
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

    public class SegmentHistoryRefMut : SegmentHistoryRef
    {
        internal SegmentHistoryRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class SegmentHistoryElementRef
    {
        internal IntPtr ptr;
        public int Index()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SegmentHistoryElement_index(this.ptr);
            return result;
        }
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

    public class SegmentHistoryElementRefMut : SegmentHistoryElementRef
    {
        internal SegmentHistoryElementRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class SegmentHistoryIterRef
    {
        internal IntPtr ptr;
        internal SegmentHistoryIterRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SegmentHistoryIterRefMut : SegmentHistoryIterRef
    {
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

    public class SeparatorComponentRef
    {
        internal IntPtr ptr;
        internal SeparatorComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SeparatorComponentRefMut : SeparatorComponentRef
    {
        internal SeparatorComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public SeparatorComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SeparatorComponent_new();
        }
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

    public class SettingValueRef
    {
        internal IntPtr ptr;
        internal SettingValueRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SettingValueRefMut : SettingValueRef
    {
        internal SettingValueRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public static SettingValue FromBool(bool value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_bool(value));
            return result;
        }
        public static SettingValue FromUint(ulong value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_uint(value));
            return result;
        }
        public static SettingValue FromInt(long value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_int(value));
            return result;
        }
        public static SettingValue FromString(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_string(value));
            return result;
        }
        public static SettingValue FromOptionalString(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_string(value));
            return result;
        }
        public static SettingValue FromOptionalEmptyString()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_empty_string());
            return result;
        }
        public static SettingValue FromFloat(double value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_float(value));
            return result;
        }
        public static SettingValue FromAccuracy(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_accuracy(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        public static SettingValue FromDigitsFormat(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_digits_format(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        public static SettingValue FromOptionalTimingMethod(string value)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_timing_method(value));
            if (result.ptr == IntPtr.Zero)
            {
                return null;
            }
            return result;
        }
        public static SettingValue FromOptionalEmptyTimingMethod()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_empty_timing_method());
            return result;
        }
        public static SettingValue FromColor(float r, float g, float b, float a)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_color(r, g, b, a));
            return result;
        }
        public static SettingValue FromOptionalColor(float r, float g, float b, float a)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_color(r, g, b, a));
            return result;
        }
        public static SettingValue FromOptionalEmptyColor()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_optional_empty_color());
            return result;
        }
        public static SettingValue FromTransparentGradient()
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_transparent_gradient());
            return result;
        }
        public static SettingValue FromVerticalGradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_vertical_gradient(r1, g1, b1, a1, r2, g2, b2, a2));
            return result;
        }
        public static SettingValue FromHorizontalGradient(float r1, float g1, float b1, float a1, float r2, float g2, float b2, float a2)
        {
            var result = new SettingValue(LiveSplitCoreNative.SettingValue_from_horizontal_gradient(r1, g1, b1, a1, r2, g2, b2, a2));
            return result;
        }
        internal SettingValue(IntPtr ptr) : base(ptr) { }
    }

    public class SharedTimerRef
    {
        internal IntPtr ptr;
        public SharedTimer Share()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new SharedTimer(LiveSplitCoreNative.SharedTimer_share(this.ptr));
            return result;
        }
        public TimerReadLock Read()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerReadLock(LiveSplitCoreNative.SharedTimer_read(this.ptr));
            return result;
        }
        public TimerWriteLock Write()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimerWriteLock(LiveSplitCoreNative.SharedTimer_write(this.ptr));
            return result;
        }
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

    public class SharedTimerRefMut : SharedTimerRef
    {
        internal SharedTimerRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class SplitsComponentRef
    {
        internal IntPtr ptr;
        internal SplitsComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SplitsComponentRefMut : SplitsComponentRef
    {
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
            var result = LiveSplitCoreNative.SplitsComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr).AsString();
            return result;
        }
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
        public void ScrollUp()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_scroll_up(this.ptr);
        }
        public void ScrollDown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_scroll_down(this.ptr);
        }
        public void SetVisualSplitCount(long count)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_visual_split_count(this.ptr, (UIntPtr)count);
        }
        public void SetSplitPreviewCount(long count)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_split_preview_count(this.ptr, (UIntPtr)count);
        }
        public void SetAlwaysShowLastSplit(bool alwaysShowLastSplit)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.SplitsComponent_set_always_show_last_split(this.ptr, alwaysShowLastSplit);
        }
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
        public SplitsComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SplitsComponent_new();
        }
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

    public class SplitsComponentStateRef
    {
        internal IntPtr ptr;
        public bool FinalSeparatorShown()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_final_separator_shown(this.ptr) != 0;
            return result;
        }
        public long Len()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_len(this.ptr);
            return result;
        }
        public long IconChangeCount()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_icon_change_count(this.ptr);
            return result;
        }
        public long IconChangeSegmentIndex(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = (long)LiveSplitCoreNative.SplitsComponentState_icon_change_segment_index(this.ptr, (UIntPtr)index);
            return result;
        }
        public string IconChangeIcon(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_icon_change_icon(this.ptr, (UIntPtr)index).AsString();
            return result;
        }
        public string Name(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_name(this.ptr, (UIntPtr)index).AsString();
            return result;
        }
        public string Delta(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_delta(this.ptr, (UIntPtr)index).AsString();
            return result;
        }
        public string Time(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_time(this.ptr, (UIntPtr)index).AsString();
            return result;
        }
        public string SemanticColor(long index)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SplitsComponentState_semantic_color(this.ptr, (UIntPtr)index).AsString();
            return result;
        }
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

    public class SplitsComponentStateRefMut : SplitsComponentStateRef
    {
        internal SplitsComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class SumOfBestCleanerRef
    {
        internal IntPtr ptr;
        internal SumOfBestCleanerRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SumOfBestCleanerRefMut : SumOfBestCleanerRef
    {
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

    public class SumOfBestComponentRef
    {
        internal IntPtr ptr;
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
            var result = LiveSplitCoreNative.SumOfBestComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
        public SumOfBestComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new SumOfBestComponentState(LiveSplitCoreNative.SumOfBestComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal SumOfBestComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SumOfBestComponentRefMut : SumOfBestComponentRef
    {
        internal SumOfBestComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public SumOfBestComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.SumOfBestComponent_new();
        }
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

    public class SumOfBestComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SumOfBestComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.SumOfBestComponentState_time(this.ptr).AsString();
            return result;
        }
        internal SumOfBestComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class SumOfBestComponentStateRefMut : SumOfBestComponentStateRef
    {
        internal SumOfBestComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class SumOfBestComponentState : SumOfBestComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.SumOfBestComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~SumOfBestComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal SumOfBestComponentState(IntPtr ptr) : base(ptr) { }
    }

    public class TextComponentRef
    {
        internal IntPtr ptr;
        public string StateAsJson()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponent_state_as_json(this.ptr).AsString();
            return result;
        }
        public TextComponentState State()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TextComponentState(LiveSplitCoreNative.TextComponent_state(this.ptr));
            return result;
        }
        internal TextComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TextComponentRefMut : TextComponentRef
    {
        public void SetCenter(string text)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.TextComponent_set_center(this.ptr, text);
        }
        public void SetLeft(string text)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.TextComponent_set_left(this.ptr, text);
        }
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
        public TextComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TextComponent_new();
        }
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

    public class TextComponentStateRef
    {
        internal IntPtr ptr;
        public string Left()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_left(this.ptr).AsString();
            return result;
        }
        public string Right()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_right(this.ptr).AsString();
            return result;
        }
        public string Center()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TextComponentState_center(this.ptr).AsString();
            return result;
        }
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

    public class TextComponentStateRefMut : TextComponentStateRef
    {
        internal TextComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class TimeRef
    {
        internal IntPtr ptr;
        public Time Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new Time(LiveSplitCoreNative.Time_clone(this.ptr));
            return result;
        }
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

    public class TimeRefMut : TimeRef
    {
        internal TimeRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class TimeSpanRef
    {
        internal IntPtr ptr;
        public TimeSpan Clone()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpan(LiveSplitCoreNative.TimeSpan_clone(this.ptr));
            return result;
        }
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

    public class TimeSpanRefMut : TimeSpanRef
    {
        internal TimeSpanRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public static TimeSpan FromSeconds(double seconds)
        {
            var result = new TimeSpan(LiveSplitCoreNative.TimeSpan_from_seconds(seconds));
            return result;
        }
        internal TimeSpan(IntPtr ptr) : base(ptr) { }
    }

    public class TimerRef
    {
        internal IntPtr ptr;
        public byte CurrentTimingMethod()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_current_timing_method(this.ptr);
            return result;
        }
        public string CurrentComparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_current_comparison(this.ptr).AsString();
            return result;
        }
        public bool IsGameTimeInitialized()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_is_game_time_initialized(this.ptr) != 0;
            return result;
        }
        public bool IsGameTimePaused()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_is_game_time_paused(this.ptr) != 0;
            return result;
        }
        public TimeSpanRef LoadingTimes()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new TimeSpanRef(LiveSplitCoreNative.Timer_loading_times(this.ptr));
            return result;
        }
        public byte CurrentPhase()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.Timer_current_phase(this.ptr);
            return result;
        }
        public RunRef GetRun()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = new RunRef(LiveSplitCoreNative.Timer_get_run(this.ptr));
            return result;
        }
        public void PrintDebug()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_print_debug(this.ptr);
        }
        internal TimerRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TimerRefMut : TimerRef
    {
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
        public void Start()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_start(this.ptr);
        }
        public void Split()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_split(this.ptr);
        }
        public void SplitOrStart()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_split_or_start(this.ptr);
        }
        public void SkipSplit()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_skip_split(this.ptr);
        }
        public void UndoSplit()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_undo_split(this.ptr);
        }
        public void Reset(bool updateSplits)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_reset(this.ptr, updateSplits);
        }
        public void Pause()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_pause(this.ptr);
        }
        public void Resume()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_resume(this.ptr);
        }
        public void TogglePause()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_toggle_pause(this.ptr);
        }
        public void TogglePauseOrStart()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_toggle_pause_or_start(this.ptr);
        }
        public void UndoAllPauses()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_undo_all_pauses(this.ptr);
        }
        public void SetCurrentTimingMethod(byte method)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_set_current_timing_method(this.ptr, method);
        }
        public void SwitchToNextComparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_switch_to_next_comparison(this.ptr);
        }
        public void SwitchToPreviousComparison()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_switch_to_previous_comparison(this.ptr);
        }
        public void InitializeGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_initialize_game_time(this.ptr);
        }
        public void UninitializeGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_uninitialize_game_time(this.ptr);
        }
        public void PauseGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_pause_game_time(this.ptr);
        }
        public void UnpauseGameTime()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            LiveSplitCoreNative.Timer_unpause_game_time(this.ptr);
        }
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
        internal TimerRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public Timer(Run run) : base(IntPtr.Zero)
        {
            if (run.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("run");
            }
            this.ptr = LiveSplitCoreNative.Timer_new(run.ptr);
            run.ptr = IntPtr.Zero;
        }
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
        internal Timer(IntPtr ptr) : base(ptr) { }
    }

    public class TimerComponentRef
    {
        internal IntPtr ptr;
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
            var result = LiveSplitCoreNative.TimerComponent_state_as_json(this.ptr, timer.ptr, layoutSettings.ptr).AsString();
            return result;
        }
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

    public class TimerComponentRefMut : TimerComponentRef
    {
        internal TimerComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public TimerComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TimerComponent_new();
        }
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

    public class TimerComponentStateRef
    {
        internal IntPtr ptr;
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimerComponentState_time(this.ptr).AsString();
            return result;
        }
        public string Fraction()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimerComponentState_fraction(this.ptr).AsString();
            return result;
        }
        public string SemanticColor()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TimerComponentState_semantic_color(this.ptr).AsString();
            return result;
        }
        internal TimerComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TimerComponentStateRefMut : TimerComponentStateRef
    {
        internal TimerComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class TimerReadLockRef
    {
        internal IntPtr ptr;
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

    public class TimerReadLockRefMut : TimerReadLockRef
    {
        internal TimerReadLockRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class TimerWriteLockRef
    {
        internal IntPtr ptr;
        internal TimerWriteLockRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TimerWriteLockRefMut : TimerWriteLockRef
    {
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

    public class TitleComponentRef
    {
        internal IntPtr ptr;
        internal TitleComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TitleComponentRefMut : TitleComponentRef
    {
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
            var result = LiveSplitCoreNative.TitleComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
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
        public TitleComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TitleComponent_new();
        }
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

    public class TitleComponentStateRef
    {
        internal IntPtr ptr;
        public string IconChange()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_icon_change(this.ptr).AsString();
            return result;
        }
        public string Line1()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_line1(this.ptr).AsString();
            return result;
        }
        public string Line2()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_line2(this.ptr).AsString();
            return result;
        }
        public bool IsCentered()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_is_centered(this.ptr) != 0;
            return result;
        }
        public bool ShowsFinishedRuns()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_shows_finished_runs(this.ptr) != 0;
            return result;
        }
        public uint FinishedRuns()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_finished_runs(this.ptr);
            return result;
        }
        public bool ShowsAttempts()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TitleComponentState_shows_attempts(this.ptr) != 0;
            return result;
        }
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

    public class TitleComponentStateRefMut : TitleComponentStateRef
    {
        internal TitleComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

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

    public class TotalPlaytimeComponentRef
    {
        internal IntPtr ptr;
        internal TotalPlaytimeComponentRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TotalPlaytimeComponentRefMut : TotalPlaytimeComponentRef
    {
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
            var result = LiveSplitCoreNative.TotalPlaytimeComponent_state_as_json(this.ptr, timer.ptr).AsString();
            return result;
        }
        public TotalPlaytimeComponentState State(TimerRef timer)
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            if (timer.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("timer");
            }
            var result = new TotalPlaytimeComponentState(LiveSplitCoreNative.TotalPlaytimeComponent_state(this.ptr, timer.ptr));
            return result;
        }
        internal TotalPlaytimeComponentRefMut(IntPtr ptr) : base(ptr) { }
    }

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
        public TotalPlaytimeComponent() : base(IntPtr.Zero)
        {
            this.ptr = LiveSplitCoreNative.TotalPlaytimeComponent_new();
        }
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

    public class TotalPlaytimeComponentStateRef
    {
        internal IntPtr ptr;
        public string Text()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TotalPlaytimeComponentState_text(this.ptr).AsString();
            return result;
        }
        public string Time()
        {
            if (this.ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("this");
            }
            var result = LiveSplitCoreNative.TotalPlaytimeComponentState_time(this.ptr).AsString();
            return result;
        }
        internal TotalPlaytimeComponentStateRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }
    }

    public class TotalPlaytimeComponentStateRefMut : TotalPlaytimeComponentStateRef
    {
        internal TotalPlaytimeComponentStateRefMut(IntPtr ptr) : base(ptr) { }
    }

    public class TotalPlaytimeComponentState : TotalPlaytimeComponentStateRefMut, IDisposable
    {
        private void Drop()
        {
            if (ptr != IntPtr.Zero)
            {
                LiveSplitCoreNative.TotalPlaytimeComponentState_drop(this.ptr);
                ptr = IntPtr.Zero;
            }
        }
        ~TotalPlaytimeComponentState()
        {
            Drop();
        }
        public void Dispose()
        {
            Drop();
            GC.SuppressFinalize(this);
        }
        internal TotalPlaytimeComponentState(IntPtr ptr) : base(ptr) { }
    }

    public static class LiveSplitCoreNative
    {
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
        public static extern uint BlankSpaceComponentState_height(IntPtr self);
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
        public static extern void CurrentComparisonComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString CurrentComparisonComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString CurrentComparisonComponentState_comparison(IntPtr self);
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
        public static extern void CurrentPaceComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString CurrentPaceComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString CurrentPaceComponentState_time(IntPtr self);
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
        public static extern void DeltaComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DeltaComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DeltaComponentState_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DeltaComponentState_semantic_color(IntPtr self);
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
        public static extern LSCoreString DetailedTimerComponentState_icon_change(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString DetailedTimerComponentState_name(IntPtr self);
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
        public static extern IntPtr HotkeySystem_new(IntPtr shared_timer);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HotkeySystem_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_default_layout();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_parse_json(string settings);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Layout_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Layout_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Layout_settings_as_json(IntPtr self);
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
        public static extern void ParseRunResult_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ParseRunResult_unwrap(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ParseRunResult_parsed_successfully(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString ParseRunResult_timer_kind(IntPtr self);
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
        public static extern void PossibleTimeSaveComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PossibleTimeSaveComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PossibleTimeSaveComponentState_time(IntPtr self);
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
        public static extern void PreviousSegmentComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PreviousSegmentComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PreviousSegmentComponentState_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString PreviousSegmentComponentState_semantic_color(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_parse(IntPtr data, UIntPtr length, string path, bool load_files);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_parse_file_handle(long handle, string path, bool load_files);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Run_clone(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_game_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Run_game_icon(IntPtr self);
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
        public static extern void Run_set_game_name(IntPtr self, string game);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Run_set_category_name(IntPtr self, string category);
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
        public static extern void RunEditor_set_game_name(IntPtr self, string game);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_category_name(IntPtr self, string category);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_parse_and_set_offset(IntPtr self, string offset);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_parse_and_set_attempt_count(IntPtr self, string attempts);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_set_game_icon(IntPtr self, IntPtr data, UIntPtr length);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_game_icon(IntPtr self);
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
        public static extern void RunEditor_selected_set_icon(IntPtr self, IntPtr data, UIntPtr length);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_selected_remove_icon(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_selected_set_name(IntPtr self, string name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_selected_parse_and_set_split_time(IntPtr self, string time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_selected_parse_and_set_segment_time(IntPtr self, string time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_selected_parse_and_set_best_segment_time(IntPtr self, string time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_selected_parse_and_set_comparison_time(IntPtr self, string comparison, string time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_add_comparison(IntPtr self, string comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_import_comparison(IntPtr self, IntPtr run, string comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunEditor_remove_comparison(IntPtr self, string comparison);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte RunEditor_rename_comparison(IntPtr self, string old_name, string new_name);
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
        public static extern IntPtr RunMetadata_variables(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunMetadataVariable_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadataVariable_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString RunMetadataVariable_value(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunMetadataVariablesIter_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RunMetadataVariablesIter_next(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_new(string name);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Segment_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Segment_name(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString Segment_icon(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Segment_comparison(IntPtr self, string comparison);
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
        public static extern IntPtr SeparatorComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SeparatorComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SeparatorComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_bool(bool value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_uint(ulong value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_int(long value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_string(string value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_string(string value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_empty_string();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_float(double value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_accuracy(string value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_digits_format(string value);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SettingValue_from_optional_timing_method(string value);
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
        public static extern UIntPtr SplitsComponentState_icon_change_segment_index(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_icon_change_icon(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_name(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_delta(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_time(IntPtr self, UIntPtr index);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SplitsComponentState_semantic_color(IntPtr self, UIntPtr index);
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
        public static extern void SumOfBestComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SumOfBestComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString SumOfBestComponentState_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TextComponent_new();
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TextComponent_into_generic(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TextComponent_state_as_json(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TextComponent_state(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_set_center(IntPtr self, string text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_set_left(IntPtr self, string text);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TextComponent_set_right(IntPtr self, string text);
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
        public static extern void Timer_print_debug(IntPtr self);
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
        public static extern void Timer_uninitialize_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_pause_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_unpause_game_time(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_set_game_time(IntPtr self, IntPtr time);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Timer_set_loading_times(IntPtr self, IntPtr time);
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
        public static extern LSCoreString TitleComponentState_icon_change(IntPtr self);
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
        public static extern void TotalPlaytimeComponentState_drop(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TotalPlaytimeComponentState_text(IntPtr self);
        [DllImport("livesplit_core", CallingConvention = CallingConvention.Cdecl)]
        public static extern LSCoreString TotalPlaytimeComponentState_time(IntPtr self);
    }

    public class LSCoreString : SafeHandle
    {
        public LSCoreString() : base(IntPtr.Zero, false) { }

        public override bool IsInvalid
        {
            get { return false; }
        }

        public string AsString()
        {
            if (handle == IntPtr.Zero)
                return null;

            int len = 0;
            while (Marshal.ReadByte(handle, len) != 0) { ++len; }
            byte[] buffer = new byte[len];
            Marshal.Copy(handle, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }
}
