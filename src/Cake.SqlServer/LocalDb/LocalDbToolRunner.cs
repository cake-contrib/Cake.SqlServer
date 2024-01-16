using System.Collections.Generic;
using System.Globalization;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.SqlServer
{
    internal class LocalDbToolRunner : Tool<LocalDbSettings>
    {
        private readonly ICakeLog contextLog;

        public LocalDbToolRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools, ICakeLog contextLog)
            : base(fileSystem, environment, processRunner, tools)
        {
            this.contextLog = contextLog;
        }

        internal void Run(LocalDbSettings settings)
        {
            if (string.IsNullOrEmpty(settings.InstanceName))
            {
                throw new InstanceNameEmptyException("settings.InstanceName");
            }

            var argumentBuilder = new ProcessArgumentBuilder();

            argumentBuilder.Append(settings.Action.ToString().ToLower(CultureInfo.InvariantCulture));

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
                    case LocalDbVersion.V14:
                        argumentBuilder.Append("14.0");
                        break;
                    case LocalDbVersion.V15:
                        argumentBuilder.Append("15.0");
                        break;
                    case LocalDbVersion.V16:
                        argumentBuilder.Append("16.0");
                        break;
                    default:
                        throw new InstanceVersionUnknownException();
                }

                // start the instance
                argumentBuilder.Append("-s");
            }

            var arguments = argumentBuilder.Render();
            contextLog.Information(Verbosity.Diagnostic, "Executing SQLLocalDB.Exe with parameters: {0}", arguments);

            var exitCode = 0;
            var output = string.Empty;

            Run(new LocalDbSettings(), argumentBuilder, new ProcessSettings { RedirectStandardOutput = true, }, process =>
           {
               exitCode = process.GetExitCode();
               output = string.Join("\n", process.GetStandardOutput() ?? new List<string>());
               contextLog.Information(Verbosity.Diagnostic, "Process output: {0}", output);
               contextLog.Information(Verbosity.Diagnostic, "Process Exit Code: {0}", exitCode);
           });

            if (exitCode != 0 || string.IsNullOrWhiteSpace(output))
            {
                throw new LocalDBExecutionFailedException("LocalDB execution failed. Please see message above.");
            }
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
            //// http://forum.ai-dot.net/viewtopic.php?t=4966
            //// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MicrosoftSQL Server Local DB\Installed Version

            return new List<FilePath>
            {
                @"C:\Program Files\Microsoft SQL Server\160\Tools\Binn\",
                @"C:\Program Files\Microsoft SQL Server\150\Tools\Binn\",
                @"C:\Program Files\Microsoft SQL Server\140\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\130\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\120\Tools\Binn\",
                @"c:\Program Files\Microsoft SQL Server\110\Tools\Binn\",
            };
        }
    }
}
