using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Diagnostics;
using System.Text;

namespace Cake.SqlServer
{
    internal static class RestoreSqlBackupImpl
    {
        // if database name is not provided, dbname from the backup is used.
        // if newStoragePath is not provided, system defaults are used
        internal static void RestoreSqlBackup(ICakeContext context, String connectionString, RestoreSqlBackupSettings settings, IList<FilePath> backupFiles, IList<FilePath> differentialBackupFiles = null)
        {
            Initializer.InitializeNativeSearchPath(context);
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, connectionString))
            {
                var firstBackupFile = backupFiles.First();
                var oldDbName = GetDatabaseName(firstBackupFile, connection);
                var databaseName = settings.NewDatabaseName ?? oldDbName;
                if (settings.SwitchToUserMode != DbUserMode.MultiUser)
                {
                    var singleModeCommand = GetSetDatabaseUserModeCommand(context, connection, databaseName, settings.SwitchToUserMode);
                    singleModeCommand.ExecuteNonQuery();
                }
                var hasDifferentialBackup = differentialBackupFiles != null && differentialBackupFiles.Any();
                var fullRestoreCommand = GetRestoreSqlBackupCommand(
                    context,
                    connection,
                    settings.BackupSetFile,
                    settings.WithReplace,
                    hasDifferentialBackup,
                    databaseName,
                    settings.NewStorageFolder,
                    backupFiles.ToArray());
                fullRestoreCommand.ExecuteNonQuery();
                if (hasDifferentialBackup)
                {
                    var differentialRestoreCommand = GetRestoreSqlBackupCommand(
                        context,
                        connection,
                        settings.DifferentialBackupSetFile,
                        false,
                        false,
                        databaseName,
                        settings.NewStorageFolder,
                        differentialBackupFiles.ToArray());
                    differentialRestoreCommand.ExecuteNonQuery();
                }
                if (settings.SwitchToUserMode != DbUserMode.MultiUser)
                {
                    var singleModeCommand = GetSetDatabaseUserModeCommand(context, connection, databaseName, DbUserMode.MultiUser);
                    singleModeCommand.ExecuteNonQuery();
                }
            }
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
            sb.AppendLine($"Restore database {Sql.EscapeName(databaseName)}");
            sb.AppendLine($"from");
            for (var i = 0; i < backupFiles.Length; i++)
            {
                // only need comma before penultimate list
                var trailingComma = i < backupFiles.Length - 1 ? "," : "";
                sb.AppendLine($"  disk = @backupFile{i}{trailingComma}");
            }
            sb.AppendLine($"with");
            if (backupSetFile.HasValue)
            {
                sb.AppendLine($"  file = {backupSetFile.Value},");
            }

            for (var i = 0; i < logicalNames.Count; i++)
            {
                sb.AppendLine($"  move @LName{i} to @LPath{i},");
            }

            if (withReplace)
            {
                sb.AppendLine($"  replace,");
            }

            if (withNoRecovery)
            {
                sb.AppendLine($"  norecovery");
            }
            else
            {
                sb.AppendLine($"  recovery");
            }
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

        private static SqlCommand GetSetDatabaseUserModeCommand(ICakeContext context, SqlConnection connection, String databaseName, DbUserMode userMode)
        {
            var sb = new StringBuilder();
            sb.AppendLine();

            switch (userMode)
            {
                case DbUserMode.SingleUser:
                    sb.AppendLine($@"if db_id({Sql.EscapeNameQuotes(databaseName)}) is not null");
                    sb.AppendLine($@"begin");
                    sb.AppendLine($@"    use master;");
                    sb.AppendLine($@"    alter database {Sql.EscapeName(databaseName)} set single_user with rollback immediate;");
                    sb.AppendLine($@"end");
                    sb.AppendLine($@";");
                    break;
                case DbUserMode.RestrictedUser:
                    sb.AppendLine($@"if db_id({Sql.EscapeNameQuotes(databaseName)}) is not null");
                    sb.AppendLine($@"begin");
                    sb.AppendLine($@"    use master;");
                    sb.AppendLine($@"    alter database {Sql.EscapeName(databaseName)} set restricted_user with rollback immediate;");
                    sb.AppendLine($@"end");
                    sb.AppendLine($@";");
                    break;
                default:
                    sb.AppendLine($@"if db_id({Sql.EscapeNameQuotes(databaseName)}) is not null");
                    sb.AppendLine($@"begin");
                    sb.AppendLine($@"    use master;");
                    sb.AppendLine($@"    alter database {Sql.EscapeName(databaseName)} set multi_user;");
                    sb.AppendLine($@"end");
                    sb.AppendLine($@";");
                    break;
            }

            context.Log.Debug($"Executing SQL : {sb}");
            var command = SqlServerAliasesImpl.CreateSqlCommand(sb.ToString(), connection);

            return command;
        }

