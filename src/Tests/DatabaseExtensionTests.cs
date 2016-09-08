using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.SqlServer;
using Dapper;
using FluentAssertions;
using NUnit.Framework;


// ReSharper disable InvokeAsExtensionMethod
namespace Tests
{
    public class DatabaseExtensionTests
    {
        private const String ConnectionString = @"data source=(LocalDb)\v12.0";
        private readonly ICakeContext context;

        public DatabaseExtensionTests()
        {
            context = NSubstitute.Substitute.For<ICakeContext>();
        }


        [Test]
        public void DropDatabase_DoesNotExist_DoesNotThrow()
        {
            Action act = () => DatabaseExtension.DropDatabase(context, ConnectionString, "DoesNotExist");

            act.ShouldNotThrow();
        }


        [Test]
        public void DropDatabase_ActuallyDrops()
        {
            //Arrange
            var dbName = "WillBeDropped";
            ExecuteSql($"Create database {dbName}");

            // Act
            DatabaseExtension.DropDatabase(context, ConnectionString, dbName);

            // Assert
            DbExists(dbName).Should().BeFalse();
        }


        [Test]
        public void CreateDatabaseIfNotExists()
        {
            // Act
            var dbName = "CakeTest";
            DatabaseExtension.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

            // Assert
            DbExists(dbName).Should().BeTrue();

            // Cleanup
            ExecuteSql($"drop database {dbName}");
        }



        [Test]
        public void DropAndCreateDatabase_Always_RemovesTable()
        {
            //Arrange
            var dbName = "ToBeRecreated";
            var tableName = "WillNotExist";
            ExecuteSql($"Create database {dbName}");
            ExecuteSql($"create table [{dbName}].dbo.{tableName} (id int null)");
            TableExists(dbName, tableName).Should().BeTrue();

            // Act
            DatabaseExtension.DropAndCreateDatabase(context, ConnectionString, dbName);

            // Assert
            TableExists(dbName, tableName).Should().BeFalse();
            ExecuteSql($"drop database {dbName}");
        }



        [Test]
        public void ExecuteSqlCommand_CreatesTables()
        {
            //Arrange
            var dbName = "ForSqlExecution";
            var tableName1 = "WillExist1";
            var tableName2 = "WillExist2";
            ExecuteSql($"Create database {dbName}");
            var sql = $@"
            create table [{dbName}].dbo.{tableName1} (id int null);
            Go
            create table [{dbName}].dbo.{tableName2} (id int null);
            Go
            ";

            // Act
            DatabaseExtension.ExecuteSqlCommand(context, ConnectionString, sql);

            // Assert
            TableExists(dbName, tableName1).Should().BeTrue();
            TableExists(dbName, tableName2).Should().BeTrue();
            ExecuteSql($"drop database {dbName}");
        }



        [Test]
        public void MethodName_StateUnderTests_ExpectedBehaviour()
        {
            //Arrange
            var dbName = "ForFileExecution";
            var tableName1 = "WillExist1";
            var tableName2 = "WillExist2";
            ExecuteSql($"Create database {dbName}");
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var sqlFilePath = Directory.GetFiles(baseDirectory, "Script.sql", SearchOption.AllDirectories).FirstOrDefault();


            // Act
            DatabaseExtension.ExecuteSqlFile(context, ConnectionString, sqlFilePath);

            // Assert
            TableExists(dbName, tableName1).Should().BeTrue();
            TableExists(dbName, tableName2).Should().BeTrue();
            ExecuteSql($"drop database {dbName}");
        }

        private void ExecuteSql(String sql)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute(sql);
            }
        }

        private static bool DbExists(String dbName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var db = connection.QuerySingle<SqlObject>($"select DB_ID('{dbName}') as Id");

                return db.Id.HasValue;
            }
        }



        private static bool TableExists(string dbName, string tableName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var db = connection.QuerySingle<SqlObject>($"select OBJECT_ID('[{dbName}].dbo.{tableName}')  as Id");

                return db.Id.HasValue;
            }
        }


        private class SqlObject
        {
            public int? Id { get; set; }
        }
    }
}
