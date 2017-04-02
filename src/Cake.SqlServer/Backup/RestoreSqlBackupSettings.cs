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

        /// <summary>
        /// Sets the flag to execute restore command `WITH REPLACE` suffix. Allows you to write over an existing database when 
        /// doing a restore without first backing up the tail of the transaction log.  
        /// The WITH REPLACE basically tells SQL Server to just throw out any active contents 
        /// in the transaction log and move forward with the restore.
        /// </summary>
        public bool WithReplace { get; set; }
    }
}