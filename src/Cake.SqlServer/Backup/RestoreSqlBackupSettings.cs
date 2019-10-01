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
        /// Default constructor for settings object. Sets SwitchToSingleUserMode to true.
        /// </summary>
        public RestoreSqlBackupSettings()
        {
            SwitchToSingleUserMode = true;
            BackupSetFile = 1;
            DifferentialBackupSetFile = 1;
        }

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

        /// <summary>
        /// Before restoring backup, database will be switched to a single user mode. 
        /// Default operation is to go into single user mode. However in some situation this might not work.
        /// Use this switch to bypass single user mode and restore the DB as it is
        /// </summary>
        public bool SwitchToSingleUserMode { get; set; }

        /// <summary>
        /// Specifies which BackupSet should be used when restoring the main backup files.
        /// </summary>
        public int BackupSetFile { get; set; }

        /// <summary>
        /// Specifies which BackupSet should be used when restoring the differential backup files.
        /// </summary>
        public int DifferentialBackupSetFile { get; set; }

    }
}
