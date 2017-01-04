using System;
using Cake.Core.IO;

namespace Cake.SqlServer
{
    /// <summary>
    /// Settings for restoring database from a backup file
    /// </summary>
    public class RestoreSqlBackupSettings
    {
        /// <summary>
        /// Gets or sets the new name of the database.
        /// Name of the database where to restore. If this is not specified, database name is taken from the backup file
        /// </summary>
        /// <value>
        /// The new name of the database.
        /// </value>
        public String NewDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the new storage folder.
        /// Path where data and log files should be stored.If this is not specified, server defaults will be used
        /// </summary>
        /// <value>
        /// The new storage folder.
        /// </value>
        public DirectoryPath NewStorageFolder { get; set; }

        //TODO add with replace
    }
}