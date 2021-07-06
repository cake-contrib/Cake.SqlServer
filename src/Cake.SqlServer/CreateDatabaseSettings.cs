#if NET5_0
using System;
#endif

namespace Cake.SqlServer
{
    /// <summary>
    /// Settings object for creation of databases
    /// See spec from https://docs.microsoft.com/en-us/sql/t-sql/statements/create-database-sql-server-transact-sql#the-model-database-and-creating-new-databases
    /// </summary>
    public sealed class CreateDatabaseSettings
    {
        /// <summary>
        /// File Spec for Primary database file.
        /// </summary>
        public CreateDatabaseFileSpec? PrimaryFile { get; set; }

        /// <summary>
        /// File Spec for transaction log file
        /// </summary>
        public CreateDatabaseFileSpec? LogFile { get; set; }

        /// <summary>
        /// Builder method to set up path for primary file.
        /// </summary>
        /// <param name="primaryFileName"></param>
        /// <returns></returns>
        public CreateDatabaseSettings WithPrimaryFile(string primaryFileName)
        {
            PrimaryFile = new CreateDatabaseFileSpec(primaryFileName);
            return this;
        }

        /// <summary>
        /// Builder method to set up path to log file.
        /// </summary>
        /// <param name="logFileName"></param>
        /// <returns></returns>
        public CreateDatabaseSettings WithLogFile(string logFileName)
        {
            LogFile = new CreateDatabaseFileSpec(logFileName);
            return this;
        }

        internal void AssignNames(string databaseName)
        {
            if (PrimaryFile != null && string.IsNullOrEmpty(PrimaryFile.Name))
            {
                PrimaryFile.Name = databaseName;
            }

            if (LogFile != null && string.IsNullOrEmpty(LogFile.Name))
            {
                LogFile.Name = databaseName + "_log";
            }
        }
    }
}
