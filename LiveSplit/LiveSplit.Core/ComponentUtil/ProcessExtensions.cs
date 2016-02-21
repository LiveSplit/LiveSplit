using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
#pragma warning disable 1591

// Note: Please be careful when modifying this because it could break existing components!

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
            get { return FileVersionInfo.GetVersionInfo(FileName); }
        }
        public override string ToString()
        {
            return ModuleName ?? base.ToString();
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
        private static Dictionary<int, ProcessModuleWow64Safe[]> ModuleCache = new Dictionary<int, ProcessModuleWow64Safe[]>();

        public static ProcessModuleWow64Safe MainModuleWow64Safe(this Process p)
        {
            return p.ModulesWow64Safe().First();
        }

        public static ProcessModuleWow64Safe[] ModulesWow64Safe(this Process p)
        {
            if (ModuleCache.Count > 100)
                ModuleCache.Clear();

            const int LIST_MODULES_ALL = 3;
            const int MAX_PATH = 260;

            var hModules = new IntPtr[1024];

            uint cb = (uint)IntPtr.Size*(uint)hModules.Length;
            uint cbNeeded;

            if (!WinAPI.EnumProcessModulesEx(p.Handle, hModules, cb, out cbNeeded, LIST_MODULES_ALL))
                throw new Win32Exception();
            uint numMods = cbNeeded / (uint)IntPtr.Size;

            int hash = p.StartTime.GetHashCode() + p.Id + (int)numMods;
            if (ModuleCache.ContainsKey(hash))
                return ModuleCache[hash];

            var ret = new List<ProcessModuleWow64Safe>();

            // everything below is fairly expensive, which is why we cache!
            var sb = new StringBuilder(MAX_PATH);
            for (int i = 0; i < numMods; i++)
            {
                sb.Clear();
                if (WinAPI.GetModuleFileNameEx(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
                    throw new Win32Exception();
                string fileName = sb.ToString();

                sb.Clear();
                if (WinAPI.GetModuleBaseName(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
                    throw new Win32Exception();
                string baseName = sb.ToString();

                var moduleInfo = new WinAPI.MODULEINFO();
                if (!WinAPI.GetModuleInformation(p.Handle, hModules[i], out moduleInfo, (uint)Marshal.SizeOf(moduleInfo)))
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

        public static IEnumerable<MemoryBasicInformation> MemoryPages(this Process process, bool all = false)
        {
            var ret = new List<MemoryBasicInformation>();

            // hardcoded values because GetSystemInfo / GetNativeSystemInfo can't return info for remote process
            var min = 0x10000L;
            var max = process.Is64Bit() ? 0x00007FFFFFFEFFFFL : 0x7FFEFFFFL;

            var mbiSize = (SizeT)Marshal.SizeOf(typeof(MemoryBasicInformation));

            var addr = min;
            do
            {
                MemoryBasicInformation mbi;
                WinAPI.VirtualQueryEx(process.Handle, (IntPtr)addr, out mbi, mbiSize);
                addr += (long)mbi.RegionSize;

                // don't care about reserved/free pages
                if (mbi.State != MemPageState.MEM_COMMIT)
                    continue;

                // probably don't care about guarded pages
                if (!all && (mbi.Protect & MemPageProtection.PAGE_GUARD) != 0)
                    continue;

                // probably don't care about image/file maps
                if (!all && mbi.Type != MemPageType.MEM_PRIVATE)
                    continue;

                ret.Add(mbi);

            } while (addr < max);

            return ret;
        }

        public static bool Is64Bit(this Process process)
        {
            bool procWow64;
            WinAPI.IsWow64Process(process.Handle, out procWow64);
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
            if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out read)
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
            if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out read)
                || read != (SizeT)bytes.Length)
                return false;

            val = is64Bit ? (IntPtr)BitConverter.ToInt64(bytes, 0) : (IntPtr)BitConverter.ToInt32(bytes, 0);

            return true;
        }

        public static bool ReadString(this Process process, IntPtr addr, int numBytes, out string str)
        {
            return ReadString(process, addr, ReadStringType.AutoDetect, numBytes, out str);
        }

        public static bool ReadString(this Process process, IntPtr addr, ReadStringType type, int numBytes, out string str)
        {
            var sb = new StringBuilder(numBytes);
            if (!ReadString(process, addr, type, sb))
            {
                str = string.Empty;
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
            if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out read)
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

        public static T ReadValue<T>(this Process process, IntPtr addr, T default_ = default(T)) where T : struct
        {
            T val;
            if (!process.ReadValue(addr, out val))
                val = default_;
            return val;
        }

        public static byte[] ReadBytes(this Process process, IntPtr addr, int count)
        {
            byte[] bytes;
            if (!process.ReadBytes(addr, count, out bytes))
                return null;
            return bytes;
        }

        public static IntPtr ReadPointer(this Process process, IntPtr addr, IntPtr default_ = default(IntPtr))
        {
            IntPtr ptr;
            if (!process.ReadPointer(addr, out ptr))
                return default_;
            return ptr;
        }

        public static string ReadString(this Process process, IntPtr addr, int numBytes, string default_ = null)
        {
            string str;
            if (!process.ReadString(addr, numBytes, out str))
                return default_;
            return str;
        }

        public static string ReadString(this Process process, IntPtr addr, ReadStringType type, int numBytes, string default_ = null)
        {
            string str;
            if (!process.ReadString(addr, type, numBytes, out str))
                return default_;
            return str;
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

        public static float ToFloatBits(this uint i)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
        }

        public static uint ToUInt32Bits(this float f)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(f), 0);
        }

        public static bool BitEquals(this float f, float o)
        {
            return ToUInt32Bits(f) == ToUInt32Bits(o);
        }
    }
}
