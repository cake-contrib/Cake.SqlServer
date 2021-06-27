#if NET461
using System;
using System.IO;
using System.Reflection;
#endif

namespace Cake.SqlServer
{
    internal static class Initializer
    {
#if NET461
        private static bool _initialized;
#endif

        internal static void InitializeNativeSearchPath()
        {
#if NET461
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (_initialized)
                {
                    return;
                }

                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                var addinFolder = Path.GetDirectoryName(path);
                NativeMethods.SetDefaultDllDirectories(NativeMethods.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                NativeMethods.AddDllDirectory(addinFolder);

                _initialized = true;
            }
#else
            // initialization not necessary
#endif
        }
    }
}
