using System.Runtime.InteropServices;

namespace Cake.SqlServer
{
    public static class NativeMethods
    {
        public const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;
	
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultDllDirectories(uint directoryFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int AddDllDirectory(string newDirectory);
    }
}
