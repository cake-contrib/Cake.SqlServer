#if NET451
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.SqlServer.Dac;

namespace Cake.SqlServer
{

    internal static class SqlBacpacImpl
    {
        public static void CreateBacpacFile(ICakeContext context, string connectionString, string databaseName, string resultingFilePath)
        {
            context.Log.Information($"About to create a bacpac file from database {databaseName}");

            var service = new DacServices(connectionString);

            service.ExportBacpac(resultingFilePath, databaseName);

            context.Log.Information($"Finished creating bacpac file from database {databaseName}. File location is {resultingFilePath}");
        }


        public static void RestoreBacpac(ICakeContext context, string connectionString, string newDatabaseName, string bacpacFilePath)
        {
            context.Log.Information($"About to restore bacpac from {bacpacFilePath} into database {newDatabaseName}");

            var bacPackage = BacPackage.Load(bacpacFilePath);

            context.Log.Debug($"Loaded bacpac file {bacPackage}");

            var service = new DacServices(connectionString);

            service.ImportBacpac(bacPackage, newDatabaseName);

            context.Log.Information($"Finished restoring bacpac file into database {newDatabaseName}");
        }
    }
}
#endif
