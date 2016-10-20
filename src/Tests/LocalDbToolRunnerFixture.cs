using System;
using Cake.SqlServer;
using Cake.Testing.Fixtures;

namespace Tests
{
    public class LocalDbToolRunnerFixture : ToolFixture<LocalDbSettings>
    {
        internal String InstanceName { get; set; }
        internal LocalDbVersion Version { get; set; }
        internal LocalDbAction Action { get; set; }

        public LocalDbToolRunnerFixture() : base("SqlLocalDB.exe")
        {
            InstanceName = "Cake-Testing";
            Version = LocalDbVersion.V12;
        }

        protected override void RunTool()
        {
            var tool = new LocalDbToolRunner(FileSystem, Environment, ProcessRunner, Tools);
            tool.Run(new LocalDbSettings() { InstanceName = this.InstanceName, InstanceVersion = Version, Action = this.Action});
        }
    }
}
