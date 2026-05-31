using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LiveSplit;

public class LiveSplitCoreFactory
{
    [DllImport("kernel32")]
    private static extern unsafe void* LoadLibrary(string dllname);

    [DllImport("kernel32")]
    private static extern unsafe void FreeLibrary(void* handle);

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
    public static void LoadLiveSplitCore()
    {
        string path = Unsafe.SizeOf<IntPtr>() == 4
            ? @"x86\livesplit_core.dll"
            : @"x64\livesplit_core.dll";
        unsafe
        {
            void* handle = LoadLibrary(path);

            if (handle == null)
            {
                throw new DllNotFoundException("Unable to load the native livesplit-core library: " + path);
            }
        }
    }
}
