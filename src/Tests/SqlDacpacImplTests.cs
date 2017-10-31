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
    public class SqlDacpacImplTests
    {
        private const String ConnectionString = @"data source=(LocalDb)\v12.0";
        private readonly ICakeContext context;


        public SqlDacpacImplTests()
        {
            context = Substitute.For<ICakeContext>();
        }

        [Test]
        public void Try_Publish_Dacpac()
        {
            var dbName = "DacpacTestDb";
            try
            {
                // Arrange 
                SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");

                // Act
                SqlDacpacImpl.PublishDacpacFile(context, ConnectionString, dbName, GetDacpacFilePath());

                // Assert
                SqlHelpers.TableExists(ConnectionString, dbName, "Table1").Should().BeTrue();
            }
            finally
            {
                SqlHelpers.DropDatabase(ConnectionString, dbName);
            }
        }



        [Test]
        public void Try_Extract_Dacpac()
        {
            var dbName = "DacpacTestDb";
            var resultingFilePath = "NsagaCreated.dacpac";
            var settings = new ExtractDacpacSettings("TestApp", "1.0.0.0") { OutputFile = resultingFilePath };
            try
            {
                // Arrange
                SqlHelpers.ExecuteSql(ConnectionString, $"create database {dbName}");
                var sql = $@"use {dbName}
                GO
                create table [{dbName}].[dbo].[Table1] (id int null, name nvarchar(max) null);
                Go";
                context.ExecuteSqlCommand(ConnectionString, sql);

                // Act
                SqlDacpacImpl.ExtractDacpacFile(context, ConnectionString, dbName, settings);

                // Assert
                File.Exists(resultingFilePath).Should().BeTrue();
            }
            finally
            {
                SqlHelpers.DropDatabase(ConnectionString, dbName);
                File.Delete(resultingFilePath);
            }
        }

        [Test]
        public void GetPublishOptions_MapsDictionary_Correctly()
        {
            //Arrange
            var publishSettings = new PublishDacpacSettings();
            publishSettings.SqlCommandVariableValues["Hello"] = "World";
            publishSettings.SqlCommandVariableValues["31Oct"] = "25Dec";

            // Act
            var publishOptions = SqlDacpacImpl.GetPublishOptions(publishSettings);

            // Assert
            var result = publishOptions.DeployOptions.SqlCommandVariableValues;
            result["Hello"].Should().Be("World");
            result["31Oct"].Should().Be("25Dec");
        }

        private static string GetDacpacFilePath()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Nsaga.dacpac", SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
