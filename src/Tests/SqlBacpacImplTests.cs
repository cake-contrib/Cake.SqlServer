using System;
using System.IO;
using System.Linq;
using Cake.Core;
using NUnit.Framework;
using Cake.SqlServer;
using FluentAssertions;
using NSubstitute;


namespace Tests
{
    public class SqlBacpacImplTests
    {
        private const String ConnectionString = @"data source=(LocalDb)\v12.0";
        private readonly ICakeContext context;


        public SqlBacpacImplTests()
        {
            context = Substitute.For<ICakeContext>();
        }

        [Test]
        public void Try_Restore_Bacpac()
        {
            try
            {
                SqlBacpacImpl.RestoreBacpac(context, ConnectionString, "NSaga", GetBacpacFilePath());
                SqlHelpers.DbExists(ConnectionString, "Nsaga").Should().BeTrue();
            }
            finally
            {
                SqlHelpers.DropDatabase(ConnectionString, "Nsaga");
            }
        }



        [Test]
        public void Try_Create_Bacpac()
        {
            var dbName = "BacpacTestDb";
            var resultingFilePath = "NsagaCreated.bacpac";
            try
            {
                // Arrange
                SqlHelpers.ExecuteSql(ConnectionString, $"Create database {dbName}");
                var sql = $@"use {dbName}
                GO
                create table [{dbName}].[dbo].[Table1] (id int null, name nvarchar(max) null);
                Go
                insert into [{dbName}].[dbo].[Table1] (id, name) values (42, 'Ultimate Question of Life');
                Go";
                context.ExecuteSqlCommand(ConnectionString, sql);

                // Act
                SqlBacpacImpl.CreateBacpacFile(context, ConnectionString, dbName, resultingFilePath);

                // Assert
                File.Exists(resultingFilePath).Should().BeTrue();
            }
            finally
            {
                SqlHelpers.DropDatabase(ConnectionString, dbName);
                File.Delete(resultingFilePath);
            }
        }




        private static string GetBacpacFilePath()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Nsaga.bacpac", SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
