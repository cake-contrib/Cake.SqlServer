using Cake.Core.Tooling;

namespace Cake.SqlServer
{
    /// <summary>
    /// Local DB settings.
    /// </summary>
    internal class LocalDbSettings : ToolSettings
    {
        public LocalDbSettings()
        {
            Action = LocalDbAction.Create;
        }

        public LocalDbAction Action { get; set; }

        public string? InstanceName { get; set; }

        public LocalDbVersion? InstanceVersion { get; set; }
    }
}
