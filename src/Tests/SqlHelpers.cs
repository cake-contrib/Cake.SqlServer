using Microsoft.Data.SqlClient;
using Dapper;

namespace Tests
{
    public static class SqlHelpers
    {
        public static void ExecuteSql(string connectionString, string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql);
            }
        }

        public static bool DbExists(string connectionString, string dbName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var db = connection.QuerySingle<SqlObject>($"select DB_ID('{dbName}') as Id");

                return db.Id.HasValue;
            }
        }


        public static bool TableExists(string connectionString, string dbName, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var db = connection.QuerySingle<SqlObject>($"select OBJECT_ID('[{dbName}].dbo.{tableName}')  as Id");

                return db.Id.HasValue;
            }
        }


        public static void DropDatabase(string connectionString, string databaseName)
        {
            ExecuteSql(connectionString, $"if (select DB_ID('{databaseName}')) is not null drop database [{databaseName}]");
        }
    }
}
