using System.Runtime.CompilerServices;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: InternalsVisibleTo("Tests")]
namespace Cake.SqlServer
{
    /// <summary>
    /// <para>
    /// Contains functionality to deal with LocalDB. A wrapper for SQLLocalDb.exe. Allows to create, start, stop and delete instances in LocalDB.
    /// See <see href="https://msdn.microsoft.com/en-us/library/hh212961%28v=sql.120%29.aspx?f=255&MSPPError=-2147217396">MSDN documentation page</see> for more details about operations
    /// </para>
    /// <para>
    /// In order to use the commands for this addin, include the following in your build.cake file to download and reference from NuGet.org:
    /// </para>
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
        /// <param name="version">Version number of LocalDB to use V11 or V12.  The specified version must be installed on the computer. If not specified, the version number defaults to the version of the SqlLocalDB utility</param>
        /// <example>
        /// <code>
        ///     #addin "nuget:?package=Cake.SqlServer"
        /// 
        ///     Task("Create-LocalDB")
        ///          .Does(() =>
        ///          {
        ///             LocalDbCreateInstance("Cake-Test", LocalDbVersion.V11);
        ///         });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void LocalDbCreateInstance(this ICakeContext context, string instanceName, LocalDbVersion version)
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
        /// Creates a server instance and starts the server. 
        /// The version number defaults to the version of the SqlLocalDB utility
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="instanceName">Name of the instance to create</param>
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
        public static void LocalDbCreateInstance(this ICakeContext context, string instanceName)
        {
            var settings = new LocalDbSettings()
            {
                Action = LocalDbAction.Create,
                InstanceName = instanceName,
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
            context.Log.Information(Verbosity.Normal, "Executing SQLLocalDB.exe with action {0} on instance {1}", settings.Action, settings.InstanceName);
            var localDbRunner = new LocalDbToolRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Log);
            localDbRunner.Run(settings);
        }
    }
}
