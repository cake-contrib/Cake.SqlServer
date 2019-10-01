using System;
using System.Collections.Generic;
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
        /// <param name="differentialBackupFiles">Absolute path to (multiple) additional differential .bak files</param>
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
        ///             var backupFileList = new List&lt;FilePath&gt; {backupFile1, backupFile2};
        ///             var diffBackupFile1 = new FilePath("C:/tmp/myDiffBackup1.bak");
        ///             var diffBackupFile2 = new FilePath("C:/tmp/myDiffBackup2.bak");
        ///             var diffBackupFileList = new List&lt;FilePath&gt; {diffBackupFile1, diffBackupFile2};
        ///             RestoreMultipleSqlBackup(connString, new RestoreSqlBackupSettings()
        ///                {
        ///                      NewDatabaseName = "RestoredFromTest.Cake",
        ///                      NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()), // place files in Temp folder
        ///                      WithReplace = true, // tells sql server to discard non-backed up data when overwriting existing database
        ///                      BackupSetFile = 1, // tells which backup set file to use for backupFile*
        ///                      DifferentialBackupSetFile = 1, // tells which backup set file to use for diffBackupFile*
        ///                }, backupFileList, diffBackupFileList);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void RestoreMultipleSqlBackup(this ICakeContext context, String connectionString, RestoreSqlBackupSettings settings, IList<FilePath> backupFiles, IList<FilePath> differentialBackupFiles = null)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));

            RestoreSqlBackupImpl.RestoreSqlBackup(context, connectionString, settings, backupFiles, differentialBackupFiles);
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

            RestoreSqlBackupImpl.RestoreSqlBackup(context, connectionString, settings, new List<FilePath> {backupFile});
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
