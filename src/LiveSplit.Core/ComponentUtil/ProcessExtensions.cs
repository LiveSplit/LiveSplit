using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using SizeT = System.UIntPtr;

// Note: Please be careful when modifying this because it could break existing components!
// http://stackoverflow.com/questions/1456785/a-definitive-guide-to-api-breaking-changes-in-net

namespace LiveSplit.ComponentUtil;
public class ProcessModuleWow64Safe
{
    public IntPtr BaseAddress { get; set; }
    public IntPtr EntryPointAddress { get; set; }
    public string FileName { get; set; }
    public int ModuleMemorySize { get; set; }
    public string ModuleName { get; set; }
    public FileVersionInfo FileVersionInfo => FileVersionInfo.GetVersionInfo(FileName);
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
    private static readonly Dictionary<int, ProcessModuleWow64Safe[]> ModuleCache = [];

    public static ProcessModuleWow64Safe MainModuleWow64Safe(this Process p)
    {
        return p.ModulesWow64Safe().First();
    }

    public static ProcessModuleWow64Safe[] ModulesWow64Safe(this Process p)
    {
        if (ModuleCache.Count > 100)
        {
            ModuleCache.Clear();
        }

        const int LIST_MODULES_ALL = 3;
        const int MAX_PATH = 260;

        if (!WinAPI.EnumProcessModulesEx(p.Handle, null, 0, out uint cbNeeded, LIST_MODULES_ALL))
        {
            throw new Win32Exception();
        }

        uint numMods = cbNeeded / (uint)Unsafe.SizeOf<IntPtr>();

        int hash = p.StartTime.GetHashCode() + p.Id + (int)numMods;
        if (ModuleCache.ContainsKey(hash))
        {
            return ModuleCache[hash];
        }

        IntPtr[] hModules = new IntPtr[(int)numMods];
        if (!WinAPI.EnumProcessModulesEx(p.Handle, hModules, cbNeeded, out _, LIST_MODULES_ALL))
        {
            throw new Win32Exception();
        }

        var ret = new List<ProcessModuleWow64Safe>();

        // everything below is fairly expensive, which is why we cache!
        var sb = new StringBuilder(MAX_PATH);
        for (int i = 0; i < numMods; i++)
        {
            sb.Clear();
            if (WinAPI.GetModuleFileNameExW(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
            {
                throw new Win32Exception();
            }

            string fileName = sb.ToString();

            sb.Clear();
            if (WinAPI.GetModuleBaseNameW(p.Handle, hModules[i], sb, (uint)sb.Capacity) == 0)
            {
                throw new Win32Exception();
            }

            string baseName = sb.ToString();

            var moduleInfo = new WinAPI.MODULEINFO();
            if (!WinAPI.GetModuleInformation(p.Handle, hModules[i], out moduleInfo, (uint)Marshal.SizeOf(moduleInfo)))
            {
                throw new Win32Exception();
            }

            ret.Add(new ProcessModuleWow64Safe()
            {
                FileName = fileName,
                BaseAddress = moduleInfo.lpBaseOfDll,
                ModuleMemorySize = (int)moduleInfo.SizeOfImage,
                EntryPointAddress = moduleInfo.EntryPoint,
                ModuleName = baseName
            });
        }

        ModuleCache[hash] = [.. ret];

        return [.. ret];
    }

    public static IEnumerable<MemoryBasicInformation> MemoryPages(this Process process, bool all = false)
    {
        // hardcoded values because GetSystemInfo / GetNativeSystemInfo can't return info for remote process
        long min = 0x10000L;
        long max = process.Is64Bit() ? 0x00007FFFFFFEFFFFL : 0x7FFEFFFFL;

        UIntPtr mbiSize = (SizeT)Marshal.SizeOf(typeof(MemoryBasicInformation));

        long addr = min;
        do
        {
            if (WinAPI.VirtualQueryEx(process.Handle, (IntPtr)addr, out MemoryBasicInformation mbi, mbiSize) == SizeT.Zero)
            {
                break;
            }

            addr += (long)mbi.RegionSize;

            // don't care about reserved/free pages
            if (mbi.State != MemPageState.MEM_COMMIT)
            {
                continue;
            }

            // probably don't care about guarded pages
            if (!all && (mbi.Protect & MemPageProtect.PAGE_GUARD) != 0)
            {
                continue;
            }

            // probably don't care about image/file maps
            if (!all && mbi.Type != MemPageType.MEM_PRIVATE)
            {
                continue;
            }

            yield return mbi;

        } while (addr < max);
    }

    public static bool Is64Bit(this Process process)
    {
        WinAPI.IsWow64Process(process.Handle, out bool procWow64);
        if (Environment.Is64BitOperatingSystem && !procWow64)
        {
            return true;
        }

        return false;
    }

    public static bool ReadValue<T>(this Process process, IntPtr addr, out T val) where T : struct
    {
        Type type = typeof(T);
        type = type.IsEnum ? Enum.GetUnderlyingType(type) : type;

        val = default;
        if (!ReadValue(process, addr, type, out object val2))
        {
            return false;
        }

        val = (T)val2;

        return true;
    }

    public static bool ReadValue(Process process, IntPtr addr, Type type, out object val)
    {

        val = null;
        int size = type == typeof(bool) ? 1 : Marshal.SizeOf(type);
        if (!ReadBytes(process, addr, size, out byte[] bytes))
        {
            return false;
        }

        val = ResolveToType(bytes, type);

        return true;
    }

    public static bool ReadBytes(this Process process, IntPtr addr, int count, out byte[] val)
    {
        byte[] bytes = new byte[count];

        val = null;
        if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT read)
            || read != (SizeT)bytes.Length)
        {
            return false;
        }

        val = bytes;

        return true;
    }

