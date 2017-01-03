using System;
using System.Data;
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
    public class SqlServerAliasesTests : IDisposable
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
        public void OpenSqlConnection_returns_open_connection()
        {
            using (var connection = SqlServerAliases.OpenSqlConnection(context, ConnectionString))
            {
                connection.State.Should().Be(ConnectionState.Open);
            }
        }



        [Test]
        public void ExecuteSqlCommand_does_not_change_connection_state()
        {
            using (var connection = SqlServerAliases.OpenSqlConnection(context, ConnectionString))
            {
                connection.MonitorEvents();
                SqlServerAliases.ExecuteSqlCommand(context, connection, "select * from sys.tables");
                connection.ShouldNotRaise(nameof(connection.StateChange));
            }
        }

        [Test]
        public void ExecuteSqlFile_does_not_change_connection_state()
        {
            const string dbName = "ForFileExecution";
            ExecuteSql($"Create database {dbName}");
            try
            {
                using (var connection = SqlServerAliases.OpenSqlConnection(context, ConnectionString))
                {
                    connection.MonitorEvents();
                    SqlServerAliases.ExecuteSqlFile(context, connection, GetSqlFilePath());
                    connection.ShouldNotRaise(nameof(connection.StateChange));
                }
            }
            finally
            {
                ExecuteSql($"Drop database {dbName}");
            }
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
        public void ExecuteSqlFile_Executes_Successfuly()
        {
            //Arrange
            var connectionString = @"data source=(LocalDb)\v12.0;Database=ForFileExecution";
            var dbName = "ForFileExecution";
            var tableName1 = "WillExist1";
            var tableName2 = "WillExist2";
            ExecuteSql($"Create database {dbName}");
            var sqlFilePath = GetSqlFilePath();

            // Act
            SqlServerAliases.ExecuteSqlFile(context, connectionString, sqlFilePath);

            // Assert
            TableExists(dbName, tableName1).Should().BeTrue();
            TableExists(dbName, tableName2).Should().BeTrue();
            SqlServerAliases.DropDatabase(context, ConnectionString, dbName);
        }

        private static string GetSqlFilePath()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Script.sql", SearchOption.AllDirectories).FirstOrDefault();
        }

        [Test]
        public void Trying_To_Connect_WithBadConnString_ThowsMeaningful_Exception()
        {
            var connString = "data source=(LocalDb)\v12.0";
            Action act = () => SqlServerAliases.CreateDatabaseIfNotExists(context, connString, "ShouldThrow");

            act.ShouldThrow<Exception>()
               .WithMessage("Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'. It should look like 'var connString = @\"(localDb)\\v12.0\"'");
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

        [Test]
        public void CreateDatabaseIfNotExists_DbExists_DoesNotThrow()
        {
            // Act
            var dbName = "ToBeRecreatedAgain";
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

            // Assert
            DbExists(dbName).Should().BeTrue();

            // Cleanup
            ExecuteSql($"drop database {dbName}");
        }


        [Test]
        public void CreateDatabase_SqlNameInjection_DoesNotInject()
        {
            // Act
            SqlServerAliases.CreateDatabase(context, ConnectionString, "test] create database hack--");

            // Assert
            DbExists("test").Should().BeFalse();
            DbExists("hack").Should().BeFalse();
            DbExists("test] create database hack--").Should().BeTrue();

            // Cleanup
            ExecuteSql("if (select DB_ID('test')) is not null drop database [test]");
            ExecuteSql("if (select DB_ID('hack')) is not null drop database [hack]");
            ExecuteSql("if (select DB_ID('test] create database hack--')) is not null drop database [test]] create database hack--]");
        }

        [Test]
        public void DropDatabase_LiteralInjection_DoesNotInject()
        {
            // Act
            try
            {
                SqlServerAliases.DropDatabase(context, ConnectionString, "some')) is null create database hack--");
            }
            catch (Exception)
            {
                // do nothing
            }

            // Assert
            DbExists("hack").Should().BeFalse();

            // Cleanup
            ExecuteSql("if (select DB_ID('hack')) is not null drop database [hack]");
        }


        [Test]
        public void CreateDatabaseIfNotExists_LiteralInjection_DoesNotInject()
        {
            // Act
            try
            {
                SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, "some')) is null create database hack--");
            }
            catch (Exception)
            {
                // do nothing
            }

            // Assert
            DbExists("hack").Should().BeFalse();
            DbExists("some'')) is null create database hack--").Should().BeTrue();

            // Cleanup
            ExecuteSql("if (select DB_ID('some'')) is null create database hack--')) is not null drop database [some')) is null create database hack--]");
        }

        [Test]
        public void SetCommandTimeout_ShortTimeout_Throws()
        {
            SqlServerAliases.SetCommandTimeout(context, 1);

            Action act = () => SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, "WAITFOR DELAY '00:00:02'");

            act.ShouldThrow<Exception>();
        }

        [Test]
        public void SetCommandTimeout_LongTimeout_DoesNotThrow()
        {
            SqlServerAliases.SetCommandTimeout(context, 3);

            Action act = () => SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, "WAITFOR DELAY '00:00:02'");

            act.ShouldNotThrow();
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


        private void DropDatabase(string databaseName)
        {
            ExecuteSql($"if (select DB_ID('{databaseName}')) is not null drop database [{databaseName}]");
        }

        private class SqlObject
        {
            public int? Id { get; set; }
        }

        public void Dispose()
        {
            DropDatabase("ForFileExecution");
            DropDatabase("WillBeDropped");
            DropDatabase("CakeTest");
            DropDatabase("ToBeRecreated");
            DropDatabase("ToBeRecreatedAgain");
            DropDatabase("ForSqlExecution");
            DropDatabase("Unknown");
            DropDatabase("test");
            DropDatabase("hack");
            DropDatabase("test]] create database hack--");
            ExecuteSql("if (select DB_ID('some'')) is null create database hack--')) is not null drop database [some')) is null create database hack--]");
        }
    }
}
