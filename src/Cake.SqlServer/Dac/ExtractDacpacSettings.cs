using System;
using System.Collections.Generic;
using Cake.Core.IO;

namespace Cake.SqlServer
{
    /// <summary>
    /// Settings for extract dacpac from database.
    /// </summary>
    public class ExtractDacpacSettings
    {
        private readonly List<Tuple<string, string>> tables = new();
        private string? outputFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractDacpacSettings"/> class.
        /// String identifier for the DAC application.
        /// </summary>
        /// <param name="appName">String identifier for the DAC application.</param>
        /// <param name="version">Version of the DAC application.</param>
        /// <exception cref="ArgumentNullException">Thrown when appName or version parameters are null.</exception>
        public ExtractDacpacSettings(string appName, string version)
        {
            Guard.ArgumentIsNotNull(appName, nameof(appName));
            Guard.ArgumentIsNotNull(version, nameof(version));

            Name = appName;
            Version = new Version(version);
        }

        /// <summary>
        /// Gets string identifier for the DAC application.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets version of the DAC application.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets or sets optional string summary of the DAC application.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets path to the package file to be created.
        /// </summary>
        public FilePath OutputFile
        {
            get { return string.IsNullOrEmpty(outputFile) ? $"{Name}.{Version}.dacpac" : outputFile; }
            set { outputFile = value.FullPath; }
        }

        /// <summary>
        /// <para>
        /// Gets optional enumerable used to retrieve enumerator over set of tables for which reference data should be stored.
        /// For each <see cref="T:System.Tuple" /> in the enumeration the first item specifies the schema of the table, and the second specifies the base identifier of the table.
        /// </para>
        /// <para>
        /// If the value for this parameter is a null reference, no reference data will be stored.
        /// </para>
        /// </summary>
        public IEnumerable<Tuple<string, string>> Tables => tables;

        /// <summary>
        ///  Include table in the set of tables for which reference data should be stored.
        /// </summary>
        /// <param name="table">Table name.</param>
        /// <param name="schema">Optional schema default to 'dbo'.</param>
        /// <returns>The same settings instance to chain config.</returns>
        /// <exception cref="ArgumentNullException">Argument null exception if table or schema is null.</exception>
        public ExtractDacpacSettings WithTable(string table, string schema = "dbo")
        {
            Guard.ArgumentIsNotNull(table, nameof(table));
            Guard.ArgumentIsNotNull(schema, nameof(schema));

            tables.Add(new Tuple<string, string>(schema, table));
            return this;
        }
    }
}
