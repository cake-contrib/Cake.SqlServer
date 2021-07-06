using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.SqlServer.Dac;

namespace Cake.SqlServer
{
    internal static class SqlBacpacImpl
    {
        public static void CreateBacpacFile(ICakeContext context, string connectionString, string databaseName, string resultingFilePath)
        {
            Initializer.InitializeNativeSearchPath();
            context.Log.Information($"About to create a bacpac file from database {databaseName}");

            var service = new DacServices(connectionString);

            service.ExportBacpac(resultingFilePath, databaseName);

            context.Log.Information($"Finished creating bacpac file from database {databaseName}. File location is {resultingFilePath}");
        }


        public static void RestoreBacpac(ICakeContext context, string connectionString, string newDatabaseName, string? bacpacFilePath)
        {
            if (string.IsNullOrEmpty(bacpacFilePath))
            {
                throw new ArgumentNullException(nameof(bacpacFilePath));
            }
            Initializer.InitializeNativeSearchPath();

            context.Log.Information($"About to restore bacpac from {bacpacFilePath} into database {newDatabaseName}");

            using (var bacPackage = BacPackage.Load(bacpacFilePath))
            {

                context.Log.Debug($"Loaded bacpac file {bacpacFilePath}");

                var service = new DacServices(connectionString);

                service.ImportBacpac(bacPackage, newDatabaseName);
            }

            context.Log.Information($"Finished restoring bacpac file into database {newDatabaseName}");
        }
    }
}
