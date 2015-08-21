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
    // both ASL and DLL autosplitters should use this as a replacement for Process.Modules
    // it's required for 64<->32 when accessing a module other than mainmodule

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

    public static class ExtensionMethods
    {
        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, [Out] IntPtr[] lphModule, uint cb, out uint lpcbNeeded, uint dwFilterFlag);

        [DllImport("psapi.dll", SetLastError = true)]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, [Out] out MODULEINFO lpmodinfo, uint cb);

        [DllImport("psapi.dll")]
        static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

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
    }
}
