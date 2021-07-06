﻿using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.SqlServer;
using FluentAssertions;
using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public sealed class SqlServerAliasesTests : IDisposable
    {
        private const string ConnectionString = @"data source=(localdb)\MSSqlLocalDb";
        private readonly ICakeContext context;

        public SqlServerAliasesTests()
        {
            context = Substitute.For<ICakeContext>();
        }

        [Test]
        public void DatabaseExists_DoesNotExist_ReturnFalse()
        {
            var dbExists = SqlServerAliases.DatabaseExists(context, ConnectionString, "DoesNotExist");
            dbExists.Should().BeFalse();
        }

        [Test]
        public void DatabaseExists_DoesExist_ReturnTrue()
        {
            // Arrange
            const string dbName = "WillExists";
            SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");

            // Act
            var dbExists = SqlServerAliases.DatabaseExists(context, ConnectionString, dbName);

            // Assert
            dbExists.Should().BeTrue();
        }

        [Test]
        public void DropDatabase_DoesNotExist_DoesNotThrow()
        {
            Action act = () => SqlServerAliases.DropDatabase(context, ConnectionString, "DoesNotExist");

            act.Should().NotThrow();
        }

        [Test]
        public void DropDatabase_ActuallyDrops()
        {
            //Arrange
            const string dbName = "WillBeDropped";
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
            const string dbName = "CakeTest";
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void CreateDatabaseIfNotExists_WithPrimaryFile()
        {
            // Act
            const string dbName = "CakeTest";
            var mdfFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.mdf");
            var createSettings = new CreateDatabaseSettings().WithPrimaryFile(mdfFilePath);
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName, createSettings);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();
            File.Exists(mdfFilePath).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void CreateDatabaseIfNotExists_WithPrimaryAndLogFile()
        {
            // Act
            const string dbName = "CakeTest";
            var mdfFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.mdf");
            var logFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.ldf");
            var createSettings = new CreateDatabaseSettings().WithPrimaryFile(mdfFilePath).WithLogFile(logFilePath);
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName, createSettings);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();
            File.Exists(mdfFilePath).Should().BeTrue();
            File.Exists(logFilePath).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void DropAndCreateDatabase_Always_RemovesTable()
        {
            //Arrange
            const string dbName = "ToBeRecreated";
            const string tableName = "WillNotExist";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");
            SqlHelpers.ExecuteSql(ConnectionString, $"create table [{dbName}].dbo.{tableName} (id int null)");
            SqlHelpers.TableExists(ConnectionString, dbName, tableName).Should().BeTrue();

            // Act
            SqlServerAliases.DropAndCreateDatabase(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.TableExists(ConnectionString, dbName, tableName).Should().BeFalse();
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void DropAndCreateDatabase_WithCreateParams()
        {
            //Arrange
            const string dbName = "ToBeRecreated";
            const string tableName = "WillNotExist";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");
            SqlHelpers.ExecuteSql(ConnectionString, $"create table [{dbName}].dbo.{tableName} (id int null)");
            SqlHelpers.TableExists(ConnectionString, dbName, tableName).Should().BeTrue();
            var mdfFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.mdf");
            var logFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.ldf");
            var createSettings = new CreateDatabaseSettings().WithPrimaryFile(mdfFilePath).WithLogFile(logFilePath);

            // Act
            SqlServerAliases.DropAndCreateDatabase(context, ConnectionString, dbName, createSettings);

            // Assert
            File.Exists(mdfFilePath).Should().BeTrue();
            File.Exists(logFilePath).Should().BeTrue();
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
                using (var monitoringSubject = connection.Monitor())
                {
                    SqlServerAliases.ExecuteSqlCommand(context, connection, "select * from sys.tables");
                    monitoringSubject.Should().NotRaise(nameof(connection.StateChange));
                }
            }
        }

        [Test]
        public void ExecuteSqlFile_does_not_change_connection_state()
        {
            const string dbName = "ForFileExecution";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");
            try
            {
                using (var connection = SqlServerAliases.OpenSqlConnection(context, ConnectionString))
                using (var monitoringSubject = connection.Monitor())
                {
                    SqlServerAliases.ExecuteSqlFile(context, connection, GetSqlFilePath());
                    monitoringSubject.Should().NotRaise(nameof(connection.StateChange));
                }
            }
            finally
            {
                SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
            }
        }

        [Test]
        public void ExecuteSqlCommand_CreatesTables()
        {
            //Arrange
            const string dbName = "ForSqlExecution";
            const string tableName1 = "WillExist1";
            const string tableName2 = "WillExist2";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");
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
            const string connectionString = @"data source=(localdb)\MSSqlLocalDb;Database=ForFileExecution";
            const string dbName = "ForFileExecution";
            const string tableName1 = "WillExist1";
            const string tableName2 = "WillExist2";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");
            var sqlFilePath = GetSqlFilePath();

            // Act
            SqlServerAliases.ExecuteSqlFile(context, connectionString, sqlFilePath);

            // Assert
            SqlHelpers.TableExists(ConnectionString, dbName, tableName1).Should().BeTrue();
            SqlHelpers.TableExists(ConnectionString, dbName, tableName2).Should().BeTrue();
            SqlServerAliases.DropDatabase(context, ConnectionString, dbName);
        }

        private static string? GetSqlFilePath()
        {
            var testDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "TestData");
            return Directory.GetFiles(testDataDirectory, "Script.sql", SearchOption.AllDirectories).FirstOrDefault();
        }

        [Test]
        public void Trying_To_Connect_WithBadConnString_ThrowsMeaningful_Exception()
        {
            const string connString = "data source=(localdb)\v12.0";
            Action act = () => SqlServerAliases.CreateDatabaseIfNotExists(context, connString, "ShouldThrow");

            act.Should().Throw<Exception>()
               .WithMessage("Looks like you are trying to connect to LocalDb. Have you correctly escaped your connection string with '@'? It should look like 'var connString = @\"(localDb)\\v12.0\"'");
        }

        [Test]
        public void CreateDatabase_DbAlreadyExists_Throws()
        {
            // Act
            const string dbName = "Unknown";
            SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");

            // Assert
            Action act = () => SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);
            act.Should().Throw<SqlException>();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void CreateDatabase_Creates_Correctly()
        {
            // Act
            const string dbName = "Unknown";
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
            const string dbName = "ToBeRecreatedAgain";
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName);
            SqlServerAliases.CreateDatabaseIfNotExists(context, ConnectionString, dbName);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void CreateDatabase_WithPrimaryFile()
        {
            // Act
            const string dbName = "CakeTest";
            var mdfFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.mdf");
            var createSettings = new CreateDatabaseSettings().WithPrimaryFile(mdfFilePath);
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName, createSettings);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();
            File.Exists(mdfFilePath).Should().BeTrue();

            // Cleanup
            SqlHelpers.ExecuteSql(ConnectionString, $"drop database {dbName}");
        }

        [Test]
        public void CreateDatabase_WithPrimaryAndLogFile()
        {
            // Act
            const string dbName = "CakeTest";
            var mdfFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.mdf");
            var logFilePath = Path.Combine(Path.GetTempPath(), "MyCakeTest.ldf");
            var createSettings = new CreateDatabaseSettings().WithPrimaryFile(mdfFilePath).WithLogFile(logFilePath);
            SqlServerAliases.CreateDatabase(context, ConnectionString, dbName, createSettings);

            // Assert
            SqlHelpers.DbExists(ConnectionString, dbName).Should().BeTrue();
            File.Exists(mdfFilePath).Should().BeTrue();
            File.Exists(logFilePath).Should().BeTrue();

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
#pragma warning disable CC0004 // Catch block cannot be empty
            catch
            {
                // do nothing
            }
#pragma warning restore CC0004 // Catch block cannot be empty

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
#pragma warning disable CC0004 // Catch block cannot be empty
            catch
            {
                // do nothing
            }
#pragma warning restore CC0004 // Catch block cannot be empty

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

            act.Should().Throw<Exception>();
        }

        [Test]
        public void SetCommandTimeout_LongTimeout_DoesNotThrow()
        {
            SqlServerAliases.SetSqlCommandTimeout(context, 3);

            Action act = () => SqlServerAliases.ExecuteSqlCommand(context, ConnectionString, "WAITFOR DELAY '00:00:02'");

            act.Should().NotThrow();
        }

        private class SqlObject
        {
            public int? Id { get; set; }
        }

        public void Dispose()
        {
            SqlHelpers.DropDatabase(ConnectionString, "ForFileExecution");
            SqlHelpers.DropDatabase(ConnectionString, "WillBeDropped");
            SqlHelpers.DropDatabase(ConnectionString, "WillExists");
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
