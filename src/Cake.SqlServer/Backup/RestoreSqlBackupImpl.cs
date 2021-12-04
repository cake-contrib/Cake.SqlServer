using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Microsoft.Data.SqlClient;

namespace Cake.SqlServer
{
    internal static class RestoreSqlBackupImpl
    {
        // if database name is not provided, dbname from the backup is used.
        // if newStoragePath is not provided, system defaults are used
        internal static void RestoreSqlBackup(
            ICakeContext context,
            string connectionString,
            RestoreSqlBackupSettings settings,
            IList<FilePath> backupFiles,
            IList<FilePath>? differentialBackupFiles = null)
        {
            Initializer.InitializeNativeSearchPath();
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, connectionString))
            {
                var firstBackupFile = backupFiles.First();
                var oldDbName = GetDatabaseName(firstBackupFile, connection);
                var databaseName = settings.NewDatabaseName ?? oldDbName;
                if (settings.SwitchToUserMode != DbUserMode.MultiUser)
                {
                    using (var singleModeCommand = GetSetDatabaseUserModeCommand(
                        context,
                        connection,
                        databaseName,
                        settings.SwitchToUserMode))
                    {
                        singleModeCommand.ExecuteNonQuery();
                    }
                }

                var hasDifferentialBackup = differentialBackupFiles?.Count > 0;
                using (var fullRestoreCommand = GetRestoreSqlBackupCommand(
                    context,
                    connection,
                    settings.BackupSetFile,
                    settings.WithReplace,
                    hasDifferentialBackup,
                    databaseName,
                    settings.NewStorageFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    backupFiles.ToArray()))
                {
                    fullRestoreCommand.ExecuteNonQuery();
                }

                if (hasDifferentialBackup)
                {
                    using (var differentialRestoreCommand = GetRestoreSqlBackupCommand(
                        context,
                        connection,
                        settings.DifferentialBackupSetFile,
                        false,
                        false,
                        databaseName,
                        settings.NewStorageFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        differentialBackupFiles?.ToArray() ?? Array.Empty<FilePath>()))
                    {
                        differentialRestoreCommand.ExecuteNonQuery();
                    }
                }

                if (settings.SwitchToUserMode != DbUserMode.MultiUser)
                {
                    using (var singleModeCommand =
                        GetSetDatabaseUserModeCommand(context, connection, databaseName, DbUserMode.MultiUser))
                    {
                        singleModeCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        internal static List<LogicalNames> GetLogicalNames(FilePath backupFile, SqlConnection connection)
        {
            const string fileListSql = "RESTORE FILELISTONLY from Disk = @backupFile";
            using (var command = SqlServerAliasesImpl.CreateSqlCommand(fileListSql, connection))
            {
                command.Parameters.AddWithValue("@backupFile", backupFile.ToString());
                var logicalNames = new List<LogicalNames>();
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        throw new InvalidBackupFileException(
                            $"Unable to read logical names from the backup at {backupFile}. Are you sure it is SQL backup file?");
                    }

                    while (reader.Read())
                    {
                        var name = new LogicalNames
                        {
                            LogicalName = reader.GetString(reader.GetOrdinal("LogicalName")),
                            PhysicalName = reader.GetString(reader.GetOrdinal("PhysicalName")),
                            Type = reader.GetString(reader.GetOrdinal("Type")),
                        };

                        logicalNames.Add(name);
                    }
                }

                return logicalNames;
            }
        }

        internal static string GetDatabaseName(FilePath backupFile, SqlConnection connection)
        {
            const string sql = "restore headeronly from disk = @backupFile";
            using (var command = SqlServerAliasesImpl.CreateSqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@backupFile", backupFile.ToString());

                return ReadSingleString(command, "DatabaseName");
            }
        }

        internal static string GetDefaultLogPath(SqlConnection connection)
        {
            const string sql = "select serverproperty('instancedefaultlogpath') as defaultpath";

            var defaultPath = ReadSingleString(sql, "defaultpath", connection);

            return defaultPath;
        }

        internal static string GetDefaultDataPath(SqlConnection connection)
        {
            const string sql = "select serverproperty('instancedefaultdatapath') as defaultpath";

            var defaultPath = ReadSingleString(sql, "defaultpath", connection);

            return defaultPath;
        }

        // This method is used to determine one single "Restore database" command.
        private static SqlCommand GetRestoreSqlBackupCommand(
            ICakeContext context,
            SqlConnection connection,
            int? backupSetFile,
            bool withReplace,
            bool withNoRecovery,
            string newDatabaseName,
            DirectoryPath newStorageFolder,
            params FilePath[] backupFiles)
        {
            var firstBackupFile = backupFiles.First();
            var oldDbName = GetDatabaseName(firstBackupFile, connection);
            var databaseName = newDatabaseName ?? oldDbName;
            context.Log.Information($"Using database name '{databaseName}' to be a name for the restored database");

            var logicalNames = GetLogicalNames(firstBackupFile, connection);

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("Restore database ").AppendLine(Sql.EscapeName(databaseName));
            sb.AppendLine(" from");
            for (var i = 0; i < backupFiles.Length; i++)
            {
                // only need comma before penultimate list
                var trailingComma = i < backupFiles.Length - 1 ? "," : string.Empty;
                sb.Append("  disk = @backupFile").Append(i).AppendLine(trailingComma);
            }

            sb.AppendLine("with");
            if (backupSetFile.HasValue)
            {
                sb.Append("  file = ").Append(backupSetFile.Value).AppendLine(",");
            }

            for (var i = 0; i < logicalNames.Count; i++)
            {
                var strIndex = i.ToString(CultureInfo.InvariantCulture);
                sb.Append("  move @LName").Append(strIndex).Append(" to @LPath").Append(strIndex).AppendLine(",");
            }

            if (withReplace)
            {
                sb.AppendLine("  replace,");
            }

            sb.AppendLine(withNoRecovery ? "  norecovery" : "  recovery");
            context.Log.Debug($"Executing SQL : {sb}");

            var pathSeparator = GetPlatformPathSeparator(connection);
            var command = SqlServerAliasesImpl.CreateSqlCommand(sb.ToString(), connection);
            for (var i = 0; i < backupFiles.Length; i++)
            {
                command.Parameters.AddWithValue($"@backupFile{i}", backupFiles[i].ToString());
            }

            for (var i = 0; i < logicalNames.Count; i++)
            {
                var lParameterName = "@LName" + i;
                context.Log.Debug($"Adding parameter '{lParameterName}' with value '{logicalNames[i].LogicalName}'");
                command.Parameters.AddWithValue(lParameterName, logicalNames[i].LogicalName);

                var filePath = GetFilePath(connection, oldDbName, databaseName, newStorageFolder, logicalNames[i], pathSeparator);
                var pathParamName = "@LPath" + i;
                context.Log.Debug($"Adding parameter '{pathParamName}' with value '{filePath}'");
                command.Parameters.AddWithValue(pathParamName, filePath);
            }

            return command;
        }

        private static SqlCommand GetSetDatabaseUserModeCommand(ICakeContext context, SqlConnection connection, string databaseName, DbUserMode userMode)
        {
            var sb = new StringBuilder();
            sb.AppendLine();

            switch (userMode)
            {
                case DbUserMode.SingleUser:
                    sb.Append("if db_id(").Append(Sql.EscapeNameQuotes(databaseName)).AppendLine(") is not null");
                    sb.AppendLine("begin");
                    sb.AppendLine("    use master;");
                    sb.Append("    alter database ").Append(Sql.EscapeName(databaseName)).AppendLine(" set single_user with rollback immediate;");
                    sb.AppendLine("end");
                    sb.AppendLine(";");
                    break;
                case DbUserMode.RestrictedUser:
                    sb.Append("if db_id(").Append(Sql.EscapeNameQuotes(databaseName)).AppendLine(") is not null");
                    sb.AppendLine("begin");
                    sb.AppendLine("    use master;");
                    sb.Append("    alter database ").Append(Sql.EscapeName(databaseName)).AppendLine(" set restricted_user with rollback immediate;");
                    sb.AppendLine("end");
                    sb.AppendLine(";");
                    break;
                default:
                    sb.Append("if db_id(").Append(Sql.EscapeNameQuotes(databaseName)).AppendLine(") is not null");
                    sb.AppendLine("begin");
                    sb.AppendLine("    use master;");
                    sb.Append("    alter database ").Append(Sql.EscapeName(databaseName)).AppendLine(" set multi_user;");
                    sb.AppendLine("end");
                    sb.AppendLine(";");
                    break;
            }

            context.Log.Debug($"Executing SQL : {sb}");
            var command = SqlServerAliasesImpl.CreateSqlCommand(sb.ToString(), connection);

            return command;
        }

        private static char GetPlatformPathSeparator(SqlConnection connection)
        {
            const string sql = "select @@version as version";

            var version = ReadSingleString(sql, "version", connection).ToLower(CultureInfo.InvariantCulture);

            if (version.Contains("linux", StringComparison.OrdinalIgnoreCase))
            {
                return '/';
            }

            return '\\';
        }

        private static string GetFilePath(
            SqlConnection connection,
            string oldDatabaseName,
            string newDatabaseName,
            DirectoryPath newPath,
            LogicalNames logicalName,
            char pathSeparator)
        {
            var fileName = System.IO.Path.GetFileName(logicalName.PhysicalName);
            fileName = fileName?.Replace(oldDatabaseName, newDatabaseName, StringComparison.OrdinalIgnoreCase);

            var folder = newPath;

            if (logicalName.Type?.Equals("L", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                folder ??= GetDefaultLogPath(connection);
            }

            if (logicalName.Type?.Equals("D", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                folder ??= GetDefaultDataPath(connection);
            }

            var fullPath = folder.FullPath + pathSeparator + fileName;

            if (pathSeparator == '\\')
            {
                return fullPath.Replace("/", "\\", StringComparison.OrdinalIgnoreCase);
            }

            return fullPath.Replace("\\", "/", StringComparison.OrdinalIgnoreCase);
        }

        private static string ReadSingleString(string sql, string columnName, SqlConnection connection)
        {
            using (var command = SqlServerAliasesImpl.CreateSqlCommand(sql, connection))
            {
                return ReadSingleString(command, columnName);
            }
        }

        private static string ReadSingleString(SqlCommand sqlCommand, string columnName)
        {
            using (var reader = sqlCommand.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    throw new NoRowsException("Unable to read data as no rows was returned");
                }

                reader.Read();
                var value = reader.GetString(reader.GetOrdinal(columnName));
                return value;
            }
        }

        internal class LogicalNames
        {
            public string? LogicalName { get; set; }

            public string? PhysicalName { get; set; }

            public string? Type { get; set; }
        }
    }
}
