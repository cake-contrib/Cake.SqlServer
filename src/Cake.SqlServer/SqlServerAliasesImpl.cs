using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Microsoft.Data.SqlClient;

namespace Cake.SqlServer
{
    internal static class SqlServerAliasesImpl
    {
        private static int? commandTimeout;

        internal static void DropDatabase(ICakeContext context, string connectionString, string databaseName)
        {
            Initializer.InitializeNativeSearchPath();
            var dropDatabaseSql =
                $@"if (select DB_ID(@DatabaseName)) is not null
               begin
                    alter database {Sql.EscapeName(databaseName)} set offline with rollback immediate;
                    alter database {Sql.EscapeName(databaseName)} set online;
                    drop database {Sql.EscapeName(databaseName)};
                end";

            try
            {
                using (var connection = OpenSqlConnection(context, connectionString))
                {
                    using (var command = CreateSqlCommand(dropDatabaseSql, connection))
                    {
                        command.Parameters.AddWithValue("@DatabaseName", databaseName);

                        context.Log.Information($"About to drop database {databaseName}");
                        command.ExecuteNonQuery();
                    }

                    context.Log.Information($"Database {databaseName} is dropped");
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Message.StartsWith("Cannot open database", StringComparison.OrdinalIgnoreCase))
                {
                    context.Log.Error($"Database {databaseName} does not exits");
                    return;
                }

                throw;
            }
        }

        internal static bool DatabaseExists(ICakeContext context, string connectionString, string databaseName)
        {
            Initializer.InitializeNativeSearchPath();
            const string databaseExistsSql = "select DB_ID(@DatabaseName) as Id";

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                context.Log.Debug($"Executing SQL : {databaseExistsSql}");
                using (var command = CreateSqlCommand(databaseExistsSql, connection))
                {
                    command.Parameters.AddWithValue("@DatabaseName", databaseName);
                    return command.ExecuteScalar() != DBNull.Value;
                }
            }
        }

