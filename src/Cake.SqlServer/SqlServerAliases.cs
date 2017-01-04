using System;
using System.Data.SqlClient;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    /// <summary>
    /// <para>
    /// Contains functionality to deal with SQL Server: DropDatabase, CreateDatabase, execute SQL, execute SQL from files, etc. 
    /// Also provides functionality to manage LocalDb instances: Create, Start, Stop, Delete instances.
    /// </para>
    /// <para>
    /// In order to use the commands for this addin, include the following in your build.cake file to download and
    /// reference from NuGet.org:
    /// <code>
    ///     #addin "nuget:?package=Cake.SqlServer"
    /// </code>
    /// </para>
    /// </summary>
    [CakeAliasCategory("SqlServer")]
    public static class SqlServerAliases
    {
        /// <summary>
        /// Drops database. Before dropping the DB, database is set to be offline, then online again.
        /// This is to be sure that there are no live connections, otherwise the script will fail.
        /// Also if the database does not exist - it will not do anything.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database name to be dropped</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Drop-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(LocalDb)\v12.0";
        ///             var dbName = "CakeTest";
        ///             DropDatabase(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void DropDatabase(this ICakeContext context, String connectionString, String databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.DropDatabase(context, connectionString, databaseName);
        }



        /// <summary>
        /// Creates an empty database. If database with this name already exists, throws a SqlException.
        /// <see cref="CreateDatabaseIfNotExists"/> if you would like to check if database already exists.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database name to be created</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(LocalDb)\v12.0";
        ///             var dbName = "CakeTest";
        ///             CreateDatabase(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateDatabase(this ICakeContext context, String connectionString, String databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.CreateDatabase(context, connectionString, databaseName);
        }


        /// <summary>
        /// Creates an empty database if another database with the same does not already exist.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database name to be created</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Database-If-Not-Exists")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(LocalDb)\v12.0";
        ///             var dbName = "CakeTest";
        ///             CreateDatabaseIfNotExists(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateDatabaseIfNotExists(this ICakeContext context, String connectionString, String databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.CreateDatabaseIfNotExists(context, connectionString, databaseName);
        }


        /// <summary>
        /// First drops, then recreates the database
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database to be dropped and re-created</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("ReCreate-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(LocalDb)\v12.0";
        ///             var dbName = "CakeTest";
        ///             DropAndCreateDatabase(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void DropAndCreateDatabase(this ICakeContext context, String connectionString, String databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.DropAndCreateDatabase(context, connectionString, databaseName);
        }


        /// <summary>
        /// Execute any SQL command.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to specify Initial Catalog. If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="sqlCommands">SQL to be executed</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Sql-Operations")
        ///         .Does(() =>
        ///         {
        ///             var connectionString = @"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase";
        ///             var sqlCommand = "create table [CakeTest].dbo.[CakeTestTable] (id int null)";
        ///             ExecuteSqlCommand(connectionString, sqlCommand);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExecuteSqlCommand(this ICakeContext context, String connectionString, string sqlCommands)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(sqlCommands, nameof(sqlCommands));

            SqlServerAliasesImpl.ExecuteSqlCommand(context, connectionString, sqlCommands);
        }


        /// <summary>
        /// Execute any SQL command.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connection">The connection to use. The connection must be open. See <see cref="OpenSqlConnection"/>.</param>
        /// <param name="sqlCommands">SQL to be executed</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Sql-Operations")
        ///         .Does(() =>
        ///         {
        ///             using (var connection = OpenSqlConnection(@"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase"))
        ///             {
        ///                 ExecuteSqlCommand(connection, "create table [CakeTest].dbo.[CakeTestTable] (id int null)");
        ///                 ExecuteSqlCommand(connection, "...");
        ///             }
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExecuteSqlCommand(this ICakeContext context, SqlConnection connection, string sqlCommands)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connection, nameof(connection));
            Guard.ArgumentIsNotNull(sqlCommands, nameof(sqlCommands));

            SqlServerAliasesImpl.ExecuteSqlCommand(context, connection, sqlCommands);
        }


        /// <summary>
        /// Reads SQL commands from a file and executes them.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to specify Initial Catalog. If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="sqlFile">Path to a file with SQL commands.</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Sql-Operations")
        ///         .Does(() =>
        ///         {
        ///             var connectionString = @"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase";
        ///             var sqlFile = "./somePath/MyScript.sql";
        ///             ExecuteSqlCommand(connectionString, sqlFile);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExecuteSqlFile(this ICakeContext context, String connectionString, FilePath sqlFile)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(sqlFile, nameof(sqlFile));

            SqlServerAliasesImpl.ExecuteSqlFile(context, connectionString, sqlFile);
        }


        /// <summary>
        /// Reads SQL commands from a file and executes them.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connection">The connection to use. The connection must be open. See <see cref="OpenSqlConnection"/>.</param>
        /// <param name="sqlFile">Path to a file with SQL commands.</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Sql-Operations")
        ///         .Does(() =>
        ///         {
        ///             using (var connection = OpenSqlConnection(@"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase"))
        ///             {
        ///                 ExecuteSqlFile(connection, "./somePath/MyScript.sql");
        ///                 ExecuteSqlFile(connection, "./somePath/MyOtherScript.sql");
        ///             }
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExecuteSqlFile(this ICakeContext context, SqlConnection connection, FilePath sqlFile)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connection, nameof(connection));
            Guard.ArgumentIsNotNull(sqlFile, nameof(sqlFile));

            SqlServerAliasesImpl.ExecuteSqlFile(context, connection, sqlFile);
        }


        /// <summary>
        /// Opens a new <see cref="SqlConnection"/> with the given connection string.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to specify Initial Catalog. If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Sql-Operations")
        ///         .Does(() =>
        ///         {
        ///             using (var connection = OpenSqlConnection(@"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase"))
        ///             {
        ///                 ExecuteSqlCommand(connection, "...");
        ///                 ExecuteSqlFile(connection, "./somePath/MyScript.sql");
        ///             }
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static SqlConnection OpenSqlConnection(this ICakeContext context, String connectionString)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));

            return SqlServerAliasesImpl.OpenSqlConnection(context, connectionString);
        }


        /// <summary>
        /// Sets the CommandTimeout property for all SqlCommands used internally
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="commandTimeout">The time in seconds to wait for the command to execute. Used to set CommandTimeout property to when creating <see cref="SqlCommand"/></param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Sql-Operations")
        ///         .Does(() =>
        ///         {
        ///             SetCommandTimeout(60);
        ///             using (var connection = OpenSqlConnection(@"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase"))
        ///             {
        ///                 ExecuteSqlCommand(connection, "...");
        ///                 ExecuteSqlFile(connection, "./somePath/MyScript.sql");
        ///             }
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void SetSqlCommandTimeout(this ICakeContext context, int commandTimeout)
        {
            SqlServerAliasesImpl.SetSqlCommandTimeout(context, commandTimeout);
        }


        ///  <summary>
        ///  Restores a database from a backup file.
        ///  </summary>
        ///  <param name="context">The Cake context.</param>
        ///  <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        ///  <param name="backupFile">Absolute path to .bak file</param>
        /// <param name="settings"></param>
        /// <example>
        ///  <code>
        ///      #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///      Task("Restore-Database")
        ///          .Does(() =>
        ///          {
        ///              var connString = @"data source=(LocalDb)\v12.0";
        ///              var backupFile = new FilePath("C:/tmp/myBackup.bak");
        ///              RestoreSqlBackup(connString, backupFile); // use database name from the backup, store files in default location
        ///  
        ///              RestoreSqlBackup(connString, backupFile, "MyNewDbName"); // rename restored database
        ///  
        ///              var newLocation = new DirectoryPath("C:/myDatabases");
        ///              RestoreSqlBackup(connString, backupFile, "MyNewDbName", newLocation); // place files in special location
        ///          });
        ///  </code>
        ///  </example>
        [CakeMethodAlias]
        public static void RestoreSqlBackup(this ICakeContext context, String connectionString, FilePath backupFile, RestoreSqlBackupSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(backupFile, nameof(backupFile));

            SqlBackupsImpl.RestoreSqlBackup(context, connectionString, backupFile, settings.NewDatabaseName, settings.NewStorageFolder);
        }

        [CakeMethodAlias]
        public static void RestoreSqlBackup(this ICakeContext context, String connectionString, FilePath backupFile)
        {
            //context.Log.Write(Core.Diagnostics.Verbosity.Diagnostic, Core.Diagnostics.LogLevel.Debug, $"Backup file from {backupFile.MakeAbsolute(context.Environment)}");
            RestoreSqlBackup(context, connectionString, backupFile, new RestoreSqlBackupSettings());
        }


    }
    /// <summary>
    /// Test
    /// </summary>
    public class RestoreSqlBackupSettings
    {
        //  Name of the database where to restore. If this is not specified, database name is taken from the backup file</param>
        //  Path where data and log files should be stored.If this is not specified, server defaults will be used</param>
        public String NewDatabaseName { get; set; }
        public DirectoryPath NewStorageFolder { get; set; }
    }
}
