#if NET5_0
using System;
#endif

namespace Cake.SqlServer
{
    /// <summary>
    /// File Spec object to describe file name and path for creating databases
    /// </summary>
    public sealed class CreateDatabaseFileSpec
    {
        /// <summary>
        /// Constructor for filespec.
        /// </summary>
        /// <param name="fileName">Path to file</param>
        public CreateDatabaseFileSpec(string fileName)
        {
            // need to replace fowards slashes to backward because SQL Server does not like them.
#if NET5_0
            FileName = fileName.Replace("/", "\\", StringComparison.OrdinalIgnoreCase);
#else
            FileName = fileName.Replace("/", "\\");
#endif
        }

        /// <summary>
        /// Path to file
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Name of file - internal name of file in SQL Server.
        /// Defaulted to database name for mdf and {DatabaseName}_log for log files
        /// </summary>
        public string? Name { get; set; }
    }
}
