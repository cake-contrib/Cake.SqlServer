using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    internal static class SqlServerAliasesFacade
    {
        internal static void DropDatabase(ICakeContext context, String connectionString, String databaseName)
        {
            var dropDatabaseSql =
                $@"if (select DB_ID('{databaseName}')) is not null
               begin
                    alter database[{databaseName}] set offline with rollback immediate;
                    alter database[{databaseName}] set online;
                    drop database[{databaseName}];
                end";

            try
            {
                using (var connection = CreateOpenConnection(connectionString, context))
                {
                    var command = new SqlCommand(dropDatabaseSql, connection);

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
            var createDbSql = $"if (select DB_ID('{databaseName}')) is null create database [{databaseName}]";

            using (var connection = CreateOpenConnection(connectionString, context))
            {
                var sqlToExecute = String.Format(createDbSql, connection.Database);
                context.Log.Debug($"Executing SQL : {sqlToExecute}");

                var command = new SqlCommand(sqlToExecute, connection);

                command.ExecuteNonQuery();
                context.Log.Information($"Database {databaseName} is created if it was not there");
            }
        }



        internal static void DropAndCreateDatabase(ICakeContext context, String connectionString, String databaseName)
        {
            DropDatabase(context, connectionString, databaseName);
            CreateDatabaseIfNotExists(context, connectionString, databaseName);
        }


        internal static void ExecuteSqlCommand(ICakeContext context, String connectionString, string sqlCommands)
        {
            var commandStrings = Regex.Split(sqlCommands, @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var connection = CreateOpenConnection(connectionString, context))
            {
                foreach (var sqlCommand in commandStrings)
                {
                    if (sqlCommand.Trim() != "")
                    {
                        context.Log.Debug($"Executing SQL : {sqlCommand}");
                        try
                        {
                            var command = new SqlCommand(sqlCommand, connection);
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
        }


        internal static void ExecuteSqlFile(ICakeContext context, String connectionString, FilePath sqlFile)
        {
            var sqlFilePath = sqlFile.FullPath;

            context.Log.Information($"Executing sql file {sqlFilePath}");

            var allSqlCommands = File.ReadAllText(sqlFilePath);

            context.ExecuteSqlCommand(connectionString, allSqlCommands);

            context.Log.Information($"Finished executing SQL from {sqlFilePath}");
        }


        private static SqlConnection CreateOpenConnection(String connectionString, ICakeContext context)
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
                if (exception.Message.StartsWith("A network-related or instance-specific error", StringComparison.InvariantCultureIgnoreCase)
                        && connectionString.ToLower().Contains("localdb"))
                {
                    context.Log.Warning("Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'. It should look like 'var connString = @\"(localDb)\\v12.0\"'");
                }
                throw;
            }
        }
    }
}
