using System;

namespace Cake.SqlServer
{
    public sealed class CreateDatabaseSettings
    {
        public CreateDatabaseFileSpec PrimaryFile { get; set; }
        public CreateDatabaseFileSpec LogFile { get; set; }

        public CreateDatabaseSettings WithPrimaryFile(String primaryFileName)
        {
            PrimaryFile = new CreateDatabaseFileSpec(primaryFileName);
            return this;
        }

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

    public sealed class CreateDatabaseFileSpec
    {
        public CreateDatabaseFileSpec(String fileName)
        {
            FileName = fileName;
        }

        public String FileName { get; }
        public String Name { get; set; }
        //public String Size { get; set; }
        //public String MaxSize { get; set; }
        //public String FileGrowth { get; set; }
    }
}
