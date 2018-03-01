using System;

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
        public CreateDatabaseFileSpec PrimaryFile { get; set; }

        /// <summary>
        /// File Spec for transaction log file
        /// </summary>
        public CreateDatabaseFileSpec LogFile { get; set; }

        /// <summary>
        /// Builder method to set up path for primary file.
        /// </summary>
        /// <param name="primaryFileName"></param>
        /// <returns></returns>
        public CreateDatabaseSettings WithPrimaryFile(String primaryFileName)
        {
            PrimaryFile = new CreateDatabaseFileSpec(primaryFileName);
            return this;
        }

        /// <summary>
        /// Builder method to set up path to log file.
        /// </summary>
        /// <param name="logFileName"></param>
        /// <returns></returns>
        public CreateDatabaseSettings WithLogFile(String logFileName)
        {
            LogFile = new CreateDatabaseFileSpec(logFileName);
            return this;
        }

        internal void AssignNames(String databaseName)
        {
            if (PrimaryFile != null && String.IsNullOrEmpty(PrimaryFile.Name))
            {
                PrimaryFile.Name = databaseName;
            }

            if (LogFile != null && String.IsNullOrEmpty(LogFile.Name))
            {
                LogFile.Name = databaseName + "_log";
            }
        }
    }


    /// <summary>
    /// File Spec object to describe file name and path for creating databases
    /// </summary>
    public sealed class CreateDatabaseFileSpec
    {
        /// <summary>
        /// Constructor for filespec.
        /// </summary>
        /// <param name="fileName">Path to file</param>
        public CreateDatabaseFileSpec(String fileName)
        {
            // need to replace fowards slashes to backward because SQL Server does not like them.
            FileName = fileName.Replace("/", "\\");
        }

        /// <summary>
        /// Path to file
        /// </summary>
        public String FileName { get; }

        /// <summary>
        /// Name of file - internal name of file in SQL Server. 
        /// Defaulted to database name for mdf and {DatabaseName}_log for log files
        /// </summary>
        public String Name { get; set; }
        //public String Size { get; set; }
        //public String MaxSize { get; set; }
        //public String FileGrowth { get; set; }
    }
}
