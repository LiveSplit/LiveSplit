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
    using OffsetT = Int32;

    public class DeepPointer
    {
        private IntPtr _absoluteBase;
        private bool _usingAbsoluteBase;

        private OffsetT _base;
        private List<OffsetT> _offsets;
        private string _module;
        
        public DeepPointer(IntPtr absoluteBase, params OffsetT[] offsets)
        {
            _absoluteBase = absoluteBase;
            _usingAbsoluteBase = true;

            InitializeOffsets(offsets);
        }

        public DeepPointer(string module, OffsetT base_, params OffsetT[] offsets)
            : this(base_, offsets)
        {
            _module = module.ToLower();
        }

        public DeepPointer(OffsetT base_, params OffsetT[] offsets)
        {
            _base = base_;
            InitializeOffsets(offsets);
        }

        public T Deref<T>(Process process, T default_ = default(T)) where T : struct // all value types including structs
        {
            T val;
            if (!Deref(process, out val))
                val = default_;
            return val;
        }

        public bool Deref<T>(Process process, out T value) where T : struct
        {
            IntPtr ptr;
            if (!DerefOffsets(process, out ptr)
                || !process.ReadValue(ptr, out value))
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public byte[] DerefBytes(Process process, int count)
        {
            byte[] bytes;
            if (!DerefBytes(process, count, out bytes))
                bytes = null;
            return bytes;
        }

        public bool DerefBytes(Process process, int count, out byte[] value)
        {
            IntPtr ptr;
            if (!DerefOffsets(process, out ptr)
                || !process.ReadBytes(ptr, count, out value))
            {
                value = null;
                return false;
            }

            return true;
        }

        public string DerefString(Process process, int numBytes, string default_ = null)
        {
            string str;
            if (!DerefString(process, ReadStringType.AutoDetect, numBytes, out str))
                str = default_;
            return str;
        }

        public string DerefString(Process process, ReadStringType type, int numBytes, string default_ = null)
        {
            string str;
            if (!DerefString(process, type, numBytes, out str))
                str = default_;
            return str;
        }

        public bool DerefString(Process process, int numBytes, out string str)
        {
            return DerefString(process, ReadStringType.AutoDetect, numBytes, out str);
        }

        public bool DerefString(Process process, ReadStringType type, int numBytes, out string str)
        {
            var sb = new StringBuilder(numBytes);
            if (!DerefString(process, type, sb))
            {
                str = null;
                return false;
            }
            str = sb.ToString();
            return true;
        }

        public bool DerefString(Process process, StringBuilder sb)
        {
            return DerefString(process, ReadStringType.AutoDetect, sb);
        }

        public bool DerefString(Process process, ReadStringType type, StringBuilder sb)
        {
            IntPtr ptr;
            if (!DerefOffsets(process, out ptr)
                || !process.ReadString(ptr, type, sb))
            {
                return false;
            }
            return true;
        }

        public bool DerefOffsets(Process process, out IntPtr ptr)
        {
            bool is64Bit = process.Is64Bit();

            if (!string.IsNullOrEmpty(_module))
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
            else if (_usingAbsoluteBase)
            {
                ptr = _absoluteBase;
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

            ptr = ptr + _offsets[_offsets.Count - 1];
            return true;
        }

        private void InitializeOffsets(params OffsetT[] offsets)
        {
            _offsets = new List<OffsetT>();
            _offsets.Add(0); // deref base first
            _offsets.AddRange(offsets);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3f
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int IX { get { return (int)X; } }
        public int IY { get { return (int)Y; } }
        public int IZ { get { return (int)Z; } }

        public Vector3f(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Distance(Vector3f other)
        {
            float result = (X - other.X) * (X - other.X) +
                (Y - other.Y) * (Y - other.Y) +
                (Z - other.Z) * (Z - other.Z);
            return (float)Math.Sqrt(result);
        }

        public float DistanceXY(Vector3f other)
        {
            float result = (X - other.X) * (X - other.X) +
                (Y - other.Y) * (Y - other.Y);
            return (float)Math.Sqrt(result);
        }

        public bool BitEquals(Vector3f other)
        {
            return X.BitEquals(other.X)
                   && Y.BitEquals(other.Y)
                   && Z.BitEquals(other.Z);
        }

        public bool BitEqualsXY(Vector3f other)
        {
            return X.BitEquals(other.X)
                   && Y.BitEquals(other.Y);
        }

        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }
    }
}
