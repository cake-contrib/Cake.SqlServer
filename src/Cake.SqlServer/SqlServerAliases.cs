﻿using System;
using Microsoft.Data.SqlClient;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
// ReSharper disable MemberCanBePrivate.Global


namespace Cake.SqlServer
{
    /// <summary>
    /// <para>
    /// Contains functionality to deal with SQL Server: DropDatabase, CreateDatabase, execute SQL, execute SQL from files, etc.
    /// Provides functionality to manage LocalDb instances: Create, Start, Stop, Delete instances;
    /// </para>
    /// <para>
    /// In order to use the commands for this addin, include the following in your build.cake file to download and
    /// reference from NuGet.org:
    /// <code>
    ///     #addin "nuget:?package=Cake.SqlServer"
    /// </code>
    /// </para>
    /// </summary>
    [CakeAliasCategory(nameof(SqlServer))]
    public static class SqlServerAliases
    {
        /// <summary>
        /// Test if the database exists
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database name to test</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Deploy-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///
        ///             if (DatabaseExists(connectionString, dbName))
        ///             {
        ///                 throw new Exception("A database with the same name already exists");
        ///             }
        ///             // do other stuff
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static bool DatabaseExists(this ICakeContext context, string connectionString, string databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            return SqlServerAliasesImpl.DatabaseExists(context, connectionString, databaseName);
        }

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
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             DropDatabase(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void DropDatabase(this ICakeContext context, string connectionString, string databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.DropDatabase(context, connectionString, databaseName);
        }



        /// <summary>
        /// Creates an empty database. If database with this name already exists, throws a SqlException.
        /// <see cref="CreateDatabaseIfNotExists(ICakeContext, String, String)"/> if you would like to check if database already exists.
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
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             CreateDatabase(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateDatabase(this ICakeContext context, string connectionString, string databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.CreateDatabase(context, connectionString, databaseName);
        }


        /// <summary>
        /// Creates an empty database. If database with this name already exists, throws a SqlException.
        /// Allows to specify primary and log files location.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database name to be created</param>
        /// <param name="settings">Settings object with parameters</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Create-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             var createSettings = new CreateDatabaseSettings()
        ///                                          .WithPrimaryFile(@"C:\MyPath\DB\CakeTest.mdf")
        ///                                          .WithLogFile(@"C:\MyPath\DB\CakeTest.ldf");
        ///             CreateDatabase(connectionString, dbName, createSettings);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateDatabase(this ICakeContext context, string connectionString, string databaseName, CreateDatabaseSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));
            Guard.ArgumentIsNotNull(settings, nameof(settings));


            SqlServerAliasesImpl.CreateDatabase(context, connectionString, databaseName, settings);
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
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             CreateDatabaseIfNotExists(connectionString, dbName);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateDatabaseIfNotExists(this ICakeContext context, string connectionString, string databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.CreateDatabaseIfNotExists(context, connectionString, databaseName);
        }

        /// <summary>
        /// Creates an empty database if another database with the same does not already exist.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database name to be created</param>
        /// <param name="settings">Settings object with parameters</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("Create-Database-If-Not-Exists")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             var createSettings = new CreateDatabaseSettings()
        ///                                          .WithPrimaryFile(@"C:\MyPath\DB\CakeTest.mdf")
        ///                                          .WithLogFile(@"C:\MyPath\DB\CakeTest.ldf");
        ///             CreateDatabaseIfNotExists(connectionString, dbName, createSettings);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateDatabaseIfNotExists(this ICakeContext context, string connectionString, string databaseName, CreateDatabaseSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            SqlServerAliasesImpl.CreateDatabaseIfNotExists(context, connectionString, databaseName, settings);
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
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             var createSettings = new CreateDatabaseSettings()
        ///                                          .WithPrimaryFile(@"C:\MyPath\DB\CakeTest.mdf")
        ///                                          .WithLogFile(@"C:\MyPath\DB\CakeTest.ldf");
        ///             DropAndCreateDatabase(connectionString, dbName, createSettings);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void DropAndCreateDatabase(this ICakeContext context, string connectionString, string databaseName)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));

            SqlServerAliasesImpl.DropAndCreateDatabase(context, connectionString, databaseName);
        }



        /// <summary>
        /// First drops, then recreates the database
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. For this operation, it is recommended to connect to the master database (default). If there are changing parameters, <see cref="SqlConnectionStringBuilder"/> is recommended to escape input.</param>
        /// <param name="databaseName">Database to be dropped and re-created</param>
        /// <param name="settings">Settings object with parameters</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("ReCreate-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(localdb)\MSSqlLocalDb";
        ///             var dbName = "CakeTest";
        ///             var createSettings = new CreateDatabaseSettings()
        ///                                          .WithPrimaryFile(@"C:\MyPath\DB\CakeTest.mdf")
        ///                                          .WithLogFile(@"C:\MyPath\DB\CakeTest.ldf");
        ///             DropAndCreateDatabase(connectionString, dbName, createSettings);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void DropAndCreateDatabase(this ICakeContext context, string connectionString, string databaseName, CreateDatabaseSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(databaseName, nameof(databaseName));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            SqlServerAliasesImpl.DropAndCreateDatabase(context, connectionString, databaseName, settings);
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
        ///             var connectionString = @"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase";
        ///             var sqlCommand = "create table [CakeTest].dbo.[CakeTestTable] (id int null)";
        ///             ExecuteSqlCommand(connectionString, sqlCommand);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExecuteSqlCommand(this ICakeContext context, string connectionString, string sqlCommands)
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
        ///             using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase"))
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
        ///             var connectionString = @"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase";
        ///             var sqlFile = "./somePath/MyScript.sql";
        ///             ExecuteSqlCommand(connectionString, sqlFile);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExecuteSqlFile(this ICakeContext context, string connectionString, FilePath sqlFile)
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
        ///             using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase"))
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
        ///             using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase"))
        ///             {
        ///                 ExecuteSqlCommand(connection, "...");
        ///                 ExecuteSqlFile(connection, "./somePath/MyScript.sql");
        ///             }
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static SqlConnection OpenSqlConnection(this ICakeContext context, string connectionString)
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
        ///             SetSqlCommandTimeout(60);
        ///             using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase"))
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
    }
}
