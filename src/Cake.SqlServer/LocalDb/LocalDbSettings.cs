using System;
using Cake.Core.Tooling;

namespace Cake.SqlServer
{
    internal class LocalDbSettings : ToolSettings
    {
        public LocalDbSettings()
        {
            Action = LocalDbAction.Create;
            InstanceVersion = LocalDbVersion.V11;
        }

        public LocalDbAction Action { get; set; }
        public String InstanceName { get; set; }
        public LocalDbVersion InstanceVersion { get; set; }
    }


    /// <summary>
    /// Version of LocalDB
    /// </summary>
    public enum LocalDbVersion
    {
        /// <summary>
        /// Represents v11 of LocalDB
        /// </summary>
        V11,

        /// <summary>
        /// Represents v12 of LocalDB
        /// </summary>
        V12,
    }

    internal enum LocalDbAction
    {
        Create,
        Delete,
        Start,
        Stop,
    }
}