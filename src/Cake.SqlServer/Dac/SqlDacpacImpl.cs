using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.SqlServer.Dac;

namespace Cake.SqlServer
{
    internal static class SqlDacpacImpl
    {
        internal static void ExtractDacpacFile(ICakeContext context, string connectionString, string targetDatabaseName, ExtractDacpacSettings settings)
        {
            context.Log.Information($"About to extract a dacpac file from database {targetDatabaseName}");

            var service = new DacServices(connectionString);

            service.Extract(settings.OutputFile.FullPath, targetDatabaseName, settings.Name, settings.Version, settings.Description, settings.Tables);

            context.Log.Information($"Finished creating dacpac file from database {targetDatabaseName}. File location is {settings.OutputFile}");
        }


        internal static void PublishDacpacFile(ICakeContext context, string connectionString, string targetDatabaseName, string dacpacFilePath, PublishDacpacSettings settings = null)
        {
            context.Log.Information($"About to publish dacpac from {dacpacFilePath} into database {targetDatabaseName}");

            var dacPackage = DacPackage.Load(dacpacFilePath);

            context.Log.Debug($"Loaded dacpac file {dacpacFilePath}");

            var service = new DacServices(connectionString);

            var options = GetPublishOptions(settings);

            service.Publish(dacPackage, targetDatabaseName, options);

            context.Log.Information($"Finished restoring dacpac file into database {targetDatabaseName}");
        }

