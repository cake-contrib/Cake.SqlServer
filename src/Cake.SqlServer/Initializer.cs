using System;
using System.IO;
using System.Reflection;

namespace Cake.SqlServer
{
    internal static class Initializer
    {
        private static bool _initialized;

        internal static void InitializeNativeSearchPath()
        {
#if NET461
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
#endif
            {
                if (_initialized)
                {
                    return;
                }

#if NET461
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                var addinFolder = Path.GetDirectoryName(path);
                NativeMethods.SetDefaultDllDirectories(NativeMethods.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                NativeMethods.AddDllDirectory(addinFolder);
#endif
                _initialized = true;
            }
        }
    }
}
