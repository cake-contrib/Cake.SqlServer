using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    /// <summary>
    /// <para>
    ///    Contains functionality to deal with SQL Server: DropDatabase, CreateDatabase, execute SQL, execute SQL from files, etc. 
    /// </para>
    /// <para>
    ///    Samples here show work with "LocalDb\v12.0". This used to be default name for LocalDB instance when installed with SQL Server 2012. 
    ///    Since Sql Server 2014 the default name for LocalDB instance is "MSSQLLocalDB", making the default instance name for LocalDB looking like this:
    ///    "(LocalDB)\MSSQLLocalDB". So before using "v12.0" double check what instance you have installed and go from there. 
    ///    Also plesase don't be alarmed that all the examples are using LocalDB. The plugin is capable of working with any SQL Server installation.
    /// </para>
    /// <para>
    ///     If you have complex connection strings, please consider using <see href="https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnectionstringbuilder">SqlConnectionStringBuilder</see>
    ///     for creating your connection strings:
    ///     <code>
    ///      var connectionString = new SqlConnectionStringBuilder { DataSource = @"(LocalDb)\MSSQLLocalDB", InitialCatalog = databaseName }.ToString()
    ///     </code>
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
        /// <param name="connectionString">Connection string where to connect to. For this operation it is preferrable to connect to master database</param>
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

            SqlServerAliasesFacade.DropDatabase(context, connectionString, databaseName);
        }



        /// <summary>
        /// Creates an empty database. If database with this name already exists, throws a SqlException.
        /// <see cref="CreateDatabaseIfNotExists"/> if you would like to check if database already exists.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="connectionString">Connection string where to connect to. For this operation it is preferrable to connect to master database</param>
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

            SqlServerAliasesFacade.CreateDatabase(context, connectionString, databaseName);
        }


        /// <summary>
        /// Creates an empty database if another database with the same does not already exist.
        /// </summary>
        /// <param name="context">The Cake context</param>
        /// <param name="connectionString">Connection string where to connect to. For this operation it is preferrable to connect to master database</param>
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

            SqlServerAliasesFacade.CreateDatabaseIfNotExists(context, connectionString, databaseName);
        }


        /// <summary>
        /// First drops, then recreates the database
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">Connection string where to connect to. For this operation it is preferrable to connect to master database</param>
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

            SqlServerAliasesFacade.DropAndCreateDatabase(context, connectionString, databaseName);
        }


        /// <summary>
        /// Execute any SQL command.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">Connection string where to connect to. Connection script must specify what database to connect to</param>
        /// <param name="sqlCommands">SQL to be executed</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Database-Operations")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(LocalDb)\v12.0";
        ///             var sqlCommand = "Create table [CakeTest].dbo.[CakeTestTable] (id int null)";
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

            SqlServerAliasesFacade.ExecuteSqlCommand(context, connectionString, sqlCommands);
        }


        /// <summary>
        /// Reads sql commands from file and executes them.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">Connection string where to connect to. Connection script must specify what database to connect to</param>
        /// <param name="sqlFile">Path to a file with sql commands</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        ///
        ///     Task("ReCreate-Database")
        ///          .Does(() =>
        ///          {
        ///             var connectionString = @"Server=(LocalDb)\v12.0";
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

            SqlServerAliasesFacade.ExecuteSqlFile(context, connectionString, sqlFile);
        }
    }
}