        private static PublishOptions GetPublishOptions(PublishDacpacSettings settings)
        {
            var options = new PublishOptions();
            if (settings == null)
            {
                return options;
            }

            options.GenerateDeploymentScript = settings.GenerateDeploymentScript;
            options.GenerateDeploymentReport = settings.GenerateDeploymentReport;
            options.DatabaseScriptPath = settings.DatabaseScriptPath;
            options.MasterDbScriptPath = settings.MasterDbScriptPath;
            options.DeployOptions = new DacDeployOptions
            {
                AdditionalDeploymentContributors = settings.AdditionalDeploymentContributors,
                AdditionalDeploymentContributorArguments = settings.AdditionalDeploymentContributorArguments,
                AllowDropBlockingAssemblies = settings.AllowDropBlockingAssemblies,
                AllowIncompatiblePlatform = settings.AllowIncompatiblePlatform,
                BackupDatabaseBeforeChanges = settings.BackupDatabaseBeforeChanges,
                BlockOnPossibleDataLoss = settings.BlockOnPossibleDataLoss,
                BlockWhenDriftDetected = settings.BlockWhenDriftDetected,
                CommandTimeout = settings.CommandTimeout,
                CommentOutSetVarDeclarations = settings.CommentOutSetVarDeclarations,
                CompareUsingTargetCollation = settings.CompareUsingTargetCollation,
                CreateNewDatabase = settings.CreateNewDatabase,
                DeployDatabaseInSingleUserMode = settings.DeployDatabaseInSingleUserMode,
                DisableAndReenableDdlTriggers = settings.DisableAndReenableDdlTriggers,
                DoNotAlterChangeDataCaptureObjects = settings.DoNotAlterChangeDataCaptureObjects,
                DoNotAlterReplicatedObjects = settings.DoNotAlterReplicatedObjects,
                DropConstraintsNotInSource = settings.DropConstraintsNotInSource,
                DropDmlTriggersNotInSource = settings.DropDmlTriggersNotInSource,
                DropExtendedPropertiesNotInSource = settings.DropExtendedPropertiesNotInSource,
                DropIndexesNotInSource = settings.DropIndexesNotInSource,
                DropObjectsNotInSource = settings.DropObjectsNotInSource,
                DropPermissionsNotInSource = settings.DropPermissionsNotInSource,
                DropRoleMembersNotInSource = settings.DropRoleMembersNotInSource,
                DropStatisticsNotInSource = settings.DropStatisticsNotInSource,
                GenerateSmartDefaults = settings.GenerateSmartDefaults,
                IgnoreAnsiNulls = settings.IgnoreAnsiNulls,
                IgnoreAuthorizer = settings.IgnoreAuthorizer,
                IgnoreColumnCollation = settings.IgnoreColumnCollation,
                IgnoreComments = settings.IgnoreComments,
                IgnoreCryptographicProviderFilePath = settings.IgnoreCryptographicProviderFilePath,
                IgnoreDdlTriggerOrder = settings.IgnoreDdlTriggerOrder,
                IgnoreDdlTriggerState = settings.IgnoreDdlTriggerState,
                IgnoreDefaultSchema = settings.IgnoreDefaultSchema,
                IgnoreDmlTriggerOrder = settings.IgnoreDmlTriggerOrder,
                IgnoreDmlTriggerState = settings.IgnoreDmlTriggerState,
                IgnoreExtendedProperties = settings.IgnoreExtendedProperties,
                IgnoreFileAndLogFilePath = settings.IgnoreFileAndLogFilePath,
                IgnoreFilegroupPlacement = settings.IgnoreFilegroupPlacement,
                IgnoreFileSize = settings.IgnoreFileSize,
                IgnoreFillFactor = settings.IgnoreFillFactor,
                IgnoreFullTextCatalogFilePath = settings.IgnoreFullTextCatalogFilePath,
                IgnoreIdentitySeed = settings.IgnoreIdentitySeed,
                IgnoreIncrement = settings.IgnoreIncrement,
                IgnoreIndexOptions = settings.IgnoreIndexOptions,
                IgnoreIndexPadding = settings.IgnoreIndexPadding,
                IgnoreKeywordCasing = settings.IgnoreKeywordCasing,
                IgnoreLockHintsOnIndexes = settings.IgnoreLockHintsOnIndexes,
                IgnoreLoginSids = settings.IgnoreLoginSids,
                IgnoreNotForReplication = settings.IgnoreNotForReplication,
                IgnoreObjectPlacementOnPartitionScheme = settings.IgnoreObjectPlacementOnPartitionScheme,
                IgnorePartitionSchemes = settings.IgnorePartitionSchemes,
                IgnorePermissions = settings.IgnorePermissions,
                IgnoreQuotedIdentifiers = settings.IgnoreQuotedIdentifiers,
                IgnoreRoleMembership = settings.IgnoreRoleMembership,
                IgnoreRouteLifetime = settings.IgnoreRouteLifetime,
                IgnoreSemicolonBetweenStatements = settings.IgnoreSemicolonBetweenStatements,
                IgnoreTableOptions = settings.IgnoreTableOptions,
                IgnoreUserSettingsObjects = settings.IgnoreUserSettingsObjects,
                IgnoreWhitespace = settings.IgnoreWhitespace,
                IgnoreWithNocheckOnCheckConstraints = settings.IgnoreWithNocheckOnCheckConstraints,
                IgnoreWithNocheckOnForeignKeys = settings.IgnoreWithNocheckOnForeignKeys,
                AllowUnsafeRowLevelSecurityDataMovement = settings.AllowUnsafeRowLevelSecurityDataMovement,
                IncludeCompositeObjects = settings.IncludeCompositeObjects,
                IncludeTransactionalScripts = settings.IncludeTransactionalScripts,
                NoAlterStatementsToChangeClrTypes = settings.NoAlterStatementsToChangeClrTypes,
                PopulateFilesOnFileGroups = settings.PopulateFilesOnFileGroups,
                RegisterDataTierApplication = settings.RegisterDataTierApplication,
                RunDeploymentPlanExecutors = settings.RunDeploymentPlanExecutors,
                ScriptDatabaseCollation = settings.ScriptDatabaseCollation,
                ScriptDatabaseCompatibility = settings.ScriptDatabaseCompatibility,
                ScriptDatabaseOptions = settings.ScriptDatabaseOptions,
                ScriptDeployStateChecks = settings.ScriptDeployStateChecks,
                ScriptFileSize = settings.ScriptFileSize,
                ScriptNewConstraintValidation = settings.ScriptNewConstraintValidation,
                ScriptRefreshModule = settings.ScriptRefreshModule,
                TreatVerificationErrorsAsWarnings = settings.TreatVerificationErrorsAsWarnings,
                UnmodifiableObjectWarnings = settings.UnmodifiableObjectWarnings,
                VerifyCollationCompatibility = settings.VerifyCollationCompatibility,
                VerifyDeployment = settings.VerifyDeployment
            };

            return options;
        }
    }
}