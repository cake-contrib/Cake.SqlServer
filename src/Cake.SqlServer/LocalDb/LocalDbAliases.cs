using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.SqlServer.LocalDb
{
    /// <summary>
    /// Contains functionality to deal with LocalDB. A wrapper for SQLLocalDb.exe. Allows to create, start, stop and delete instances in LocalDB.
    /// 
    /// See <see href="https://msdn.microsoft.com/en-us/library/hh212961%28v=sql.120%29.aspx?f=255&MSPPError=-2147217396">MSDN documentation page</see> for more details about operations
    /// 
    /// In order to use the commands for this addin, include the following in your build.cake file to download and
    /// reference from NuGet.org:
    /// <code>
    ///     #addin "nuget:?package=Cake.SqlServer"
    /// </code>
    /// </summary>
    [CakeAliasCategory("SqlServer")]
    public static class LocalDbAliases
    {

        /// <summary>
        /// Creates a server instance and starts the server. 
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="instanceName">Name of the instance to create</param>
        /// <param name="version">Version number of LocalDB to use V11 or V12</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-LocalDB")
        ///          .Does(() =>
        ///          {
        ///             LocalDbCreateInstance("Cake-Test");
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void LocalDbCreateInstance(this ICakeContext context, string instanceName, LocalDbVersion version = LocalDbVersion.V11)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Create,
                InstanceName = instanceName,
                InstanceVersion = version,
            };
            ExecuteRunner(context, settings);
        }


        /// <summary>
        /// Deletes the LocalDB instance
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="instanceName">Instance name to delete</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Delete-LocalDB")
        ///          .Does(() =>
        ///          {
        ///             LocalDbDeleteInstance("Cake-Test");
        ///         });
        /// </code>
        /// </example>        
        [CakeMethodAlias]
        public static void LocalDbDeleteInstance(this ICakeContext context, string instanceName)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Delete,
                InstanceName = instanceName,
            };
            ExecuteRunner(context, settings);
        }


        /// <summary>
        /// Starts the LocalDB instance. Instance must exist before you can start it.
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="instanceName">Name of the instance to start</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Start-LocalDB")
        ///          .Does(() =>
        ///          {
        ///             LocalDbStartInstance("Cake-Test");
        ///         });
        /// </code>
        /// </example>        
        [CakeMethodAlias]
        public static void LocalDbStartInstance(this ICakeContext context, string instanceName)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Start,
                InstanceName = instanceName,
            };
            ExecuteRunner(context, settings);
        }


        /// <summary>
        /// Stops the LocalDB instance.
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="instanceName">Name of the instance to stop</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Stop-LocalDB")
        ///          .Does(() =>
        ///          {
        ///             LocalDbStopInstance("Cake-Test");
        ///         });
        /// </code>
        /// </example>  
        [CakeMethodAlias]
        public static void LocalDbStopInstance(this ICakeContext context, string instanceName)
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
