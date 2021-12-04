using System;

namespace Cake.SqlServer
{
    /// <summary>
    /// File Spec object to describe file name and path for creating databases.
    /// </summary>
    public sealed class CreateDatabaseFileSpec
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabaseFileSpec"/> class.
        /// </summary>
        /// <param name="fileName">Path to file.</param>
        public CreateDatabaseFileSpec(string fileName)
        {
            // need to replace fowards slashes to backward because SQL Server does not like them.
            FileName = fileName.Replace("/", "\\", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets path to file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets or sets name of file - internal name of file in SQL Server.
        /// Defaulted to database name for mdf and {DatabaseName}_log for log files.
        /// </summary>
        public string? Name { get; set; }
    }
}
