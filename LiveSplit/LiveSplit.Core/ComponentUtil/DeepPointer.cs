using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
#pragma warning disable 1591

// Note: Please be careful when modifying this because it could break existing components!

namespace LiveSplit.ComponentUtil
{
    using SizeT = UIntPtr;
    using OffsetT = Int32;

    public class DeepPointer
    {
        private List<OffsetT> _offsets;
        private OffsetT _base;
        private string _module;

        public DeepPointer(string module, OffsetT base_, params OffsetT[] offsets)
            : this(base_, offsets)
        {
            _module = module.ToLower();
        }

        public DeepPointer(OffsetT base_, params OffsetT[] offsets)
        {
            _base = base_;
            _offsets = new List<OffsetT>();
            _offsets.Add(0); // deref base first
            _offsets.AddRange(offsets);
        }

        public T Deref<T>(Process process, T default_ = default(T)) where T : struct // all value types including structs
        {
            T val;
            if (!this.Deref(process, out val))
                val = default_;
            return val;
        }

        public bool Deref<T>(Process process, out T value) where T : struct
        {
            OffsetT offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;
            if (!this.DerefOffsets(process, out ptr)
                || !process.ReadValue(ptr + offset, out value))
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public byte[] DerefBytes(Process process, int count)
        {
            byte[] bytes;
            if (!this.DerefBytes(process, count, out bytes))
                bytes = null;
            return bytes;
        }

        public bool DerefBytes(Process process, int count, out byte[] value)
        {
            OffsetT offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;
            if (!this.DerefOffsets(process, out ptr)
                || !process.ReadBytes(ptr + offset, count, out value))
            {
                value = null;
                return false;
            }

            return true;
        }

        public string DerefString(Process process, int numBytes, string default_ = null)
        {
            string str;
            if (!this.DerefString(process, ReadStringType.AutoDetect, numBytes, out str))
                str = default_;
            return str;
        }

        public string DerefString(Process process, ReadStringType type, int numBytes, string default_ = null)
        {
            string str;
            if (!this.DerefString(process, type, numBytes, out str))
                str = default_;
            return str;
        }

        public bool DerefString(Process process, int numBytes, out string str)
        {
            return this.DerefString(process, ReadStringType.AutoDetect, numBytes, out str);
        }

        public bool DerefString(Process process, ReadStringType type, int numBytes, out string str)
        {
            var sb = new StringBuilder(numBytes);
            if (!this.DerefString(process, type, sb))
            {
                str = null;
                return false;
            }
            str = sb.ToString();
            return true;
        }

        public bool DerefString(Process process, StringBuilder sb)
        {
            return this.DerefString(process, ReadStringType.AutoDetect, sb);
        }

        public bool DerefString(Process process, ReadStringType type, StringBuilder sb)
        {
            OffsetT offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;
            if (!this.DerefOffsets(process, out ptr)
                || !process.ReadString(ptr + offset, type, sb))
            {
                return false;
            }
            return true;
        }

        bool DerefOffsets(Process process, out IntPtr ptr)
        {
            bool is64Bit = process.Is64Bit();

            if (!String.IsNullOrEmpty(_module))
            {
                ProcessModuleWow64Safe module = process.ModulesWow64Safe()
                    .FirstOrDefault(m => m.ModuleName.ToLower() == _module);
                if (module == null)
                {
                    ptr = IntPtr.Zero;
                    return false;
                }

                ptr = module.BaseAddress + _base;
            }
            else
            {
                ptr = process.MainModuleWow64Safe().BaseAddress + _base;
            }


            for (int i = 0; i < _offsets.Count - 1; i++)
            {
                if (!process.ReadPointer(ptr + _offsets[i], is64Bit, out ptr)
                    || ptr == IntPtr.Zero)
                {
                    return false;
                }
            }

            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3f
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int IX { get { return (int)this.X; } }
        public int IY { get { return (int)this.Y; } }
        public int IZ { get { return (int)this.Z; } }

        public Vector3f(float x, float y, float z) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float Distance(Vector3f other)
        {
            float result = (this.X - other.X) * (this.X - other.X) +
                (this.Y - other.Y) * (this.Y - other.Y) +
                (this.Z - other.Z) * (this.Z - other.Z);
            return (float)Math.Sqrt(result);
        }

        public float DistanceXY(Vector3f other)
        {
            float result = (this.X - other.X) * (this.X - other.X) +
                (this.Y - other.Y) * (this.Y - other.Y);
            return (float)Math.Sqrt(result);
        }

        public bool BitEquals(Vector3f other)
        {
            return    this.X.BitEquals(other.X)
                   && this.Y.BitEquals(other.Y)
                   && this.Z.BitEquals(other.Z);
        }

        public bool BitEqualsXY(Vector3f other)
        {
            return    this.X.BitEquals(other.X)
                   && this.Y.BitEquals(other.Y);
        }

        public override string ToString()
        {
            return this.X + " " + this.Y + " " + this.Z;
        }
    }
}
