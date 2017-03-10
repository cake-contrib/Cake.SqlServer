using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    internal static class SqlServerAliasesImpl
    {
        private static int? commandTimeout;

        internal static void DropDatabase(ICakeContext context, String connectionString, String databaseName)
        {
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
                    var command = CreateSqlCommand(dropDatabaseSql, connection);
                    command.Parameters.AddWithValue("@DatabaseName", databaseName);

                    context.Log.Information($"About to drop database {databaseName}");
                    command.ExecuteNonQuery();
                    context.Log.Information($"Database {databaseName} is dropped");
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Message.StartsWith("Cannot open database"))
                {
                    context.Log.Error($"Database {databaseName} does not exits");
                    return;
                }
                throw;
            }
        }

        internal static void CreateDatabaseIfNotExists(ICakeContext context, String connectionString, String databaseName)
        {
            var createDbSql = $"if (select DB_ID(@DatabaseName)) is null create database {Sql.EscapeName(databaseName)}";

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                var sqlToExecute = String.Format(createDbSql, connection.Database);
                context.Log.Debug($"Executing SQL : {sqlToExecute}");

                var command = CreateSqlCommand(sqlToExecute, connection);
                command.Parameters.AddWithValue("@DatabaseName", databaseName);

                command.ExecuteNonQuery();
                context.Log.Information($"Database {databaseName} is created if it was not there");
            }
        }


        internal static void CreateDatabase(ICakeContext context, String connectionString, String databaseName)
        {
            var createDbSql = $"create database {Sql.EscapeName(databaseName)}";

            using (var connection = OpenSqlConnection(context, connectionString))
            {
                var sqlToExecute = String.Format(createDbSql, connection.Database);
                context.Log.Debug($"Executing SQL : {sqlToExecute}");

                var command = CreateSqlCommand(sqlToExecute, connection);

                command.ExecuteNonQuery();
                context.Log.Information($"Database {databaseName} is created");
            }
        }


        internal static void DropAndCreateDatabase(ICakeContext context, String connectionString, String databaseName)
        {
            DropDatabase(context, connectionString, databaseName);
            CreateDatabase(context, connectionString, databaseName);
        }


        internal static void ExecuteSqlCommand(ICakeContext context, String connectionString, string sqlCommands)
        {
            using (var connection = OpenSqlConnection(context, connectionString))
            {
                ExecuteSqlCommand(context, connection, sqlCommands);
            }
        }


        internal static void ExecuteSqlCommand(ICakeContext context, SqlConnection connection, string sqlCommands)
        {
            var commandStrings = Regex.Split(sqlCommands, @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var sqlCommand in commandStrings)
            {
                if (sqlCommand.Trim() != "")
                {
                    context.Log.Debug($"Executing SQL : {sqlCommand}");
                    try
                    {
                        var command = CreateSqlCommand(sqlCommand, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        context.Log.Warning($"Exception happened while executing this command: {sqlCommand}");
                        throw;
                    }
                }
            }
        }


        internal static void ExecuteSqlFile(ICakeContext context, String connectionString, FilePath sqlFile)
        {
            using (var connection = OpenSqlConnection(context, connectionString))
            {
                ExecuteSqlFile(context, connection, sqlFile);
            }
        }


        internal static void ExecuteSqlFile(ICakeContext context, SqlConnection connection, FilePath sqlFile)
        {
            var sqlFilePath = sqlFile.FullPath;

            context.Log.Information($"Executing sql file {sqlFilePath}");

            var allSqlCommands = File.ReadAllText(sqlFilePath);

            context.ExecuteSqlCommand(connection, allSqlCommands);

            context.Log.Information($"Finished executing SQL from {sqlFilePath}");
        }


        internal static SqlConnection OpenSqlConnection(ICakeContext context, String connectionString)
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                context.Log.Debug($"About to open connection with this connection string: {connectionString}");
                connection.Open();
                return connection;
            }
            catch (SqlException exception)
            {
                if (exception.Message.StartsWith("A network-related or instance-specific error", StringComparison.OrdinalIgnoreCase)
                        && (connectionString.ToLower().Contains("localdb") || connectionString.ToLower().Contains("\v")))
                {
                    var errorMessage = "Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'? It should look like 'var connString = @\"(localDb)\\v12.0\"'";
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
    }
}
