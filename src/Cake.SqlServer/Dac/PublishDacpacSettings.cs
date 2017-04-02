namespace Cake.SqlServer
{
    /// <summary>
    /// Settings for extract dacpac from database
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
        /// Specifies additional deployment contributors which should run - in addition
        /// to those specified in the dacpac.
        /// </summary>
        public string AdditionalDeploymentContributors { get; set; }

        /// <summary>
        /// Specifies additional deployment contributor arguments in addition to those already listed
        /// in the dacpac.
        /// </summary>
        public string AdditionalDeploymentContributorArguments { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether CLR deployment will cause blocking assemblies to be dropped.
        /// </summary>
        /// <value>
        /// True to drop blocking assemblies during CLR deployment; otherwise, false.
        /// Default is false.
        /// </value>
        public bool AllowDropBlockingAssemblies { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether deployment will block due to platform compatibility.
        /// </summary>
        /// <value>
        /// True to block deployment to incompatible platforms; otherwise, false.
        /// Default is false.
        /// </value>
        public bool AllowIncompatiblePlatform { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether a database backup will be performed before proceeding
        /// with the actual deployment actions.
        /// </summary>
        /// <value>
        /// True to perform a database backup prior to deployment; otherwise, false.
        /// Default is false.
        /// </value>
        public bool BackupDatabaseBeforeChanges { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether deployment should stop if the operation could cause data loss.
        /// </summary>
        /// <value>
        /// True to stop deployment if possible data loss if detected; otherwise, false.
        /// Default is true.
        /// </value>
        public bool BlockOnPossibleDataLoss { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the system will check for differences between the
        /// present state of the database and the registered state of the database and block deployment
        /// if changes are detected.  Even if this option is set to true, drift detection will only occur
        /// on a database if it was previously deployed with the <see cref="P:Microsoft.SqlServer.Dac.DacDeployOptions.RegisterDataTierApplication" /> option enabled.
        /// </summary>
        /// <value>
        /// True to error is drift is detected; otherwise, false.
        /// Default is true.
        /// </value>
        public bool BlockWhenDriftDetected { get; set; }

        /// <summary>
        /// Specifies the command timeout in seconds when executing queries against SQLServer.
        /// </summary>
        /// <value>
        /// The command timeout in seconds.
        /// Default is 60
        /// </value>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the declaration of SQLCMD variables are commented
        /// out in the script header.
        /// </summary>
        /// <value>
        /// True to comment out these declarations; otherwise, false.
        /// Default is false.
        /// </value>
        public bool CommentOutSetVarDeclarations { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the target collation will be used for identifier
        /// comparison.
        /// </summary>
        /// <value>
        /// False to use the source collation; otherwise, true to use the target collation.
        /// Default is false.
        /// </value>
        public bool CompareUsingTargetCollation { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the existing database will be dropped
        /// and a new database created before proceeding with the actual deployment actions.
        /// Acquires single-user mode before dropping the existing database.
        /// </summary>
        /// <value>
        /// True to drop and re-create the database; otherwise, false.
        /// Default is false.
        /// </value>
        public bool CreateNewDatabase { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the system will acquire single-user mode on the target
        /// database during the duration of the deployment operation.
        /// </summary>
        /// <value>
        /// True to acquire single-user mode during deployment; otherwise, false.
        /// Default is false.
        /// </value>
        /// <remarks>
        /// The database will be returned to multi-user mode after all changes are applied.
        /// Database may remain in single-user mode if an error occurs during execution.
        /// </remarks>
        public bool DeployDatabaseInSingleUserMode { get; set; }

        /// <summary>
        /// Get or set boolean that specifies if all DDL triggers will be disabled for the duration of the
        /// deployment operation and then re-enabled after all changes are applied.
        /// </summary>
        /// <value>
        /// True to disable DDL triggers during deployment; otherwise, false.
        /// Default is true.
        /// </value>
        /// <remarks>
        /// Triggers may remain disabled if an error occurs during execution.
        /// </remarks>
        public bool DisableAndReenableDdlTriggers { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether items configured for Change Data Capture (CDC)
        /// should be altered during deployment.
        /// </summary>
        /// <value>
        /// True to not alter objects configured for CDC; otherwise, false.
        /// Default is true.
        /// </value>
        public bool DoNotAlterChangeDataCaptureObjects { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether items configured for Replication
        /// should be altered during deployment.
        /// </summary>
        /// <value>
        /// True to not alter objects configured for Replication; otherwise, false.
        /// Default is true.
        /// </value>
        public bool DoNotAlterReplicatedObjects { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all constraints that do not
        /// exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop constraints not in the source model; otherwise, false.
        /// Default is true.
        /// </value>
        /// <remarks>
        /// This applies to check, default, foreign key, primary key, and unique constraints.
        /// </remarks>
        public bool DropConstraintsNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all DML triggers that do not
        /// exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop DML triggers not in the source model; otherwise, false.
        /// Default is true.
        /// </value>
        public bool DropDmlTriggersNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all extended properties that do
        /// not exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop extended properties not in the source model; otherwise, false.
        /// Default is true.
        /// </value>
        public bool DropExtendedPropertiesNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all indexes that do not
        /// exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop indexes not in the source model; otherwise, false.
        /// Default is true.
        /// </value>
        public bool DropIndexesNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether objects that exist in the target but not source should be dropped during deployment.
        /// </summary>
        /// <value>
        /// True if objects that exist in the target but not source should be dropped; otherwise false.
        /// Default is false.
        /// </value>
        public bool DropObjectsNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all permissions that do not
        /// exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop permissions not in the source model; otherwise, false.
        /// Default is false.
        /// </value>
        public bool DropPermissionsNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all role memberships that do not
        /// exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop role memberships not in the source model; otherwise, false.
        /// Default is false.
        /// </value>
        public bool DropRoleMembersNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to drop all role memberships that do not
        /// exist in the source model.
        /// </summary>
        /// <value>
        /// True to drop role memberships not in the source model; otherwise, false.
        /// Default is false.
        /// </value>
        public bool DropStatisticsNotInSource { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether default values should be generated to populate NULL columns that are constrained to NOT NULL values.
        /// </summary>
        /// <value>
        /// True if default values should be generated; otherwise false.
        /// Default is false.
        /// </value>
        /// <remarks>
        /// This is useful when needing to add a new NOT NULL column to an existing table with data.
        /// </remarks>
        public bool GenerateSmartDefaults { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the ANSI_NULL option from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the ANSI_NULL option; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreAnsiNulls { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the AUTHORIZATION option from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the AUTHORIZATION option; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreAuthorizer { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the collation specifier from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the collation specifier; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreColumnCollation { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude comments from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in comments; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreComments { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the file specification
        /// of a cryptographic provider from consideration when comparing the source and
        /// target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in a cryptographic provider's  file specification; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreCryptographicProviderFilePath { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude DDL trigger order from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in DDL trigger order; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreDdlTriggerOrder { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude DDL trigger state from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in DDL trigger state; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreDdlTriggerState { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the DEFAULT_SCHEMA option from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the DEFAULT_SCHEMA options; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreDefaultSchema { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude DML trigger order from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in DDL trigger order; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreDmlTriggerOrder { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude DML trigger state from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in DML trigger state; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreDmlTriggerState { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude all extended properties from
        /// consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in extended properties; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreExtendedProperties { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the FILENAME option of
        /// FILE objects from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the FILENAME option of FILE objects; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreFileAndLogFilePath { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the filegroup specifier
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the filegroup specifier; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreFilegroupPlacement { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the SIZE option of FILE objects
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the SIZE option of FILE objects; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreFileSize { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the FILLFACTOR option
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the FILLFACTOR option; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreFillFactor { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the path specification of
        /// FULLTEXT CATALOG objects from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the path specification of FULLTEXT CATALOG objects; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreFullTextCatalogFilePath { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the seed value of IDENTITY columns
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the seed value of IDENTITY columns; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreIdentitySeed { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the increment value of IDENTITY columns
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the increment value of IDENTITY columns; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreIncrement { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude differences in index options
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in index options; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreIndexOptions { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the PAD_INDEX option
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the PAD_INDEX option; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreIndexPadding { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude difference in the casing of keywords
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in the casing of keywords; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreKeywordCasing { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the ALLOW_ROW_LOCKS and
        /// ALLOW_PAGE_LOGKS options from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the ALLOW_ROW_LOCKS and ALLOW_PAGE_LOGKS options; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreLockHintsOnIndexes { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the SID option of the LOGIN object
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the SID option of the LOGIN object; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreLoginSids { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the NOT FOR REPLICATION option
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the NOT FOR REPLICATION option; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreNotForReplication { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the partition scheme object
        /// from consideration when comparing the source and target model for the following
        /// objects: Table, Index, Unique Key, Primary Key, and Queue.
        /// </summary>
        /// <value>
        /// True to ignore partition schemes; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreObjectPlacementOnPartitionScheme { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the parameter type and
        /// boundary VALUES of a PARTITION FUNCTION from consideration when comparing the
        /// source and target model.  Also excludes FILEGROUP and partition function of a
        /// PARTITION SCHEMA from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore aspects of partition functions and schemes; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnorePartitionSchemes { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude all permission statements
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore all permission statements; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnorePermissions { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the QUOTED_IDENTIFIER option
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the QUOTED_IDENTIFIER option; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreQuotedIdentifiers { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude all ROLE MEMBERSHIP objects
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore ROLE MEMBERSHIP objects; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreRoleMembership { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the LIFETIME option of ROUTE objects
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the LIFETIME option of ROUTE objects; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreRouteLifetime { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the existence or absence of semi-colons
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore semi-colons; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreSemicolonBetweenStatements { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the options on the target table are updated
        /// to match the source table.
        /// </summary>
        /// <value>
        /// True to ignore difference in table options and not update the target table; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreTableOptions { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude user settings
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences in user settings; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreUserSettingsObjects { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude whitespace
        /// from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore differences whitespace; otherwise, false.
        /// Default is true.
        /// </value>
        public bool IgnoreWhitespace { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the CHECK|NO CHECK option of a CHECK
        /// constraint object from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the CHECK|NO CHECK option of a CHECK constraint object; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreWithNocheckOnCheckConstraints { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to exclude the CHECK|NO CHECK option of a FOREIGN KEY
        /// constraint object from consideration when comparing the source and target model.
        /// </summary>
        /// <value>
        /// True to ignore the CHECK|NO CHECK option of a FOREIGN KEY constraint object; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IgnoreWithNocheckOnForeignKeys { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to ignore blocking data motion on RLS enabled tables
        /// </summary>
        /// <value>
        /// True to ignore block on data motion when Row level security is enabled on a table
        /// Default is false i.e. data motion is blocked with RLS by default.
        /// </value>
        public bool AllowUnsafeRowLevelSecurityDataMovement { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to include referenced, external elements that also
        /// compose the source model and then update the target database in a single deployment operation.
        /// </summary>
        /// <value>
        /// True to include composite objects; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IncludeCompositeObjects { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to use transations during the deployment operation
        /// and commit the transaction after all changes are successfully applied.
        /// </summary>
        /// <value>
        /// True to use transactions during deployment; otherwise, false.
        /// Default is false.
        /// </value>
        public bool IncludeTransactionalScripts { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to force a change to CLR assemblies by dropping and recreating them.
        /// </summary>
        /// <value>
        /// True if CLR assemblies should be dropped; otherwise false to allow ALTER statements to change CLR assemblies.
        /// Default is false.
        /// </value>
        public bool NoAlterStatementsToChangeClrTypes { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether files are supplied for filegroups defined in the deployment source.
        /// </summary>
        /// <value>
        /// True to specify files for filegroups; otherwise false.
        /// Default is true.
        /// </value>
        public bool PopulateFilesOnFileGroups { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the database will be registered as a Data-Tier Application.
        /// If the target database is already a registered Data-Tier Application, then the registration will be updated.
        /// </summary>
        /// <value>
        /// True to register the database as a Data-Tier Application; otherwise, false.
        /// Default is false.
        /// </value>
        public bool RegisterDataTierApplication { get; set; }

        /// <summary>
        /// Specifies whether DeploymentPlanExecutor contributors should be run when other operations are executed.
        /// Default is false.
        /// </summary>
        public bool RunDeploymentPlanExecutors { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the target database should be altered to match the
        /// source model's collation.
        /// </summary>
        /// <value>
        /// True to alter the target database's collation; otherwise, false.
        /// Default is false.
        /// </value>
        public bool ScriptDatabaseCollation { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the target database should be altered to match the
        /// source model's compatibility level.
        /// </summary>
        /// <value>
        /// True to alter the target database's compatibility level; otherwise, false.
        /// Default is false.
        /// </value>
        public bool ScriptDatabaseCompatibility { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the database options in the target database should
        /// be updated to match the source model.
        /// </summary>
        /// <value>
        /// True to alter the target database's options; otherwise, false.
        /// Default is true.
        /// </value>
        public bool ScriptDatabaseOptions { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the target database should be checked to ensure that
        /// it exists, is online and can be updated.
        /// </summary>
        /// <value>
        /// True to perform state checks on the target database; otherwise, false.
        /// Default is false.
        /// </value>
        public bool ScriptDeployStateChecks { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether a file size is specified when adding files to file groups.
        /// </summary>
        /// <value>
        /// True to specify a file size when adding files to file groups; otherwise, false.
        /// Default is false.
        /// </value>
        public bool ScriptFileSize { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether constraints are validated after all changes are applied.
        /// </summary>
        /// <value>
        /// True to validate check constraints; otherwise, false.
        /// Default is true.
        /// </value>
        /// <remarks>
        /// Constraints are always added with NOCHECK option; as a result their validation is skipped during creation.
        /// </remarks>
        public bool ScriptNewConstraintValidation { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether referencing procedures are refreshed when referenced objects are updated.
        /// </summary>
        /// <value>
        /// True to refresh referencing procedures; otherwise false.
        /// Default is true.
        /// </value>
        public bool ScriptRefreshModule { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether to treat errors that occur during publish verification as warnings.
        /// The check is performed against the generated deployment plan before the plan is executed against the target database.
        /// Plan verification detects problems, such as the loss of target-only objects (for example, indexes), that must be
        /// dropped to make a change. Verification also detects situations where dependencies (such as tables or views) exist
        /// because of a reference to a composite project, but do not exist in the target database. You might choose to treat
        /// verification errors as warnings to get a complete list of issues instead of allowing the publish
        /// action to stop when the first error occurs.
        /// </summary>
        /// <value>
        /// True to treat errors as warnings; otherwise, false.
        /// Default is false.
        /// </value>
        public bool TreatVerificationErrorsAsWarnings { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether warnings should be generated when differences are found
        /// in objects that cannot be modified, for example, if the file size or file paths were different for a file.
        /// </summary>
        /// <value>
        /// True to generate warnings; otherwise, false.
        /// Default is true.
        /// </value>
        public bool UnmodifiableObjectWarnings { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether deployment will verify that the collation specified in the
        /// source model is compatible with the collation specified in the target model.
        /// </summary>
        /// <value>
        /// True to continue if errors are generated during plan verification; otherwise, false.
        /// Default is true.
        /// </value>
        public bool VerifyCollationCompatibility { get; set; }

        /// <summary>
        /// Get or set boolean that specifies whether the plan verification phase is executed or not.
        /// </summary>
        /// <value>
        /// True to perform plan verification; otherwise, false to skip it.
        /// Default is true.
        /// </value>
        public bool VerifyDeployment { get; set; }

        /// <summary>
        /// Configures options for what will be reported when performing certain operations from <see cref="T:Microsoft.SqlServer.Dac.DacServices" />,
        /// in particular whether a DeployReport and/or DeployScript will be generated
        /// </summary>
        public PublishDacpacSettings()
        {
            this.GenerateDeploymentScript = true;
            this.GenerateDeploymentReport = false;
        }
    }
}
