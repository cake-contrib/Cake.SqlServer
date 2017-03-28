using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.SqlServer.Dac;

namespace Cake.SqlServer
{
    internal static class SqlDacpacImpl
    {
        /// <summary>
        /// Extract the schema from a database into a package.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="targetDatabaseName">The target database.</param>
        /// <param name="settings">The settings.</param>
        public static void ExtractDacpacFile(ICakeContext context, string connectionString, string targetDatabaseName, ExtractDacpacSettings settings)
        {
            context.Log.Information($"About to extract a dacpac file from database {targetDatabaseName}");

            var service = new DacServices(connectionString);

            service.Extract(settings.OutputFile.FullPath, targetDatabaseName, settings.Name, settings.Version, settings.Description, settings.Tables);

            context.Log.Information($"Finished creating dacpac file from database {targetDatabaseName}. File location is {settings.OutputFile}");
        }

        /// <summary>
        /// Extract the schema from a database into a package.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="targetDatabaseName">The target database.</param>
        /// <param name="dacpacFilePath">Path to the dac file.</param>
        /// <param name="config">Configure the sql deployment</param>
        public static void PublishDacpacFile(ICakeContext context, string connectionString, string targetDatabaseName, string dacpacFilePath, Action<PublishOptions> config = null)
        {
            context.Log.Information($"About to restore bacpac from {dacpacFilePath} into database {targetDatabaseName}");

            var dacPackage = DacPackage.Load(dacpacFilePath);

            context.Log.Debug($"Loaded dacpac file {dacPackage}");

            var service = new DacServices(connectionString);

            var options = new PublishOptions {DeployOptions = new DacDeployOptions()};
            config?.Invoke(options);

            service.Publish(dacPackage, targetDatabaseName, options);

            context.Log.Information($"Finished restoring dacpac file into database {targetDatabaseName}");
        }
    }
}