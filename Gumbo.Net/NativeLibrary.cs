#define CORECLR
using System;
using System.Runtime.InteropServices;

namespace Gumbo
{
    public static class NativeLibrary
    {
        public static string LibraryOverride { get; set; }

        public static INativeLibrary Create(string name)
        {
#if CORECLR
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return new WindowsNativeLibrary(name);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return new LinuxNativeLibrary(name);
            else throw new NotImplementedException("Unmanaged library loading is not implemented on this platform");
#else
            return new WindowsNativeLibrary(name);
#endif
        }
    }

    public interface INativeLibrary : IDisposable
    {
        T MarshalStructure<T>(string name);
    }

    internal sealed class WindowsNativeLibrary : INativeLibrary
    {
        readonly IntPtr _LibarayHandle;
        bool _IsDisposed = false;

        public WindowsNativeLibrary(string name)
        {
            var library = NativeLibrary.LibraryOverride ?? (IntPtr.Size == 8 ? $"x64\\{name}" : $"x86\\{name}");
            _LibarayHandle = LoadLibrary(library);
            if (_LibarayHandle == IntPtr.Zero)
                throw new InvalidOperationException($"library {name} not found");
        }

        public void Dispose()
        {
            if (_IsDisposed)
                return;
            FreeLibrary(_LibarayHandle);
            _IsDisposed = true;
        }

        public T MarshalStructure<T>(string name)
        {
            if (_IsDisposed)
                throw new ObjectDisposedException("UnmanagedLibrary");
            var ptr = GetProcAddress(_LibarayHandle, name);
            return ptr != IntPtr.Zero ? Marshal.PtrToStructure<T>(ptr) : throw new InvalidOperationException($"function {name} not found");
        }

        ~WindowsNativeLibrary() { Dispose(); }

        [DllImport("kernel32.dll")] static extern IntPtr LoadLibrary(string fileName);
        [DllImport("kernel32.dll")] static extern int FreeLibrary(IntPtr handle);
        [DllImport("kernel32.dll")] static extern IntPtr GetProcAddress(IntPtr handle, string procedureName);
    }

    internal sealed class LinuxNativeLibrary : INativeLibrary
    {
        readonly IntPtr _LibarayHandle;
        const int RTLD_NOW = 2;
        bool _IsDisposed = false;

        public LinuxNativeLibrary(string name)
        {
            var library = NativeLibrary.LibraryOverride ?? (IntPtr.Size == 8 ? $"x64\\{name}" : $"x86\\{name}");
            _LibarayHandle = dlopen(library, RTLD_NOW);
            if (_LibarayHandle == IntPtr.Zero)
                throw new InvalidOperationException($"library {name} not found");
        }

        public void Dispose()
        {
            if (_IsDisposed)
                return;
            dlclose(_LibarayHandle);
            _IsDisposed = true;
        }

        public T MarshalStructure<T>(string name)
        {
            if (_IsDisposed)
                throw new ObjectDisposedException("UnmanagedLibrary");
            // clear previous errors if any
            dlerror();
            var ptr = dlsym(_LibarayHandle, name);
            var errPtr = dlerror();
            if (errPtr != IntPtr.Zero)
                throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
            return Marshal.PtrToStructure<T>(ptr);
        }

        ~LinuxNativeLibrary() { Dispose(); }

        [DllImport("libdl.so")] static extern IntPtr dlopen(string fileName, int flags);
        [DllImport("libdl.so")] static extern IntPtr dlsym(IntPtr handle, string symbol);
        [DllImport("libdl.so")] static extern int dlclose(IntPtr handle);
        [DllImport("libdl.so")] static extern IntPtr dlerror();
    }
}
