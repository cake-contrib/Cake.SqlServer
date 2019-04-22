using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

[assembly: CakeNamespaceImport("System.Data.SqlClient")]
[assembly: CakeNamespaceImport("Cake.SqlServer")]

namespace Cake.SqlServer
{
    /// <summary>
    /// Contains functionality to deal with DAC functionality from SQL Server: create and restore BACPAC files; 
    /// create and restore DACPAC files
    /// </summary>
    [CakeAliasCategory("SqlServer")]
    public static class DacAliases
    {
        /// <summary>
        /// Creates a bacpac file for easy database backuping. 
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="databaseName">Name of the database you'd like to create a bacpac from</param>
        /// <param name="resultingFilePath">Full path where you'd like to store resulting bacpac</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Bacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(localdb)\MSSqlLocalDb";
        ///     
        ///     		var dbName = "ForBacpac";
        ///     
        ///     		CreateDatabase(connString, dbName);
        ///     
        ///     		CreateBacpacFile(connString, dbName, new FilePath(@".\ForBacpac.bacpac"));
        ///     	});
        ///     });        
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CreateBacpacFile(this ICakeContext context, String connectionString, String databaseName, FilePath resultingFilePath)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(resultingFilePath, nameof(resultingFilePath));

            SqlBacpacImpl.CreateBacpacFile(context, connectionString, databaseName, resultingFilePath.FullPath);
        }


        /// <summary>
        /// Restores a bacpac file into a database.
        /// <para>
        /// NB: there must be no database with the name you provide. Otherwise exception will be thrown.
        /// </para>
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="databaseName">Name of a new database you are creating </param>
        /// <param name="bacpacFilePath">Full path to the bacpac file</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Bacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(localdb)\MSSqlLocalDb";
        ///     
        ///     		var dbName = "FromBacpac";
        ///     
        ///     		var file = new FilePath(@".\src\Tests\Nsaga.bacpac");
        ///     
        ///     		RestoreBacpac(connString, dbName, file);
        ///     	});
        ///     });        
        /// </code>
        /// </example>        
        [CakeMethodAlias]
        public static void RestoreBacpac(this ICakeContext context, String connectionString, String databaseName, FilePath bacpacFilePath)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(bacpacFilePath, nameof(bacpacFilePath));

            SqlBacpacImpl.RestoreBacpac(context, connectionString, databaseName, bacpacFilePath.FullPath);
        }


        /// <summary>
        /// Extracts a dacpac file to a database package. 
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="targetDatabaseName">Name of the database you'd like to extract a package from</param>
        /// <param name="settings">Custom setting for the extract operation.</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Extract-Dacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(localdb)\MSSqlLocalDb";
        ///     
        ///     		var dbName = "ForDacpac";
        ///     
        ///     		CreateDatabase(connString, dbName);
        /// 
        ///     		var settings = new ExtractDacpacSettings("MyAppName", "2.0.0.0") { 
        ///     			OutputFile = new FilePath(@".\Nsaga.dacpac")
        /// 			};
        ///     
        ///     		ExtractDacpacFile(connString, dbName, settings);
        ///     	});
        ///     });        
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void ExtractDacpacFile(this ICakeContext context, String connectionString, String targetDatabaseName, ExtractDacpacSettings settings)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(targetDatabaseName, nameof(targetDatabaseName));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            SqlDacpacImpl.ExtractDacpacFile(context, connectionString, targetDatabaseName, settings);
        }


        /// <summary>
        /// Publish a dacpac file to a database.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="connectionString">The connection string. You may want to connect to master database for this operation.</param>
        /// <param name="targetDatabaseName">Name of a target database.</param>
        /// <param name="dacpacFilePath">Full path to the dacpac file.</param>
        /// <param name="settings">Configure the sql deployment</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-Bacpac")
        ///     	.Does(() =>{
        ///     		var connString = @"data source=(localdb)\MSSqlLocalDb";
        ///     
        ///     		var dbName = "ForDacpac";
        ///     
        ///     		var file = new FilePath(@".\src\Tests\Nsaga.dacpac");
        ///     
        ///     		var settings = new PublishDacpacSettings { 
        ///     			GenerateDeploymentScript = true
        /// 			};
        /// 
        ///     		PublishDacpacFile(connString, dbName, file, settings);
        ///     	});
        ///     });        
        /// </code>
        /// </example>        
        [CakeMethodAlias]
        public static void PublishDacpacFile(this ICakeContext context, String connectionString, String targetDatabaseName, FilePath dacpacFilePath, PublishDacpacSettings settings = null)
        {
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(dacpacFilePath, nameof(dacpacFilePath));

            SqlDacpacImpl.PublishDacpacFile(context, connectionString, targetDatabaseName, dacpacFilePath.FullPath, settings);
        }
    }
}
