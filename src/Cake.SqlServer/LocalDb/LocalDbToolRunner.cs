using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;


namespace Cake.SqlServer
{
    public class LocalDbToolRunner : Tool<LocalDbSettings>
    {
        private readonly IFileSystem fileSystem;
        private readonly ICakeEnvironment environment;
        private readonly IProcessRunner processRunner;
        private readonly IToolLocator toolsLocator;

        public LocalDbToolRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) 
            : base(fileSystem, environment, processRunner, tools)
        {
            this.fileSystem = fileSystem;
            this.environment = environment;
            this.processRunner = processRunner;
            this.toolsLocator = tools;
        }

        protected override string GetToolName()
        {
            return "SqlLocalDB";
        }

        protected override IEnumerable<string> GetToolExecutableNames()
        {
            yield return "SqlLocalDB.exe";
        }

        protected override IEnumerable<FilePath> GetAlternativeToolPaths(LocalDbSettings settings)
        {
            //http://forum.ai-dot.net/viewtopic.php?t=4966
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MicrosoftSQL Server Local DB\Installed Version
            // var sqlLocalDbPath = @"c:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe";
            //var sqlLocalDbPath = @"C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe";

            return new List<FilePath>()
            {
                @"c:\Program Files\Microsoft SQL Server\140\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\130\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\120\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\110\Tools\Binn\",
            };
        }

        public void Run(LocalDbSettings settings)
        {
            if (String.IsNullOrEmpty(settings.InstanceName))
            {
                throw new ArgumentNullException("settings.InstanceName");
            }
            var argumentBuilder = new ProcessArgumentBuilder();

            argumentBuilder.Append(settings.Action.ToString().ToLower());

            argumentBuilder.Append(settings.InstanceName);

            if (settings.Action == LocalDbAction.Create)
            {
                switch (settings.InstanceVersion)
                {
                    case LocalDbVersion.V11:
                        argumentBuilder.Append("v11.0");
                        break;
                    case LocalDbVersion.V12:
                        argumentBuilder.Append("v12.0");
                        break;
                }

                argumentBuilder.Append("-s"); // start the instance;
            }

            Run(new LocalDbSettings(), argumentBuilder);
        }
    }
}
