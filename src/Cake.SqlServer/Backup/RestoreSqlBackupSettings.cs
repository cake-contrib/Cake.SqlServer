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

        /// <summary>
        /// Before restoring backup, database will be switched to a single user mode. 
        /// Default operation is to go into single user mode. However in some situation this might not work.
        /// Use this switch to bypass single user mode and restore the DB as it is
        /// 
        /// This property has been obsoleted by SwitchToUserMode which allows for a third option
        /// </summary>
        [Obsolete("Use SwitchToUserMode instead")]
        public bool SwitchToSingleUserMode
        {
            get => SwitchToUserMode == DbUserMode.SingleUser;
            set => SwitchToUserMode = value ? DbUserMode.SingleUser : DbUserMode.MultiUser;
        }

        /// <summary>
        /// Before restoring backup, database will be switched to the user mode selected
        /// Default operation is to go into single user mode. However in some situation this might not work.
        /// Use this switch to bypass single user mode and restore the DB as it is, or your can use restricted mode instead.
        /// </summary>
        public DbUserMode SwitchToUserMode { get; set; } = DbUserMode.SingleUser;

        /// <summary>
        /// If set this specifies which BackupSet should be used when restoring the main backup files.
        /// </summary>
        public int? BackupSetFile { get; set; }

        /// <summary>
        /// If set this specifies which BackupSet should be used when restoring the differential backup files.
        /// </summary>
        public int? DifferentialBackupSetFile { get; set; }
    }

    /// <summary>
    /// This enum specifies what user mode the database should be in
    /// </summary>
    public enum DbUserMode
    {
        /// <summary>
        /// This is the default database user access mode. In this database user access mode any user who have permission to access the database can access the database.
        /// </summary>
        MultiUser,
        /// <summary>
        /// In this user mode at any given point of time only one user can access the database. The user can be any user who has access to the database.
        /// </summary>
        SingleUser,
        /// <summary>
        /// In this user mode only the users who have db_owner or db_creator permission can access.
        /// </summary>
        RestrictedUser
    }
}
