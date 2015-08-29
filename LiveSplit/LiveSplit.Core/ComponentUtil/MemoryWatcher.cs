using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
#pragma warning disable 1591

// Note: Please be careful when modifying this because it could break existing components!

namespace LiveSplit.ComponentUtil
{
    public class MemoryWatcherList : List<MemoryWatcher>
    {
        public delegate void MemoryWatcherDataChangedEventHandler(MemoryWatcher watcher);
        public event MemoryWatcherDataChangedEventHandler OnWatcherDataChanged;

        public MemoryWatcher this[string name]
        {
            get { return this.First(w => w.Name == name); }
        }

        public void UpdateAll(Process process)
        {
            if (this.OnWatcherDataChanged != null)
            {
                var changedList = new List<MemoryWatcher>();

                foreach (var watcher in this)
                {
                    bool changed = watcher.Update(process);
                    if (changed)
                        changedList.Add(watcher);
                }

                // only report changes when all of the other watches are updated too
                foreach (var watcher in changedList)
                {
                    this.OnWatcherDataChanged(watcher);
                }
            }
            else
            {
                foreach (var watcher in this)
                {
                    watcher.Update(process);
                }
            }
        }

        public void ResetAll()
        {
            foreach (var watcher in this)
            {
                watcher.Reset();
            }
        }
    }

    public abstract class MemoryWatcher
    {
        public enum ReadFailAction
        {
            DontUpdate,
            SetZeroOrNull,
        }

        public string Name { get; set; }
        public bool Enabled { get; set; }
        public object Current { get; set; }
        public object Old { get; set; }
        public bool Changed { get; protected set; }
        public TimeSpan? UpdateInterval { get; set; }
        public ReadFailAction FailAction { get; set; }

        protected bool InitialUpdate { get; set; }
        protected DateTime? LastUpdateTime { get; set; }
        protected DeepPointer DeepPtr { get; set; }
        protected IntPtr Address { get; set; }

        protected AddressType AddrType { get; private set; }
        protected enum AddressType { DeepPointer, Absolute }

        protected MemoryWatcher(DeepPointer pointer)
        {
            this.DeepPtr = pointer;
            this.AddrType = AddressType.DeepPointer;
            this.Enabled = true;
            this.FailAction = ReadFailAction.DontUpdate;
        }

        protected MemoryWatcher(IntPtr address)
        {
            this.Address = address;
            this.AddrType = AddressType.Absolute;
            this.Enabled = true;
            this.FailAction = ReadFailAction.DontUpdate;
        }

        /// <summary>
        /// Updates the watcher and returns true if the value has changed.
        /// </summary>
        public abstract bool Update(Process process);
            
        public abstract void Reset();

        protected bool CheckInterval()
        {
            if (this.UpdateInterval.HasValue)
            {
                if (this.LastUpdateTime.HasValue)
                {
                    if (DateTime.Now - this.LastUpdateTime.Value < this.UpdateInterval.Value)
                        return false;
                }
                this.LastUpdateTime = DateTime.Now;
            }
            return true;
        }
    }

    public class StringWatcher : MemoryWatcher
    {
        public new string Current
        {
            get { return (string)base.Current; }
            set { base.Current = value; }
        }
        public new string Old
        {
            get { return (string)base.Old; }
            set { base.Current = value; }
        }

        public delegate void StringChangedEventHandler(string old, string current);
        public event StringChangedEventHandler OnChanged;

        private ReadStringType _stringType;
        private int _numBytes;

        public StringWatcher(DeepPointer pointer, ReadStringType type, int numBytes)
            : base(pointer)
        {
            _stringType = type;
            _numBytes = numBytes;
        }

        public StringWatcher(DeepPointer pointer, int numBytes)
            : this(pointer, ReadStringType.AutoDetect, numBytes) { }

        public StringWatcher(IntPtr address, ReadStringType type, int numBytes)
            : base(address)
        {
            _stringType = type;
            _numBytes = numBytes;
        }

        public StringWatcher(IntPtr address, int numBytes)
            : this(address, ReadStringType.AutoDetect, numBytes) { }

        public override bool Update(Process process)
        {
            this.Changed = false;

            if (!this.Enabled)
                return false;

            if (!this.CheckInterval())
                return false;

            string str;
            bool success;
            if (this.AddrType == AddressType.DeepPointer)
                success = this.DeepPtr.DerefString(process, _stringType, _numBytes, out str);
            else
                success = process.ReadString(this.Address, _stringType, _numBytes, out str);

            if (success)
            {
                base.Old = base.Current;
                base.Current = str;
            }
            else
            {
                if (this.FailAction == ReadFailAction.DontUpdate)
                    return false;

                base.Old = base.Current;
                base.Current = str;
            }

            if (!this.InitialUpdate)
            {
                this.InitialUpdate = true;
                return false;
            }

            if (!this.Current.Equals(this.Old))
            {
                if (this.OnChanged != null)
                    this.OnChanged(this.Old, this.Current);
                this.Changed = true;
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            base.Current = null;
            base.Old = null;
            this.InitialUpdate = false;
        }
    }

    public class MemoryWatcher<T> : MemoryWatcher where T : struct
    {
        public new T Current
        {
            get { return (T)(base.Current ?? default(T)); }
            set { base.Current = value; }
        }
        public new T Old
        {
            get { return (T)(base.Old ?? default(T)); }
            set { base.Current = value; }
        }

        public delegate void DataChangedEventHandler(T old, T current);
        public event DataChangedEventHandler OnChanged;

        public MemoryWatcher(DeepPointer pointer)
            : base(pointer) { }
        public MemoryWatcher(IntPtr address)
            : base(address) { }

        public override bool Update(Process process)
        {
            this.Changed = false;

            if (!this.Enabled)
                return false;

            if (!this.CheckInterval())
                return false;

            base.Old = this.Current;

            T val;
            bool success;
            if (this.AddrType == AddressType.DeepPointer)
                success = this.DeepPtr.Deref(process, out val);
            else
                success = process.ReadValue(this.Address, out val);

            if (success)
            {
                base.Old = base.Current;
                base.Current = val;
            }
            else
            {
                if (this.FailAction == ReadFailAction.DontUpdate)
                    return false;

                base.Old = base.Current;
                base.Current = val;
            }

            if (!this.InitialUpdate)
            {
                this.InitialUpdate = true;
                return false;
            }

            if (!this.Current.Equals(this.Old))
            {
                if (this.OnChanged != null)
                    this.OnChanged(this.Old, this.Current);
                this.Changed = true;
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            base.Current = default(T);
            base.Old = default(T);
            this.InitialUpdate = false;
        }
    }
}
