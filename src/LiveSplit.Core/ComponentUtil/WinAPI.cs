using System;
using System.Runtime.InteropServices;
using System.Text;

using SizeT = nuint;

namespace LiveSplit.ComponentUtil;
public enum MemPageState : uint
{
    MEM_COMMIT = 0x1000,
    MEM_RESERVE = 0x2000,
    MEM_FREE = 0x10000,
}

public enum MemPageType : uint
{
    MEM_PRIVATE = 0x20000,
    MEM_MAPPED = 0x40000,
    MEM_IMAGE = 0x1000000
}

[Flags]
public enum MemPageProtect : uint
{
    PAGE_NOACCESS = 0x01,
    PAGE_READONLY = 0x02,
    PAGE_READWRITE = 0x04,
    PAGE_WRITECOPY = 0x08,
    PAGE_EXECUTE = 0x10,
    PAGE_EXECUTE_READ = 0x20,
    PAGE_EXECUTE_READWRITE = 0x40,
    PAGE_EXECUTE_WRITECOPY = 0x80,
    PAGE_GUARD = 0x100,
    PAGE_NOCACHE = 0x200,
    PAGE_WRITECOMBINE = 0x400,
}

[StructLayout(LayoutKind.Sequential)]
public struct MemoryBasicInformation // MEMORY_BASIC_INFORMATION
{
    public nint BaseAddress;
    public nint AllocationBase;
    public MemPageProtect AllocationProtect;
    public SizeT RegionSize;
    public MemPageState State;
    public MemPageProtect Protect;
    public MemPageType Type;
}

public static class WinAPI
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, [Out] byte[] lpBuffer,
        SizeT nSize, out SizeT lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer,
        SizeT nSize, out SizeT lpNumberOfBytesWritten);

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumProcessModulesEx(nint hProcess, [Out] nint[] lphModule, uint cb,
        out uint lpcbNeeded, uint dwFilterFlag);

    [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetModuleFileNameExW(nint hProcess, nint hModule, [Out] StringBuilder lpBaseName,
        uint nSize);

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetModuleInformation(nint hProcess, nint hModule, [Out] out MODULEINFO lpmodinfo,
        uint cb);

    [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
    public static extern uint GetModuleBaseNameW(nint hProcess, nint hModule, [Out] StringBuilder lpBaseName,
        uint nSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process(nint hProcess,
        [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern SizeT VirtualQueryEx(nint hProcess, nint lpAddress,
        [Out] out MemoryBasicInformation lpBuffer, SizeT dwLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nint VirtualAllocEx(nint hProcess, nint lpAddress, SizeT dwSize, uint flAllocationType,
        MemPageProtect flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool VirtualFreeEx(nint hProcess, nint lpAddress, SizeT dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool VirtualProtectEx(nint hProcess, nint lpAddress, SizeT dwSize,
        MemPageProtect flNewProtect, [Out] out MemPageProtect lpflOldProtect);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern nint NtSuspendProcess(nint hProcess);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern nint NtResumeProcess(nint hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, SizeT dwStackSize,
        nint lpStartAddress, nint lpParameter, uint dwCreationFlags, out nint lpThreadId);

    [StructLayout(LayoutKind.Sequential)]
    public struct MODULEINFO
    {
        public nint lpBaseOfDll;
        public uint SizeOfImage;
        public nint EntryPoint;
    }
}
