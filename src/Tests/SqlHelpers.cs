using System;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Tests
{
    public class SqlHelpers
    {
        public static void ExecuteSql(String connectionString, String sql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql);
            }
        }

        public static bool DbExists(String connectionString, String dbName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var db = connection.QuerySingle<SqlObject>($"select DB_ID('{dbName}') as Id");

                return db.Id.HasValue;
            }
        }


        public static bool TableExists(String connectionString, string dbName, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var db = connection.QuerySingle<SqlObject>($"select OBJECT_ID('[{dbName}].dbo.{tableName}')  as Id");

                return db.Id.HasValue;
            }
        }


        public static void DropDatabase(String connectionString, string databaseName)
        {
            ExecuteSql(connectionString, $"if (select DB_ID('{databaseName}')) is not null drop database [{databaseName}]");
        }

    }
    public class SqlObject
    {
        public int? Id { get; set; }
    }
}
