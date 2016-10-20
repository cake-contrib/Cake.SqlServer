using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.SqlServer.LocalDb
{
    [CakeAliasCategory("SqlServer")]
    public static class LocalDbAliases
    {
        [CakeMethodAlias]
        public static void CreateLocalDbInstance(this ICakeContext context, string instanceName, LocalDbVersion version = LocalDbVersion.V11)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Create,
                InstanceName = instanceName,
                InstanceVersion = version,
            };
            ExecuteRunner(context, settings);
        }

        [CakeMethodAlias]
        public static void DeleteLocalDbInstance(this ICakeContext context, string instanceName)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Delete,
                InstanceName = instanceName,
            };
            ExecuteRunner(context, settings);
        }


        [CakeMethodAlias]
        public static void StartLocalDbInstance(this ICakeContext context, string instanceName)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Start,
                InstanceName = instanceName,
            };
            ExecuteRunner(context, settings);
        }


        [CakeMethodAlias]
        public static void StopLocalDbInstance(this ICakeContext context, string instanceName)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Stop,
                InstanceName = instanceName,
            };
            ExecuteRunner(context, settings);
        }


        private static void ExecuteRunner(this ICakeContext context, LocalDbSettings settings)
        {
            var localDbRunner = new LocalDbToolRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            localDbRunner.Run(settings);
        }
    }
}