    public static bool ReadPointer(this Process process, IntPtr addr, out IntPtr val)
    {
        return ReadPointer(process, addr, process.Is64Bit(), out val);
    }

    public static bool ReadPointer(this Process process, IntPtr addr, bool is64Bit, out IntPtr val)
    {
        byte[] bytes = new byte[is64Bit ? 8 : 4];

        val = IntPtr.Zero;
        if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT read)
            || read != (SizeT)bytes.Length)
        {
            return false;
        }

        val = is64Bit ? (IntPtr)BitConverter.ToInt64(bytes, 0) : (IntPtr)BitConverter.ToUInt32(bytes, 0);

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
        byte[] bytes = new byte[sb.Capacity];
        if (!WinAPI.ReadProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT read)
            || read != (SizeT)bytes.Length)
        {
            return false;
        }

        if (type == ReadStringType.AutoDetect)
        {
            if ((ulong)read >= 2 && bytes[1] == '\x0')
            {
                sb.Append(Encoding.Unicode.GetString(bytes));
            }
            else
            {
                sb.Append(Encoding.UTF8.GetString(bytes));
            }
        }
        else if (type == ReadStringType.UTF8)
        {
            sb.Append(Encoding.UTF8.GetString(bytes));
        }
        else if (type == ReadStringType.UTF16)
        {
            sb.Append(Encoding.Unicode.GetString(bytes));
        }
        else
        {
            sb.Append(Encoding.ASCII.GetString(bytes));
        }

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

    public static T ReadValue<T>(this Process process, IntPtr addr, T default_ = default) where T : struct
    {
        if (!process.ReadValue(addr, out T val))
        {
            val = default_;
        }

        return val;
    }

    public static byte[] ReadBytes(this Process process, IntPtr addr, int count)
    {
        if (!process.ReadBytes(addr, count, out byte[] bytes))
        {
            return null;
        }

        return bytes;
    }

    public static IntPtr ReadPointer(this Process process, IntPtr addr, IntPtr default_ = default)
    {
        if (!process.ReadPointer(addr, out IntPtr ptr))
        {
            return default_;
        }

        return ptr;
    }

    public static string ReadString(this Process process, IntPtr addr, int numBytes, string default_ = null)
    {
        if (!process.ReadString(addr, numBytes, out string str))
        {
            return default_;
        }

        return str;
    }

    public static string ReadString(this Process process, IntPtr addr, ReadStringType type, int numBytes, string default_ = null)
    {
        if (!process.ReadString(addr, type, numBytes, out string str))
        {
            return default_;
        }

        return str;
    }

    public static bool WriteValue<T>(this Process process, IntPtr addr, T obj) where T : struct
    {
        int size = Marshal.SizeOf(obj);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return process.WriteBytes(addr, arr);
    }

    public static bool WriteBytes(this Process process, IntPtr addr, byte[] bytes)
    {
        if (!WinAPI.WriteProcessMemory(process.Handle, addr, bytes, (SizeT)bytes.Length, out SizeT written)
            || written != (SizeT)bytes.Length)
        {
            return false;
        }

        return true;
    }

    private static bool WriteJumpOrCall(Process process, IntPtr addr, IntPtr dest, bool call)
    {
        bool x64 = process.Is64Bit();

        int jmpLen = x64 ? 12 : 5;

        var instruction = new List<byte>(jmpLen);
        if (x64)
        {
            instruction.AddRange(new byte[] { 0x48, 0xB8 }); // mov rax immediate
            instruction.AddRange(BitConverter.GetBytes((long)dest));
            instruction.AddRange(new byte[] { 0xFF, call ? (byte)0xD0 : (byte)0xE0 }); // jmp/call rax
        }
        else
        {
            int offset = unchecked((int)dest - (int)(addr + jmpLen));
            instruction.AddRange(new byte[] { call ? (byte)0xE8 : (byte)0xE9 }); // jmp/call immediate
            instruction.AddRange(BitConverter.GetBytes(offset));
        }

        process.VirtualProtect(addr, jmpLen, MemPageProtect.PAGE_EXECUTE_READWRITE, out MemPageProtect oldProtect);
        bool success = process.WriteBytes(addr, [.. instruction]);
        process.VirtualProtect(addr, jmpLen, oldProtect);

        return success;
    }

    public static bool WriteJumpInstruction(this Process process, IntPtr addr, IntPtr dest)
    {
        return WriteJumpOrCall(process, addr, dest, false);
    }

    public static bool WriteCallInstruction(this Process process, IntPtr addr, IntPtr dest)
    {
        return WriteJumpOrCall(process, addr, dest, true);
    }

    public static IntPtr WriteDetour(this Process process, IntPtr src, int overwrittenBytes, IntPtr dest)
    {
        int jmpLen = process.Is64Bit() ? 12 : 5;
        if (overwrittenBytes < jmpLen)
        {
            throw new ArgumentOutOfRangeException(nameof(overwrittenBytes),
                $"must be >= length of jmp instruction ({jmpLen})");
        }

        // allocate memory to store the original src prologue bytes we overwrite with jump to dest
        // along with the jump back to src
        IntPtr gate;
        if ((gate = process.AllocateMemory(jmpLen + overwrittenBytes)) == IntPtr.Zero)
        {
            throw new Win32Exception();
        }

        try
        {
            // read the original bytes from the prologue of src
            byte[] origSrcBytes = process.ReadBytes(src, overwrittenBytes);
            if (origSrcBytes == null)
            {
                throw new Win32Exception();
            }

            // write the original prologue of src into the start of gate
            if (!process.WriteBytes(gate, origSrcBytes))
            {
                throw new Win32Exception();
            }

            // write the jump from the end of the gate back to src
            if (!process.WriteJumpInstruction(gate + overwrittenBytes, src + overwrittenBytes))
            {
                throw new Win32Exception();
            }

            // finally write the jump from src to dest
            if (!process.WriteJumpInstruction(src, dest))
            {
                throw new Win32Exception();
            }

            // nop the leftover bytes in the src prologue
            int extraBytes = overwrittenBytes - jmpLen;
            if (extraBytes > 0)
            {
                byte[] nops = Enumerable.Repeat((byte)0x90, extraBytes).ToArray();
                if (!process.VirtualProtect(src + jmpLen, nops.Length, MemPageProtect.PAGE_EXECUTE_READWRITE,
                    out MemPageProtect oldProtect))
                {
                    throw new Win32Exception();
                }

                if (!process.WriteBytes(src + jmpLen, nops))
                {
                    throw new Win32Exception();
                }

                process.VirtualProtect(src + jmpLen, nops.Length, oldProtect);
            }
        }
        catch
        {
            process.FreeMemory(gate);
            throw;
        }

        return gate;
    }

    private static object ResolveToType(byte[] bytes, Type type)
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
            {
                val = false;
            }
            else
            {
                val = bytes[0] != 0;
            }
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

    public static IntPtr AllocateMemory(this Process process, int size)
    {
        return WinAPI.VirtualAllocEx(process.Handle, IntPtr.Zero, (SizeT)size, (uint)MemPageState.MEM_COMMIT,
            MemPageProtect.PAGE_EXECUTE_READWRITE);
    }

    public static bool FreeMemory(this Process process, IntPtr addr)
    {
        const uint MEM_RELEASE = 0x8000;
        return WinAPI.VirtualFreeEx(process.Handle, addr, SizeT.Zero, MEM_RELEASE);
    }

    public static bool VirtualProtect(this Process process, IntPtr addr, int size, MemPageProtect protect,
        out MemPageProtect oldProtect)
    {
        return WinAPI.VirtualProtectEx(process.Handle, addr, (SizeT)size, protect, out oldProtect);
    }

    public static bool VirtualProtect(this Process process, IntPtr addr, int size, MemPageProtect protect)
    {
        return WinAPI.VirtualProtectEx(process.Handle, addr, (SizeT)size, protect, out MemPageProtect oldProtect);
    }

    public static IntPtr CreateThread(this Process process, IntPtr startAddress, IntPtr parameter)
    {
        return WinAPI.CreateRemoteThread(process.Handle, IntPtr.Zero, SizeT.Zero, startAddress, parameter, 0,
            out IntPtr threadId);
    }

    public static IntPtr CreateThread(this Process process, IntPtr startAddress)
    {
        return CreateThread(process, startAddress, IntPtr.Zero);
    }

    public static void Suspend(this Process process)
    {
        WinAPI.NtSuspendProcess(process.Handle);
    }

    public static void Resume(this Process process)
    {
        WinAPI.NtResumeProcess(process.Handle);
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