        internal static void CreateDatabaseIfNotExists(ICakeContext context, string connectionString, string databaseName)
        {
            Initializer.InitializeNativeSearchPath();
            var createDbSql = $"if (select DB_ID(@DatabaseName)) is null create database {Sql.EscapeName(databaseName)}";

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                context.Log.Debug($"Executing SQL : {createDbSql}");

                using (var command = CreateSqlCommand(createDbSql, connection))
                {
                    command.Parameters.AddWithValue("@DatabaseName", databaseName);

                    command.ExecuteNonQuery();
                }

                context.Log.Information($"Database {databaseName} is created if it was not there");
            }
        }

        internal static void CreateDatabaseIfNotExists(ICakeContext context, string connectionString, string databaseName, CreateDatabaseSettings settings)
        {
            Initializer.InitializeNativeSearchPath();
            settings.AssignNames(databaseName);

            var sql = GenerateCreateDbSql(databaseName, settings);

            sql = "if (select DB_ID(@DatabaseName)) is null " + sql;

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                context.Log.Debug($"Executing SQL : {sql}");

                using (var command = CreateSqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DatabaseName", databaseName);
                    command.ExecuteNonQuery();
                }

                context.Log.Information($"Database {databaseName} is created if it was not there");
            }
        }

        internal static void CreateDatabase(ICakeContext context, string connectionString, string databaseName)
        {
            Initializer.InitializeNativeSearchPath();
            var createDbSql = $"create database {Sql.EscapeName(databaseName)}";

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                context.Log.Debug($"Executing SQL : {createDbSql}");

                using (var command = CreateSqlCommand(createDbSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                context.Log.Information($"Database {databaseName} is created");
            }
        }

        internal static void CreateDatabase(ICakeContext context, string connectionString, string databaseName, CreateDatabaseSettings settings)
        {
            Initializer.InitializeNativeSearchPath();
            settings.AssignNames(databaseName);

            var sql = GenerateCreateDbSql(databaseName, settings);

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                context.Log.Debug($"Executing SQL : {sql}");

                using (var command = CreateSqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                context.Log.Information($"Database {databaseName} is created if it was not there");
            }
        }

        internal static void DropAndCreateDatabase(ICakeContext context, string connectionString, string databaseName)
        {
            Initializer.InitializeNativeSearchPath();
            DropDatabase(context, connectionString, databaseName);
            CreateDatabase(context, connectionString, databaseName);
        }

        internal static void DropAndCreateDatabase(ICakeContext context, string connectionString, string databaseName, CreateDatabaseSettings settings)
        {
            Initializer.InitializeNativeSearchPath();
            DropDatabase(context, connectionString, databaseName);
            CreateDatabase(context, connectionString, databaseName, settings);
        }

        internal static void ExecuteSqlCommand(ICakeContext context, string connectionString, string sqlCommands)
        {
            Initializer.InitializeNativeSearchPath();
            using (var connection = OpenSqlConnection(context, connectionString))
            {
                ExecuteSqlCommand(context, connection, sqlCommands);
            }
        }

        internal static void ExecuteSqlCommand(ICakeContext context, SqlConnection connection, string sqlCommands)
        {
            Initializer.InitializeNativeSearchPath();
            var commandStrings = Regex.Split(
                sqlCommands,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(5));

            foreach (var sqlCommand in commandStrings)
            {
                if (sqlCommand.Trim() != string.Empty)
                {
                    context.Log.Debug($"Executing SQL : {sqlCommand}");
                    try
                    {
                        using (var command = CreateSqlCommand(sqlCommand, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        context.Log.Warning($"Exception happened while executing this command: {sqlCommand}");
                        throw;
                    }
                }
            }
        }

        internal static void ExecuteSqlFile(ICakeContext context, string connectionString, FilePath sqlFile)
        {
            Initializer.InitializeNativeSearchPath();
            using (var connection = OpenSqlConnection(context, connectionString))
            {
                ExecuteSqlFile(context, connection, sqlFile);
            }
        }

        internal static void ExecuteSqlFile(ICakeContext context, SqlConnection connection, FilePath sqlFile)
        {
            Initializer.InitializeNativeSearchPath();
            var sqlFilePath = sqlFile.FullPath;

            context.Log.Information($"Executing sql file {sqlFilePath}");

#pragma warning disable SEC0116
            var allSqlCommands = File.ReadAllText(sqlFilePath);
#pragma warning restore SEC0116

            context.ExecuteSqlCommand(connection, allSqlCommands);

            context.Log.Information($"Finished executing SQL from {sqlFilePath}");
        }

        internal static SqlConnection OpenSqlConnection(ICakeContext context, string connectionString)
        {
            Initializer.InitializeNativeSearchPath();
            try
            {
                var connection = new SqlConnection(connectionString);
                context.Log.Debug($"About to open connection with this connection string: {connectionString}");
                connection.Open();
                return connection;
            }
            catch (SqlException exception)
            {
                if (exception.Message.StartsWith("A network-related or instance-specific error", StringComparison.InvariantCultureIgnoreCase)
                    && (connectionString.ToLower(CultureInfo.InvariantCulture).Contains("localdb", StringComparison.OrdinalIgnoreCase)
                        || connectionString.ToLower(CultureInfo.InvariantCulture).Contains('\v', StringComparison.OrdinalIgnoreCase)))
                {
                    const string errorMessage = "Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'? It should look like 'var connString = @\"(localDb)\\v12.0\"'";
                    context.Log.Error(errorMessage);
                    var newException = new Exception(errorMessage, exception);
                    throw newException;
                }

                throw;
            }
        }

        internal static void SetSqlCommandTimeout(ICakeContext context, int commandTimeout)
        {
            context.Log.Debug($"Default SQL timeout have been changed to {commandTimeout} seconds");
            SqlServerAliasesImpl.commandTimeout = commandTimeout;
        }

        internal static SqlCommand CreateSqlCommand(string sql, SqlConnection connection)
        {
            var command = new SqlCommand(sql, connection);

            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            return command;
        }

        private static string GenerateCreateDbSql(string databaseName, CreateDatabaseSettings settings)
        {
            var sb = new StringBuilder($" create database {Sql.EscapeName(databaseName)}");
            if (settings.PrimaryFile != null || settings.LogFile != null)
            {
                sb.Append(" ON ");
            }

#pragma warning disable S1135 // Track uses of "TODO" tags
#pragma warning disable MA0026 // Fix TODO comment
            if (settings.PrimaryFile != null)
            {
                // TODO replace with param
                sb.Append(" PRIMARY (Name = ")
                    .Append(Sql.EscapeName(settings.PrimaryFile.Name))
                    .Append(", FILENAME = '")
                    .Append(settings.PrimaryFile.FileName)
                    .Append("') ");
            }
#pragma warning restore MA0026 // Fix TODO comment
#pragma warning restore S1135 // Track uses of "TODO" tags

            if (settings.LogFile != null)
            {
                sb.Append(" LOG ON (NAME = ")
                    .Append(Sql.EscapeName(settings.LogFile.Name))
                    .Append(", FILENAME = '")
                    .Append(settings.LogFile.FileName)
                    .Append("') ");
            }

            return sb.ToString();
        }
    }
}
