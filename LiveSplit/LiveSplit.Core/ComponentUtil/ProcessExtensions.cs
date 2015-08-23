using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
#pragma warning disable 1591

namespace LiveSplit.ComponentUtil
{
    using SizeT = UIntPtr;

    public class ProcessModuleWow64Safe
    {
        public IntPtr BaseAddress { get; set; }
        public IntPtr EntryPointAddress { get; set; }
        public string FileName { get; set; }
        public int ModuleMemorySize { get; set; }
        public string ModuleName { get; set; }
        public FileVersionInfo FileVersionInfo
        {
            get { return FileVersionInfo.GetVersionInfo(this.FileName); }
        }
    }

    public enum ReadStringType
    {
        AutoDetect,
        ASCII,
        UTF8,
        UTF16
    }

    public static class ExtensionMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            SizeT dwSize, out SizeT lpNumberOfBytesRead);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, [Out] IntPtr[] lphModule, uint cb,
            out uint lpcbNeeded, uint dwFilterFlag);

        [DllImport("psapi.dll", SetLastError = true)]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
            uint nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, [Out] out MODULEINFO lpmodinfo,
            uint cb);

        [DllImport("psapi.dll")]
        private static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
            uint nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr hProcess, [Out] out bool wow64Process);

        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEINFO
        {
             public IntPtr lpBaseOfDll;
             public uint SizeOfImage;
             public IntPtr EntryPoint;
        }

        // 🔔🔔🔔 ＳＨＡＭＥ 🔔🔔🔔
        // HACK HACK HACK - perf
        private static Dictionary<int, ProcessModuleWow64Safe[]> ModuleCache = new Dictionary<int, ProcessModuleWow64Safe[]>();

        public static ProcessModuleWow64Safe MainModuleWow64Safe(this Process p)
        {
            return p.ModulesWow64Safe().First();
        }

        public static ProcessModuleWow64Safe[] ModulesWow64Safe(this Process p)
        {
            int hash = p.StartTime.GetHashCode() + p.Id;
            if (ModuleCache.ContainsKey(hash))
                return ModuleCache[hash];

            const int LIST_MODULES_ALL = 3;
            const int MAX_PATH = 260;

            var hModules = new IntPtr[1024];

            uint cb = (uint)IntPtr.Size*(uint)hModules.Length;
            uint cbNeeded;

            if (!EnumProcessModulesEx(p.Handle, hModules, cb, out cbNeeded, LIST_MODULES_ALL))
                throw new Win32Exception();

            var ret = new List<ProcessModuleWow64Safe>();

            uint numMods = cb/(uint)IntPtr.Size;
            var sb = new StringBuilder(MAX_PATH);
            for (int i = 0; i < numMods; i++)
            {
                sb.Clear();
                if (GetModuleFileNameEx(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
                    throw new Win32Exception();
                string fileName = sb.ToString();

                sb.Clear();
                if (GetModuleBaseName(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
                    throw new Win32Exception();
                string baseName = sb.ToString();

                var moduleInfo = new MODULEINFO();
                if (!GetModuleInformation(p.Handle, hModules[i], out moduleInfo, (uint)Marshal.SizeOf(moduleInfo)))
                    throw new Win32Exception();

                ret.Add(new ProcessModuleWow64Safe()
                {
                    FileName = fileName,
                    BaseAddress = moduleInfo.lpBaseOfDll,
                    ModuleMemorySize = (int)moduleInfo.SizeOfImage,
                    EntryPointAddress = moduleInfo.EntryPoint,
                    ModuleName = baseName
                });
            }

            ModuleCache.Add(hash, ret.ToArray());

            return ret.ToArray();
        }

        public static bool Is64Bit(this Process process)
        {
            bool procWow64;
            IsWow64Process(process.Handle, out procWow64);
            if (Environment.Is64BitOperatingSystem && !procWow64)
                return true;
            return false;
        }

        public static bool ReadValue<T>(this Process process, IntPtr addr, out T val) where T : struct
        {
            var type = typeof(T);
            type = type.IsEnum ? Enum.GetUnderlyingType(type) : type;

            val = default(T);
            object val2;
            if (!ReadValue(process, addr, type, out val2))
                return false;

            val = (T)val2;

            return true;
        }

        public static bool ReadValue(Process process, IntPtr addr, Type type, out object val)
        {
            byte[] bytes;

            val = null;
            int size = type == typeof(bool) ? 1 : Marshal.SizeOf(type);
            if (!ReadBytes(process, addr, size, out bytes))
                return false;

            val = ResolveToType(bytes, type);

            return true;
        }

        public static bool ReadBytes(this Process process, IntPtr addr, int count, out byte[] val)
        {
            var bytes = new byte[count];

            SizeT read;
            val = null;
            if (!ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out read)
                || read != (SizeT)bytes.Length)
                return false;

            val = bytes;

            return true;
        }

        public static bool ReadPointer(this Process process, IntPtr addr, out IntPtr val)
        {
            return ReadPointer(process, addr, process.Is64Bit(), out val);
        }

        public static bool ReadPointer(this Process process, IntPtr addr, bool is64Bit, out IntPtr val)
        {
            var bytes = new byte[is64Bit ? 8 : 4];

            SizeT read;
            val = IntPtr.Zero;
            if (!ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out read)
                || read != (SizeT)bytes.Length)
                return false;

            val = is64Bit ? (IntPtr)BitConverter.ToInt64(bytes, 0) : (IntPtr)BitConverter.ToInt32(bytes, 0);

            return true;
        }

        public static bool ReadString(this Process process, IntPtr addr, int len, out string str)
        {
            return ReadString(process, addr, ReadStringType.AutoDetect, len, out str);
        }

        public static bool ReadString(this Process process, IntPtr addr, ReadStringType type, int len, out string str)
        {
            var sb = new StringBuilder(len);
            if (!ReadString(process, addr, type, sb))
            {
                str = String.Empty;
                return false;
            }

            str = sb.ToString();

            return true;
        }

        public static bool ReadString(this Process process, IntPtr addr, StringBuilder sb)
        {
            return ReadString(process, addr, ReadStringType.AutoDetect, sb);
        }

        public static bool ReadString(this Process process, IntPtr addr, ReadStringType type, StringBuilder sb)
        {
            var bytes = new byte[sb.Capacity];
            SizeT read;
            if (!ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out read)
                || read != (SizeT)bytes.Length)
                return false;

            if (type == ReadStringType.AutoDetect)
            {
                if (read.ToUInt64() >= 2 && bytes[1] == '\x0')
                    sb.Append(Encoding.Unicode.GetString(bytes));
                else
                    sb.Append(Encoding.UTF8.GetString(bytes));
            }
            else if (type == ReadStringType.UTF8)
                sb.Append(Encoding.UTF8.GetString(bytes));
            else if (type == ReadStringType.UTF16)
                sb.Append(Encoding.Unicode.GetString(bytes));
            else
                sb.Append(Encoding.ASCII.GetString(bytes));

            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '\0')
                {
                    sb.Remove(i, sb.Length - i);
                    break;
                }
            }

            return true;
        }

        static object ResolveToType(byte[] bytes, Type type)
        {
            object val;

            if (type == typeof(int))
            {
                val = BitConverter.ToInt32(bytes, 0);
            }
            else if (type == typeof(uint))
            {
                val = BitConverter.ToUInt32(bytes, 0);
            }
            else if (type == typeof(float))
            {
                val = BitConverter.ToSingle(bytes, 0);
            }
            else if (type == typeof(double))
            {
                val = BitConverter.ToDouble(bytes, 0);
            }
            else if (type == typeof(byte))
            {
                val = bytes[0];
            }
            else if (type == typeof(bool))
            {
                if (bytes == null)
                    val = false;
                else
                    val = (bytes[0] != 0);
            }
            else if (type == typeof(short))
            {
                val = BitConverter.ToInt16(bytes, 0);
            }
            else // probably a struct
            {
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                try
                {
                    val = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
                }
                finally
                {
                    handle.Free();
                }
            }

            return val;
        }

        public static unsafe float ToFloatBits(this uint i)
        {
            return *((float*)&i);
        }

        public static unsafe uint ToUInt32Bits(this float f)
        {
            return *((uint*)&f);
        }

        public static bool BitEquals(this float f, float o)
        {
            return ToUInt32Bits(f) == ToUInt32Bits(o);
        }
    }
}
