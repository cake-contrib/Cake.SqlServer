#if NET461
using System.Runtime.InteropServices;

namespace Cake.SqlServer
{

    internal static class NativeMethods
    {
        internal const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetDefaultDllDirectories(uint directoryFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int AddDllDirectory(string newDirectory);
    }
}
#endif
