namespace Cake.SqlServer
{
    public sealed class CreateDatabaseSettings
    {
        public string PrimaryFileName { get; private set; }
        public string LogFileName { get; private set;}

        public CreateDatabaseSettings WithPrimaryFile(string primaryFileName)
        {
            PrimaryFileName = primaryFileName;
            return this;
        }

        public CreateDatabaseSettings WithLogFile(string logFileName)
        {
            LogFileName = logFileName;
            return this;
        }
    }
}
