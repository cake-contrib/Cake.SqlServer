using System.Threading;
using Microsoft.SqlServer.Dac;

namespace Cake.SqlServer
{
    /// <summary>
    /// Configures options for what will be reported when performing certain operations from <see cref="T:Microsoft.SqlServer.Dac.DacServices" />,
    /// in particular whether a DeployReport and/or DeployScript will be generated
    /// </summary>
    public class PublishDacpacSettings
    {


        /// <summary>
        /// Sets whether Deployment Script(s) should be generated during deploy.
        /// If true, a script to update the database will be generated, and a script to
        /// update Master may also be generated if the target is an Azure SQL DB and this
        /// database has not yet been created.
        /// </summary>
        /// <value>Defaults to true</value>
        public bool GenerateDeploymentScript { get; set; }

        /// <summary>
        /// Sets whether a Deployment Report should be generated during deploy.
        /// This report is a high-level summary of actions being performed during deployment.
        /// </summary>
        /// <value>Defaults to false</value>
        public bool GenerateDeploymentReport { get; set; }

        /// <summary>
        /// Optional path to write the DB-level deployment script, if <see cref="P:Microsoft.SqlServer.Dac.PublishOptions.GenerateDeploymentScript" /> is true.
        /// This script contains all operations that must be done against the database during deployment.
        /// </summary>
        public string DatabaseScriptPath { get; set; }

        /// <summary>
        /// Optional path to write the master database-level deployment script, if <see cref="P:Microsoft.SqlServer.Dac.PublishOptions.GenerateDeploymentScript" /> is true.
        /// This script is only created if Azure SQL DB is the target as USE statements are not supported on that platform.
        /// It contains all operations that must be done against the master database, for instance Create Database statements
        /// </summary>
        public string MasterDbScriptPath { get; set; }

        /// <summary>
        /// Configures options for what will be reported when performing certain operations from <see cref="T:Microsoft.SqlServer.Dac.DacServices" />,
        /// in particular whether a DeployReport and/or DeployScript will be generated
        /// </summary>
        public PublishDacpacSettings()
        {
            GenerateDeploymentScript = true;
            GenerateDeploymentReport = false;
        }
    }
}
