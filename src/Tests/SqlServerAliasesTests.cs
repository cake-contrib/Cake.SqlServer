using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.SqlServer;
using Dapper;
using FluentAssertions;
using NUnit.Framework;
using NSubstitute;


// ReSharper disable InvokeAsExtensionMethod
namespace Tests
{
    public class SqlServerAliasesTests
    {
        private const String ConnectionString = @"data source=(LocalDb)\v12.0";
        private readonly ICakeContext context;

        public SqlServerAliasesTests()
        {
            context = Substitute.For<ICakeContext>();
        }


        [Test]
        public void DropDatabase_DoesNotExist_DoesNotThrow()
        {
            Action act = () => SqlServerAliases.DropDatabase(context, ConnectionString, "DoesNotExist");

            act.ShouldNotThrow();
        }


        [Test]
        public void DropDatabase_ActuallyDrops()
        {
            //Arrange
            var dbName = "WillBeDropped";
            ExecuteSql($"Create database {dbName}");

            // Act
            SqlServerAliases.DropDatabase(context, ConnectionString, dbName);

            // Assert
            DbExists(dbName).Should().BeFalse();
        }


        [Test]
        public void CreateDatabaseIfNotExists()
        {
            // Act
            var dbName = "CakeTest";
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

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
            SqlServerAliases.DropAndCreateDatabase(context, ConnectionString, dbName);

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
            SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, sql);

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
            SqlServerAliases.ExecuteSqlFile(context, ConnectionString, sqlFilePath);

            // Assert
            TableExists(dbName, tableName1).Should().BeTrue();
            TableExists(dbName, tableName2).Should().BeTrue();
            ExecuteSql($"drop database {dbName}");
        }

        [Test]
        public void Trying_To_Connect_WithBadConnString_ThowsMeaningful_Exception()
        {
            var connString = "data source=(LocalDb)\v12.0";
            Action act = () => SqlServerAliases.CreateDatabaseIfNotExists(context, connString, "ShouldThrow");

            act.ShouldThrow<Exception>()
                .WithMessage(
                    "Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'. It should look like 'var connString = @\"(localDb)\\v12.0\"'");
        }


        [Test]
        public void CreateDatabase_DbAlreadyExists_Throws()
        {
            // Act
            var dbName = "Unknown";
            ExecuteSql($"create database {dbName}");

            // Assert
            Action act = () => SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);
            act.ShouldThrow<SqlException>();

            // Cleanup
            ExecuteSql($"drop database {dbName}");
        }


        [Test]
        public void CreateDatabase_Creates_Correctly()
        {
            // Act
            var dbName = "Unknown";
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);

            // Assert
            DbExists(dbName).Should().BeTrue();

            // Cleanup
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
