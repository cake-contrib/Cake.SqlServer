using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;


namespace Cake.SqlServer
{ 
    /// <summary>
    /// Contains functionality to restore SQL Server backup files *.bak
    /// </summary>
    [CakeAliasCategory("SqlServer")]
    public static class BackupAliases
    {
        /// <summary>
        /// Restores a database from a backup file.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="backupFile">Absolute path to .bak file</param>
        /// <param name="settings">Settings for restoring database</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Restore-Database")
        ///         .Does(() =>
        ///         {
        ///             var connString = @"data source=(LocalDb)\v12.0";
        ///             var backupFile = new FilePath("C:/tmp/myBackup.bak");
        ///             RestoreSqlBackup(connString, backupFile, new RestoreSqlBackupSettings() 
        ///                {
        ///                      NewDatabaseName = "RestoredFromTest.Cake",
        ///                      NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()), // place files in Temp folder
        ///                      WithReplace = true, // tells sql server to discard non-backed up data when overwriting existing database
        ///                }); 
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void RestoreSqlBackup(this ICakeContext context, String connectionString, FilePath backupFile, RestoreSqlBackupSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(backupFile, nameof(backupFile));

            SqlBackupsImpl.RestoreSqlBackup(context, connectionString, backupFile, settings);
        }

        /// <summary>
        /// Restores a database from a backup file.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="backupFile">Absolute path to .bak file</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Restore-Database")
        ///         .Does(() =>
        ///         {
        ///             var connString = @"data source=(LocalDb)\v12.0";
        ///             var backupFile = new FilePath("C:/tmp/myBackup.bak");
        ///             RestoreSqlBackup(connString, backupFile); 
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void RestoreSqlBackup(this ICakeContext context, String connectionString, FilePath backupFile)
        {
            RestoreSqlBackup(context, connectionString, backupFile, new RestoreSqlBackupSettings());
        }
    }
}