#if NET461
using System;
#else
using System.Runtime.InteropServices;
#endif
using Cake.Core;

namespace Cake.SqlServer
{
    internal static class Initializer
    {
        private static bool _initialized;

        internal static void InitializeNativeSearchPath(ICakeContext context)
        {
#if NET461
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
            {
                if (_initialized)
                {
                    return;
                }

                var res = context.Tools.Resolve("Microsoft.Data.SqlClient.SNI.x64.dll").GetDirectory().FullPath;
                NativeMethods.SetDefaultDllDirectories(NativeMethods.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                NativeMethods.AddDllDirectory(res);

                _initialized = true;
            }
        }
    }
}
