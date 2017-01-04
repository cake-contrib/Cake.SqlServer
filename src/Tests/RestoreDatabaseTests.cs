using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.SqlServer;
using FluentAssertions;
using NSubstitute;

namespace Tests
{
    public class RestoreDatabaseTests
    {
        private const String ConnectionString = @"data source=(LocalDb)\v12.0";
        private readonly ICakeContext context;

        public RestoreDatabaseTests()
        {
            context = Substitute.For<ICakeContext>();
        }

        [Test]
        public void RestoreDatabase_MinimalInformation_DoesNotThrow()
        {
            var originalDbName = "CakeRestoreTest";
            try
            {
                //Arrange
                var path = GetBackupFilePath();

                // Act
                RestoreDatabaseImpl.RestoreDatabase(context, ConnectionString, new FilePath(path));

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName);
            }
            finally
            {
                // Cleanup
                //SqlHelpers.DropDatabase(ConnectionString, originalDbName);
            }
        }

        [Test]
        public void RestoreDatabase_MultiLogDatabase_DoesNotThrow()
        {
            var originalDbName = "CakeRestoreTest";
            try
            {
                //Arrange
                var path = GetBackupFilePath("multiFileBackup.bak");

                RestoreDatabaseImpl.RestoreDatabase(context, ConnectionString, new FilePath(path));

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName);
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, originalDbName);
            }
        }


        [Test]
        public void Can_Restore_Database()
        {
            try
            {
                //Arrange
                var path = GetBackupFilePath();
                var newDatabaseName = "NewCakeTest";

                // Act
                RestoreDatabaseImpl.RestoreDatabase(context, ConnectionString, new FilePath(path), newDatabaseName, new DirectoryPath(System.IO.Path.GetTempPath()));

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName);
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, "NewCakeTest");
            }
        }

        [Test]
        public void Can_Read_DefaultLogPath()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, ConnectionString))
            {
                // Act
                var path = RestoreDatabaseImpl.GetDefaultLogPath(connection);

                // Assert
                path.Should().NotBeNullOrEmpty();
                Directory.Exists(path).Should().BeTrue();
            }
        }

        [Test]
        public void Can_Read_DefaultDataPath()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, ConnectionString))
            {
                // Act
                var path = RestoreDatabaseImpl.GetDefaultDataPath(connection);

                // Assert
                path.Should().NotBeNullOrEmpty();
                Directory.Exists(path).Should().BeTrue();
            }
        }


        [Test]
        public void Can_Read_LogicalNames()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, ConnectionString))
            {
                //Arrange
                var path = GetBackupFilePath();

                // Act
                var names = RestoreDatabaseImpl.GetLogicalNames(path, connection);

                // Assert
                names.Should().HaveCount(2);
            }
        }

        [Test]
        public void GetDatabaseName_Should_ReturnName()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, ConnectionString))
            {
                //Arrange
                var path = GetBackupFilePath();

                // Act
                var names = RestoreDatabaseImpl.GetDatabaseName(path, connection);

                // Assert
                names.Should().Be("CakeRestoreTest");
            }
        }

        private static string GetBackupFilePath(String filename = "CakeRestoreTest.bak")
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, filename, SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
