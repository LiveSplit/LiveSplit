using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LiveSplit;

public class LiveSplitCoreFactory
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
    public static void LoadLiveSplitCore()
    {
        string path;

        if (Unsafe.SizeOf<IntPtr>() == 4)
        {
            path = "x86\\livesplit_core.dll";
        }
        else
        {
            path = "x64\\livesplit_core.dll";
        }

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
