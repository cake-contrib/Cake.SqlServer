using System;
using System.Data.SqlClient;
using Cake.Core;
using Cake.SqlServer;
using NUnit.Framework;

// ReSharper disable InvokeAsExtensionMethod
namespace Tests
{
    public class DatabaseExtensionTests
    {
        [TestCase(@"data source=(LocalDb)\v12.0")]
        [TestCase(@"data source=.\SQLEXPRESS;integrated security=SSPI;Initial Catalog=master;")]
        public void CreateDatabaseIfNotExists(String connectionString)
        {
            //Arrange
            var context = NSubstitute.Substitute.For<ICakeContext>();
            
            // Act
            DatabaseExtension.CreateDatabaseIfNotExists(context, connectionString, "CakeTest");

            // Assert
            using (var connection = new SqlConnection(connectionString))
            {
                var checkDbSql = $"select DB_ID('CakeTest')";
                connection.Open();
                var command = new SqlCommand(checkDbSql, connection);

                var reader = command.ExecuteReader();

                Assert.True(reader.HasRows);
            }
        }
    }
}
