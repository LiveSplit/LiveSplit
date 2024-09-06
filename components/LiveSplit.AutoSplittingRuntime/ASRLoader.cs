using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LiveSplit.AutoSplittingRuntime;

public class ASRLoader
{
    [DllImport("kernel32")]
    private unsafe static extern void* LoadLibrary(string dllname);

    [DllImport("kernel32")]
    private unsafe static extern void FreeLibrary(void* handle);

    private sealed unsafe class LibraryUnloader
    {
        internal LibraryUnloader(void* handle)
        {
            this.handle = handle;
        }

        ~LibraryUnloader()
        {
            if (handle != null)
            {
                FreeLibrary(handle);
            }
        }

        private readonly void* handle;

    }

    private static LibraryUnloader unloader;

    public static void LoadASR()
    {
        if (unloader != null)
        {
            return;
        }

        string path;

        if (Unsafe.SizeOf<nint>() == 8)
        {
            path = @"Components\x64\asr_capi.dll";
        }
        else
        {
            path = @"Components\x86\asr_capi.dll";
        }

        unsafe
        {
            void* handle = LoadLibrary(path);

            if (handle == null)
            {
                throw new DllNotFoundException("Unable to load the native ASR library: " + path);
            }

            unloader = new LibraryUnloader(handle);
        }
    }
}