        private static char GetPlatformPathSeparator(SqlConnection connection)
        {
            var sql = "select @@version as version";

            var version = ReadSingleString(sql, "version", connection).ToLower();

            if (version.Contains("linux"))
            {
                return '/';
            }

            return '\\';
        }

        internal static List<LogicalNames> GetLogicalNames(FilePath backupFile, SqlConnection connection)
        {
            var fileListSql = @"RESTORE FILELISTONLY from Disk = @backupFile";
            var command = SqlServerAliasesImpl.CreateSqlCommand(fileListSql, connection);
            command.Parameters.AddWithValue("@backupFile", backupFile.ToString());
            var logicalNames = new List<LogicalNames>();
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    throw new Exception($"Unable to read logical names from the backup at {backupFile}. Are you sure it is SQL backup file?");
                }
                while (reader.Read())
                {
                    var name = new LogicalNames()
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


        internal static String GetDatabaseName(FilePath backupFile, SqlConnection connection)
        {
            var sql = @"restore headeronly from disk = @backupFile";
            var command = SqlServerAliasesImpl.CreateSqlCommand(sql, connection);
            command.Parameters.AddWithValue("@backupFile", backupFile.ToString());

            var dbName = ReadSingleString(command, "DatabaseName");

            return dbName;
        }


        private static String GetFilePath(SqlConnection connection, string oldDatabaseName, string newDatabaseName, DirectoryPath newPath, LogicalNames logicalName, char pathSeparator)
        {
            var fileName = System.IO.Path.GetFileName(logicalName.PhysicalName);
            fileName = fileName?.Replace(oldDatabaseName, newDatabaseName);

            var folder = newPath;

            if (logicalName.Type.Equals("L", StringComparison.OrdinalIgnoreCase))
            {
                folder = folder ?? GetDefaultLogPath(connection);
            }
            if (logicalName.Type.Equals("D", StringComparison.OrdinalIgnoreCase))
            {
                folder = folder ?? GetDefaultDataPath(connection);
            }

            var fullPath = folder.FullPath + pathSeparator + fileName;

            if (pathSeparator == '\\')
            {
                return fullPath.Replace("/", "\\");
            }
            else
            {
                return fullPath.Replace("\\", "/");
            }

        }


        internal static String GetDefaultLogPath(SqlConnection connection)
        {
            var sql = "select serverproperty('instancedefaultlogpath') as defaultpath";

            var defaultPath = ReadSingleString(sql, "defaultpath", connection);

            return defaultPath;
        }


        internal static String GetDefaultDataPath(SqlConnection connection)
        {
            var sql = "select serverproperty('instancedefaultdatapath') as defaultpath";

            var defaultPath = ReadSingleString(sql, "defaultpath", connection);

            return defaultPath;
        }
        private static string ReadSingleString(String sql, String columnName, SqlConnection connection)
        {
            var command = SqlServerAliasesImpl.CreateSqlCommand(sql, connection);
            return ReadSingleString(command, columnName);
        }


        private static string ReadSingleString(SqlCommand sqlCommand, String columnName)
        {
            using (var reader = sqlCommand.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    throw new Exception($"Unable to read data as no raws was returned");
                }
                reader.Read();
                var value = reader.GetString(reader.GetOrdinal(columnName));
                return value;
            }
        }



        internal class LogicalNames
        {
            public String LogicalName { get; set; }
            public String PhysicalName { get; set; }
            public String Type { get; set; }
        }
    }
}
