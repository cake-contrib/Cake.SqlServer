namespace Cake.SqlServer
{
    /// <summary>
    /// Settings for backing up database to a file
    /// </summary>
    public class BackupDatabaseSettings
    {
        /// <summary>
        /// Default constructor. Sets Compress to true.
        /// </summary>
        public BackupDatabaseSettings()
        {
            Compress = true;
        }

        /// <summary>
        /// Gets or sets the backup file path.
        /// This can either be a file or a folder
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Sets the flag to execute backup and create a compressed file
        /// </summary>
        public bool Compress { get; set; }
    }
}
