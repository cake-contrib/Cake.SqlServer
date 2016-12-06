using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.SqlServer;
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
        public void Can_Restore_Database()
        {
            try
            {
                //Arrange
                var path = GetBackupFilePath();
                var newDatabaseName = "NewCakeTest";

                // Act
                SqlServerAliasesImpl.RestoreDatabase(context, ConnectionString, newDatabaseName, new FilePath(path), new DirectoryPath(System.IO.Path.GetTempPath()));

                // Assert
                SqlHelpers.DbExists(ConnectionString, newDatabaseName);
            }
            finally
            {
                // Cleanup
                SqlHelpers.DropDatabase(ConnectionString, "NewCakeTest");
            }
        }

        private static string GetBackupFilePath()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "CakeRestoreTest.bak", SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
