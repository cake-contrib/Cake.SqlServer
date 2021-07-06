﻿using System.Collections.Generic;
using Cake.SqlServer;
using Cake.Testing;
using Cake.Testing.Fixtures;

namespace Tests
{
    internal class LocalDbToolRunnerFixture : ToolFixture<LocalDbSettings>
    {
        internal string InstanceName { get; set; }
        internal LocalDbVersion Version { get; set; }
        internal LocalDbAction Action { get; set; }

        public LocalDbToolRunnerFixture() : base("SqlLocalDB.exe")
        {
            InstanceName = "Cake-Testing";
            Version = LocalDbVersion.V12;
        }

        protected override void RunTool()
        {
            ProcessRunner.Process.SetStandardOutput(new List<string> { "Success" });

            var tool = new LocalDbToolRunner(FileSystem, Environment, ProcessRunner, Tools, new FakeLog());
            tool.Run(new LocalDbSettings { InstanceName = InstanceName, InstanceVersion = Version, Action = Action });
        }
    }
}
