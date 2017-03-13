using NUnit.Framework;
using System;
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
                SqlBackupsImpl.RestoreSqlBackup(context, ConnectionString, new FilePath(path), new RestoreSqlBackupSettings());

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
        public void RestoreDatabase_DatabaseRename_DoesNotThrow()
        {
            var databaseName = "NewRandomDatabase";
            try
            {
                //Arrange
                var path = GetBackupFilePath();

                SqlBackupsImpl.RestoreSqlBackup(context, ConnectionString, new FilePath(path), new RestoreSqlBackupSettings() { NewDatabaseName = databaseName });

                // Assert
                SqlHelpers.DbExists(ConnectionString, databaseName);
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, databaseName);
            }
        }


        [Test]
        public void RestoreDatabase_MoveLocation_DoesNotThrow()
        {
            var newDatabaseName = "RestoredFromTest.Cake";
            try
            {
                //Arrange
                var path = GetBackupFilePath();

                // Act
                SqlBackupsImpl.RestoreSqlBackup(context, ConnectionString, new FilePath(path), new RestoreSqlBackupSettings() { NewDatabaseName = newDatabaseName, NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName);
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, newDatabaseName);
            }
        }

        [Test]
        public void RestoreDatabase_WithReplace_DoesNotThrow()
        {
            var originalDbName = "CakeRestoreTest";
            try
            {
                //Arrange
                var path = GetBackupFilePath();

                // Act
                SqlBackupsImpl.RestoreSqlBackup(context, ConnectionString, new FilePath(path), new RestoreSqlBackupSettings() { WithReplace = true });

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
        public void Can_Read_DefaultLogPath()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, ConnectionString))
            {
                // Act
                var path = SqlBackupsImpl.GetDefaultLogPath(connection);

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
                var path = SqlBackupsImpl.GetDefaultDataPath(connection);

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
                var names = SqlBackupsImpl.GetLogicalNames(path, connection);

                // Assert
                names.Should().HaveCount(3);
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
                var names = SqlBackupsImpl.GetDatabaseName(path, connection);

                // Assert
                names.Should().Be("CakeRestoreTest");
            }
        }

        private static string GetBackupFilePath(String filename = "multiFileBackup.bak")
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), filename, SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
