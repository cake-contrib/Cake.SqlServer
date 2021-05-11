using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;


namespace Cake.SqlServer
{
    internal class LocalDbToolRunner : Tool<LocalDbSettings>
    {
        private readonly IFileSystem fileSystem;
        private readonly ICakeEnvironment environment;
        private readonly IProcessRunner processRunner;
        private readonly IToolLocator toolsLocator;
        private readonly ICakeLog contextLog;

        public LocalDbToolRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools, ICakeLog contextLog)
            : base(fileSystem, environment, processRunner, tools)
        {
            this.fileSystem = fileSystem;
            this.environment = environment;
            this.processRunner = processRunner;
            this.toolsLocator = tools;
            this.contextLog = contextLog;
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
                @"C:\Program Files\Microsoft SQL Server\150\Tools\Binn\",
                @"C:\Program Files\Microsoft SQL Server\140\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\130\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\120\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\110\Tools\Binn\",
            };
        }

        internal void Run(LocalDbSettings settings)
        {
            if (String.IsNullOrEmpty(settings.InstanceName))
            {
                throw new ArgumentNullException("settings.InstanceName");
            }
            var argumentBuilder = new ProcessArgumentBuilder();

            argumentBuilder.Append(settings.Action.ToString().ToLower());

            argumentBuilder.AppendQuoted(settings.InstanceName);

            if (settings.Action == LocalDbAction.Create)
            {
                switch (settings.InstanceVersion)
                {
                    case LocalDbVersion.V11:
                        argumentBuilder.Append("11.0");
                        break;
                    case LocalDbVersion.V12:
                        argumentBuilder.Append("12.0");
                        break;
                    case LocalDbVersion.V13:
                        argumentBuilder.Append("13.0");
                        break;
                }

                argumentBuilder.Append("-s"); // start the instance;
            }

            var arguments = argumentBuilder.Render();
            contextLog.Information(Verbosity.Diagnostic, "Executing SQLLocalDB.Exe with parameters: {0}", arguments);

            var exitCode = 0;
            var output = string.Empty;

            Run(new LocalDbSettings(), argumentBuilder, new ProcessSettings() { RedirectStandardOutput = true,  }, process =>
            {
                exitCode = process.GetExitCode();
                output = string.Join("\n", process.GetStandardOutput() ?? new List<string>());
                contextLog.Information(Verbosity.Diagnostic, "Process output: {0}", output);
                contextLog.Information(Verbosity.Diagnostic, "Process Exit Code: {0}", exitCode);
            });

            if (exitCode != 0 || String.IsNullOrWhiteSpace(output))
            {
                throw new Exception("LocalDB execution failed. Please see message above.");
            }
        }
    }
}
