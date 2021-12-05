namespace Cake.SqlServer
{
    /// <summary>
    /// Settings for backing up database to a file.
    /// </summary>
    public class BackupDatabaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupDatabaseSettings"/> class.
        /// Sets Compress to true.
        /// </summary>
        public BackupDatabaseSettings()
        {
            Compress = true;
        }

        /// <summary>
        /// Gets or sets the backup file path.
        /// This can either be a file or a folder.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether execute backup and create a compressed file.
        /// </summary>
        public bool Compress { get; set; }
    }
}
