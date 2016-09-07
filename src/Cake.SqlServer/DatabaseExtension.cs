using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;


namespace Cake.SqlServer
{
    public static class DatabaseExtension
    {
        [CakeMethodAlias]
        public static void DropDatabase(this ICakeContext context, String connectionString, String databaseName)
        {
            var dropDatabaseSql =
            $@"if (select DB_ID('{databaseName}')) is not null
               begin
                    alter database[{ databaseName}] set offline with rollback immediate;
                    alter database[{ databaseName}] set online;
                    drop database[{ databaseName}];
                end";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

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


        [CakeMethodAlias]
        public static void CreateDatabaseIfNotExists(this ICakeContext context, String connectionString, String databaseName)
        {
            var createDbSql = $"if (select DB_ID('{databaseName}')) is null create database [{databaseName}]";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sqlToExecute = String.Format(createDbSql, connection.Database);
                context.Log.Debug($"Executing SQL : {sqlToExecute}");

                var command = new SqlCommand(sqlToExecute, connection);

                command.ExecuteNonQuery();
                context.Log.Information($"Database {databaseName} is created if it was not there");
            }
        }


        [CakeMethodAlias]
        public static void DropAndCreateDatabase(this ICakeContext context, String connectionString, String databaseName)
        {
            context.DropDatabase(connectionString, databaseName);
            context.CreateDatabaseIfNotExists(connectionString, databaseName);
        }


        [CakeMethodAlias]
        public static void ExecuteSqlCommand(this ICakeContext context, String connectionString, string sqlCommands)
        {
            var commandStrings = Regex.Split(sqlCommands, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var sqlCommand in commandStrings)
                {
                    if (sqlCommand.Trim() != "")
                    {
                        context.Log.Debug($"Executing SQL : {sqlCommand}");
                        var command = new SqlCommand(sqlCommand, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }



        [CakeMethodAlias]
        public static void ExecuteSqlFile(this ICakeContext context, String connectionString, string sqlFile)
        {
            context.Log.Information($"Executing sql file {sqlFile}");

            var allSqlCommands = File.ReadAllText(sqlFile);

            context.ExecuteSqlCommand(connectionString, allSqlCommands);

            context.Log.Information($"Finished executing SQL from {sqlFile}");
        }
    }
}
