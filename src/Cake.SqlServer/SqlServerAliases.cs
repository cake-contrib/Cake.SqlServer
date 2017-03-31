using System;
using System.Data.SqlClient;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Microsoft.SqlServer.Dac;

// ReSharper disable MemberCanBePrivate.Global


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

        /// <summary>
        /// Creates a bacpac file for easy database backuping. 
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="databaseName">Name of the database you'd like to create a bacpac from</param>
        /// <param name="resultingFilePath">Full path where you'd like to store resulting bacpac</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Bacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(LocalDb)\v12.0";
        ///     
        ///     		var dbName = "ForBacpac";
        ///     
        ///     		CreateDatabase(connString, dbName);
        ///     
        ///     		CreateBacpacFile(connString, dbName, new FilePath(@".\ForBacpac.bacpac"));
        ///     	});
        ///     });        
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateBacpacFile(this ICakeContext context, String connectionString, String databaseName, FilePath resultingFilePath)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(resultingFilePath, nameof(resultingFilePath));

            SqlBacpacImpl.CreateBacpacFile(context, connectionString, databaseName, resultingFilePath.FullPath);
        }


        /// <summary>
        /// Restores a bacpac file into a database.
        /// <para>
        /// NB: there must be no database with the name you provide. Otherwise exception will be thrown.
        /// </para>
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="databaseName">Name of a new database you are creating </param>
        /// <param name="bacpacFilePath">Full path to the bacpac file</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Bacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(LocalDb)\v12.0";
        ///     
        ///     		var dbName = "FromBacpac";
        ///     
        ///     		var file = new FilePath(@".\src\Tests\Nsaga.bacpac");
        ///     
        ///     		RestoreBacpac(connString, dbName, file);
        ///     	});
        ///     });        
        /// </code>
        /// </example>        
        [CakeMethodAlias]
        public static void RestoreBacpac(this ICakeContext context, String connectionString, String databaseName, FilePath bacpacFilePath)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(bacpacFilePath, nameof(bacpacFilePath));

            SqlBacpacImpl.RestoreBacpac(context, connectionString, databaseName, bacpacFilePath.FullPath);
        }

        /// <summary>
        /// Extracts a dacpac file to a database package. 
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="targetDatabaseName">Name of the database you'd like to extract a package from</param>
        /// <param name="settings">Custom setting for the extract operation.</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Extract-Dacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(LocalDb)\v12.0";
        ///     
        ///     		var dbName = "ForDacpac";
        ///     
        ///     		CreateDatabase(connString, dbName);
        /// 
        ///     		var settings = new ExtractDacpacSettings("MyAppName", "2.0.0.0") { 
        ///     			OutputFile = new FilePath(@".\Nsaga.dacpac")
        /// 			};
        ///     
        ///     		ExtractDacpacFile(connString, dbName, settings);
        ///     	});
        ///     });        
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExtractDacpacFile(this ICakeContext context, String connectionString, String targetDatabaseName, ExtractDacpacSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(targetDatabaseName, nameof(targetDatabaseName));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            SqlDacpacImpl.ExtractDacpacFile(context, connectionString, targetDatabaseName, settings);
        }


        /// <summary>
        /// Publish a dacpac file to a database.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="targetDatabaseName">Name of a target database.</param>
        /// <param name="dacpacFilePath">Full path to the dacpac file.</param>
        /// <param name="settings">Configure the sql deployment</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Bacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(LocalDb)\v12.0";
        ///     
        ///     		var dbName = "ForDacpac";
        ///     
        ///     		var file = new FilePath(@".\src\Tests\Nsaga.dacpac");
        ///     
        ///     		var settings = new PublishDacpacSettings { 
        ///     			GenerateDeploymentScript = true
        /// 			};
        /// 
        ///     		PublishDacpacFile(connString, dbName, file, settings);
        ///     	});
        ///     });        
        /// </code>
        /// </example>        
        [CakeMethodAlias]
        public static void PublishDacpacFile(this ICakeContext context, String connectionString, String targetDatabaseName, FilePath dacpacFilePath, PublishDacpacSettings settings = null)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(dacpacFilePath, nameof(dacpacFilePath));

            SqlDacpacImpl.PublishDacpacFile(context, connectionString, targetDatabaseName, dacpacFilePath.FullPath, settings);
        }
    }
}
