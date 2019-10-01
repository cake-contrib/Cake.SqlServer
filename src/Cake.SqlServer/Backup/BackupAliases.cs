using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    /// <summary>
    /// Contains functionality to backup and restore SQL Server database
    /// </summary>
    [CakeAliasCategory("SqlServer")]
    public static class BackupAliases
    {
        /// <summary>
        /// Restores a database from multiple backup files.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="settings">Settings for restoring database</param>
        /// <param name="backupFiles">Absolute path to (multiple) .bak files</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Restore-Database")
        ///         .Does(() =>
        ///         {
        ///             var connString = @"data source=(localdb)\MSSqlLocalDb";
        ///             var backupFile1 = new FilePath("C:/tmp/myBackup1.bak");
        ///             var backupFile2 = new FilePath("C:/tmp/myBackup2.bak");
        ///             RestoreMultipleSqlBackup(connString, new RestoreSqlBackupSettings()
        ///                {
        ///                      NewDatabaseName = "RestoredFromTest.Cake",
        ///                      NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()), // place files in Temp folder
        ///                      WithReplace = true, // tells sql server to discard non-backed up data when overwriting existing database
        ///                }, backupFile1, backupFile2);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void RestoreMultipleSqlBackup(this ICakeContext context, String connectionString, RestoreSqlBackupSettings settings, params FilePath[] backupFiles)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));

            RestoreSqlBackupImpl.RestoreSqlBackup(context, connectionString, settings, backupFiles);
        }

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
        ///             var connString = @"data source=(localdb)\MSSqlLocalDb";
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

            RestoreSqlBackupImpl.RestoreSqlBackup(context, connectionString, settings, backupFile);
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
        ///             var connString = @"data source=(localdb)\MSSqlLocalDb";
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

        /// <summary>
        /// Backup an existing database to a file.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. Regardless of the database specified, the connection will switch to master database for this operation.</param>
        /// <param name="databaseName">Database to backup.</param>
        /// <param name="settings">Settings for backing up database.</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Backup-Database")
        ///         .Does(() =>
        ///         {
        ///             var connString = @"data source=(localdb)\MSSqlLocalDb";
        ///             var databaseName = "MyDatabase";
        ///             BackupDatabase(connString, databaseName, new BackupDatabaseSettings() 
        ///                {
        ///                      Compress = false,
        ///                      Path = System.IO.Path.GetTempPath() // place files in Temp folder
        ///                }); 
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void BackupDatabase(this ICakeContext context, string connectionString, string databaseName, BackupDatabaseSettings settings)
        {
            BackupDatabaseImpl.Execute(context, connectionString, databaseName, settings);
        }
    }
}
