using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.SqlServer;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Tests
{
    public class RestoreDatabaseTests
    {
        private const string ConnectionString = @"data source=(localdb)\MSSqlLocalDb";
        private readonly ICakeContext _context;

        public RestoreDatabaseTests()
        {
            _context = Substitute.For<ICakeContext>();
        }

        [Test]
        public void RestoreDatabase_MinimalInformation_DoesNotThrow()
        {
            const string originalDbName = "CakeRestoreTest";
            try
            {
                // Arrange
                var path = GetBackupFilePath();

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, new RestoreSqlBackupSettings(), new List<FilePath> { new FilePath(path) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, originalDbName);
            }
        }

        [Test]
        public void RestoreMultipleDatabase_MinimalInformation_DoesNotThrow()
        {
            const string originalDbName = "CakeRestoreTest";
            try
            {
                // Arrange
                var pathList = GetMultipleBackupFilePaths();
                var differentialPathList = GetMultipleBackupFilePaths("differentialMultiFilesBackup*.bak");

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, new RestoreSqlBackupSettings(), pathList, differentialBackupFiles: differentialPathList);

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, originalDbName);
            }
        }

        [Test]
        public void RestoreDatabase_NoSingleUserModeInformation_DoesNotThrow()
        {
            const string originalDbName = "CakeRestoreTest";
            try
            {
                // Arrange
                var path = GetBackupFilePath();
                var settings = new RestoreSqlBackupSettings { SwitchToUserMode = DbUserMode.MultiUser };

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, settings, new List<FilePath> { new FilePath(path) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, originalDbName);
            }
        }

        [Test]
        public void RestoreMultipleDatabase_NoSingleUserModeInformation_DoesNotThrow()
        {
            const string originalDbName = "CakeRestoreTest";
            try
            {
                // Arrange
                var pathList = GetMultipleBackupFilePaths();
                var differentialPathList = GetMultipleBackupFilePaths("differentialMultiFilesBackup*.bak");
                var settings = new RestoreSqlBackupSettings { SwitchToUserMode = DbUserMode.MultiUser };

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, settings, pathList, differentialBackupFiles: differentialPathList);

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName).Should().BeTrue();
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
            const string databaseName = "NewRandomDatabase";
            try
            {
                // Arrange
                var path = GetBackupFilePath();

                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, new RestoreSqlBackupSettings { NewDatabaseName = databaseName }, new List<FilePath> { new FilePath(path) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, databaseName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, databaseName);
            }
        }

        [Test]
        public void RestoreMultipleDatabase_DatabaseRename_DoesNotThrow()
        {
            const string databaseName = "NewRandomDatabase";
            try
            {
                // Arrange
                var pathList = GetMultipleBackupFilePaths();
                var differentialPathList = GetMultipleBackupFilePaths("differentialMultiFilesBackup*.bak");

                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, new RestoreSqlBackupSettings { NewDatabaseName = databaseName }, pathList, differentialBackupFiles: differentialPathList);

                // Assert
                SqlHelpers.DbExists(ConnectionString, databaseName).Should().BeTrue();
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
            const string newDatabaseName = "RestoredFromTest.Cake";
            try
            {
                // Arrange
                var path = GetBackupFilePath();
                var settings = new RestoreSqlBackupSettings { NewDatabaseName = newDatabaseName, NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()) };

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, settings, new List<FilePath> { new FilePath(path) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, newDatabaseName);
            }
        }

        [Test]
        public void RestoreMultipleDatabase_MoveLocation_DoesNotThrow()
        {
            const string newDatabaseName = "RestoredFromTest.Cake";
            try
            {
                // Arrange
                var pathList = GetMultipleBackupFilePaths();
                var differentialPathList = GetMultipleBackupFilePaths("differentialMultiFilesBackup*.bak");
                var settings = new RestoreSqlBackupSettings { NewDatabaseName = newDatabaseName, NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()) };

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, settings, pathList, differentialBackupFiles: differentialPathList);

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName).Should().BeTrue();
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
            const string originalDbName = "CakeRestoreTest";
            try
            {
                // Arrange
                var path = GetBackupFilePath();

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, new RestoreSqlBackupSettings { WithReplace = true }, new List<FilePath> { new FilePath(path) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, originalDbName);
            }
        }

        [Test]
        public void RestoreMultipleDatabase_WithReplace_DoesNotThrow()
        {
            const string originalDbName = "CakeRestoreTest";
            try
            {
                // Arrange
                var pathList = GetMultipleBackupFilePaths();
                var differentialPathList = GetMultipleBackupFilePaths("differentialMultiFilesBackup*.bak");

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, new RestoreSqlBackupSettings { WithReplace = true }, pathList, differentialBackupFiles: differentialPathList);

                // Assert
                SqlHelpers.DbExists(ConnectionString, originalDbName).Should().BeTrue();
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
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(_context, ConnectionString))
            {
                // Act
                var path = RestoreSqlBackupImpl.GetDefaultLogPath(connection);

                // Assert
                path.Should().NotBeNullOrEmpty();
                Directory.Exists(path).Should().BeTrue();
            }
        }

        [Test]
        public void Can_Read_DefaultDataPath()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(_context, ConnectionString))
            {
                // Act
                var path = RestoreSqlBackupImpl.GetDefaultDataPath(connection);

                // Assert
                path.Should().NotBeNullOrEmpty();
                Directory.Exists(path).Should().BeTrue();
            }
        }

        [Test]
        public void Can_Read_LogicalNames()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(_context, ConnectionString))
            {
                // Arrange
                var path = GetBackupFilePath("multiFilesBackup1.bak");

                // Act
                var names = RestoreSqlBackupImpl.GetLogicalNames(path, connection);

                // Assert
                names.Should().HaveCount(2);
            }
        }

        [Test]
        public void GetDatabaseName_Should_ReturnName()
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(_context, ConnectionString))
            {
                // Arrange
                var path = GetBackupFilePath("multiFilesBackup1.bak");

                // Act
                var names = RestoreSqlBackupImpl.GetDatabaseName(path, connection);

                // Assert
                names.Should().Be("CakeRestoreTest");
            }
        }

        [Test]
        public void RestoreDatabase_AllOptionsToggled_DoesNotThrow()
        {
            const string newDatabaseName = "RestoredFromTest.Cake";
            try
            {
                // Arrange
                var path = GetBackupFilePath();
                var settings = new RestoreSqlBackupSettings
                {
                    NewDatabaseName = newDatabaseName,
                    NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()),
                    WithReplace = true,
                    SwitchToUserMode = DbUserMode.MultiUser,
                };

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, settings, new List<FilePath> { new FilePath(path) });

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, newDatabaseName);
            }
        }

        [Test]
        public void RestoreMultipleDatabase_AllOptionsToggled_DoesNotThrow()
        {
            const string newDatabaseName = "RestoredFromTest.Cake";
            try
            {
                // Arrange
                var pathList = GetMultipleBackupFilePaths();
                var differentialPathList = GetMultipleBackupFilePaths("differentialMultiFilesBackup*.bak");
                var settings = new RestoreSqlBackupSettings
                {
                    NewDatabaseName = newDatabaseName,
                    NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()),
                    WithReplace = true,
                    SwitchToUserMode = DbUserMode.MultiUser,
                    BackupSetFile = 1,
                    DifferentialBackupSetFile = 1,
                };

                // Act
                RestoreSqlBackupImpl.RestoreSqlBackup(_context, ConnectionString, settings, pathList, differentialBackupFiles: differentialPathList);

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, newDatabaseName);
            }
        }

        private static string? GetBackupFilePath(string filename = "multiFileBackup.bak")
        {
            var testDataDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "TestData");
            return Directory.GetFiles(testDataDirectory, filename, SearchOption.AllDirectories).FirstOrDefault();
        }

        private static IList<FilePath> GetMultipleBackupFilePaths(string searchPattern = "multiFilesBackup*.bak")
        {
            var testDataDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "TestData");
            var fileList = Directory.GetFiles(testDataDirectory, searchPattern, SearchOption.AllDirectories);
            return fileList.Select(path => new FilePath(path)).ToList();
        }
    }
}
