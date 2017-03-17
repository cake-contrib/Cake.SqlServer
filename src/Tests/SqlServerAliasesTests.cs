using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.SqlServer;
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
            SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");

            // Act
            SqlServerAliases.DropDatabase(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeFalse();
        }


        [Test]
        public void CreateDatabaseIfNotExists()
        {
            // Act
            var dbName = "CakeTest";
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }



        [Test]
        public void DropAndCreateDatabase_Always_RemovesTable()
        {
            //Arrange
            var dbName = "ToBeRecreated";
            var tableName = "WillNotExist";
            SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");
            SqlHelpers.ExecuteSql(ConnectionString, $"create table [{dbName}].dbo.{tableName} (id int null)");
            SqlHelpers.TableExists(ConnectionString, dbName, tableName).Should().BeTrue();

            // Act
            SqlServerAliases.DropAndCreateDatabase(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.TableExists(ConnectionString, dbName, tableName).Should().BeFalse();
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
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
            SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");
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
                SqlHelpers.ExecuteSql(ConnectionString, $"Drop database {dbName}");
            }
        }



        [Test]
        public void ExecuteSqlCommand_CreatesTables()
        {
            //Arrange
            var dbName = "ForSqlExecution";
            var tableName1 = "WillExist1";
            var tableName2 = "WillExist2";
            SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");
            var sql = $@"
            create table [{dbName}].dbo.{tableName1} (id int null);
            Go
            create table [{dbName}].dbo.{tableName2} (id int null);
            Go
            ";

            // Act
            SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, sql);

            // Assert
            SqlHelpers.TableExists(ConnectionString, dbName, tableName1).Should().BeTrue();
            SqlHelpers.TableExists(ConnectionString, dbName, tableName2).Should().BeTrue();
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }



        [Test]
        public void ExecuteSqlFile_Executes_Successfuly()
        {
            //Arrange
            var connectionString = @"data source=(LocalDb)\v12.0;Database=ForFileExecution";
            var dbName = "ForFileExecution";
            var tableName1 = "WillExist1";
            var tableName2 = "WillExist2";
            SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");
            var sqlFilePath = GetSqlFilePath();

            // Act
            SqlServerAliases.ExecuteSqlFile(context, connectionString, sqlFilePath);

            // Assert
            SqlHelpers.TableExists(ConnectionString, dbName, tableName1).Should().BeTrue();
            SqlHelpers.TableExists(ConnectionString, dbName, tableName2).Should().BeTrue();
            SqlServerAliases.DropDatabase(context, ConnectionString, dbName);
        }

        private static string GetSqlFilePath()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), "Script.sql", SearchOption.AllDirectories).FirstOrDefault();
        }

        [Test]
        public void Trying_To_Connect_WithBadConnString_ThowsMeaningful_Exception()
        {
            var connString = "data source=(LocalDb)\v12.0";
            Action act = () => SqlServerAliases.CreateDatabaseIfNotExists(context, connString, "ShouldThrow");

            act.ShouldThrow<Exception>()
               .WithMessage("Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'? It should look like 'var connString = @\"(localDb)\\v12.0\"'");
        }


        [Test]
        public void CreateDatabase_DbAlreadyExists_Throws()
        {
            // Act
            var dbName = "Unknown";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");

            // Assert
            Action act = () => SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);
            act.ShouldThrow<SqlException>();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }


        [Test]
        public void CreateDatabase_Creates_Correctly()
        {
            // Act
            var dbName = "Unknown";
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void CreateDatabaseIfNotExists_DbExists_DoesNotThrow()
        {
            // Act
            var dbName = "ToBeRecreatedAgain";
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }


        [Test]
        public void CreateDatabase_SqlNameInjection_DoesNotInject()
        {
            // Act
            SqlServerAliases.CreateDatabase(context, ConnectionString, "test] create database hack--");

            // Assert
            SqlHelpers.DbExists(ConnectionString, "test").Should().BeFalse();
            SqlHelpers.DbExists(ConnectionString, "hack").Should().BeFalse();
            SqlHelpers.DbExists(ConnectionString, "test] create database hack--").Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, "if (select DB_ID('test')) is not null drop database [test]");
            SqlHelpers.ExecuteSql(ConnectionString, "if (select DB_ID('hack')) is not null drop database [hack]");
            SqlHelpers.ExecuteSql(ConnectionString, "if (select DB_ID('test] create database hack--')) is not null drop database [test]] create database hack--]");
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
            SqlHelpers.DbExists(ConnectionString, "hack").Should().BeFalse();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, "if (select DB_ID('hack')) is not null drop database [hack]");
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
            SqlHelpers.DbExists(ConnectionString, "hack").Should().BeFalse();
            SqlHelpers.DbExists(ConnectionString, "some'')) is null create database hack--").Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, "if (select DB_ID('some'')) is null create database hack--')) is not null drop database [some')) is null create database hack--]");
        }

        [Test]
        public void SetCommandTimeout_ShortTimeout_Throws()
        {
            SqlServerAliases.SetSqlCommandTimeout(context, 1);

            Action act = () => SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, "WAITFOR DELAY '00:00:02'");

            act.ShouldThrow<Exception>();
        }

        [Test]
        public void SetCommandTimeout_LongTimeout_DoesNotThrow()
        {
            SqlServerAliases.SetSqlCommandTimeout(context, 3);

            Action act = () => SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, "WAITFOR DELAY '00:00:02'");

            act.ShouldNotThrow();
        }



        private class SqlObject
        {
            public int? Id { get; set; }
        }
        

        public void Dispose()
        {
            SqlHelpers.DropDatabase(ConnectionString, "ForFileExecution");
            SqlHelpers.DropDatabase(ConnectionString, "WillBeDropped");
            SqlHelpers.DropDatabase(ConnectionString, "CakeTest");
            SqlHelpers.DropDatabase(ConnectionString, "ToBeRecreated");
            SqlHelpers.DropDatabase(ConnectionString, "ToBeRecreatedAgain");
            SqlHelpers.DropDatabase(ConnectionString, "ForSqlExecution");
            SqlHelpers.DropDatabase(ConnectionString, "Unknown");
            SqlHelpers.DropDatabase(ConnectionString, "test");
            SqlHelpers.DropDatabase(ConnectionString, "hack");
            SqlHelpers.DropDatabase(ConnectionString, "test]] create database hack--");
            SqlHelpers.ExecuteSql(ConnectionString, "if (select DB_ID('some'')) is null create database hack--')) is not null drop database [some')) is null create database hack--]");
        }
    }
}
