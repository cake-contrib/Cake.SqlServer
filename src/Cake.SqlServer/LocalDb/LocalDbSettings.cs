using System;
using Cake.Core.Tooling;

namespace Cake.SqlServer
{
    public class LocalDbSettings : ToolSettings
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


    public enum LocalDbVersion
    {
        V11,
        V12,
    }

    public enum LocalDbAction
    {
        Create,
        Delete,
        Start,
        Stop,
    }
}