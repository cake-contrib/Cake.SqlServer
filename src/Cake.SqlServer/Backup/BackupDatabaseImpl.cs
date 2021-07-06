﻿#if NET5_0
using System;
#endif
using System.IO;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.SqlServer
{
    internal static class BackupDatabaseImpl
    {
        internal static void Execute(ICakeContext context, string connectionString, string databaseName, BackupDatabaseSettings settings)
        {
            Initializer.InitializeNativeSearchPath();
            Guard.ArgumentIsNotNull(context, nameof(context));
            Guard.ArgumentIsNotNull(connectionString, nameof(connectionString));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            var backupFile = GetBackupFileName(databaseName, settings.Path);
            var compress = settings.Compress ? ", COMPRESSION" : string.Empty;

            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, connectionString))
            {
                context.Log.Information($"Backing up database '{databaseName}' to {backupFile}");

                var sql = $@"
                    BACKUP DATABASE {Sql.EscapeName(databaseName)}
                    TO DISK = @backupFile
                    WITH FORMAT, INIT, COPY_ONLY, NAME = '{databaseName} Full Backup',
                    SKIP, REWIND, NOUNLOAD, STATS = 10 {compress}";

                context.Log.Information(sql);

                using (var command = SqlServerAliasesImpl.CreateSqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@backupFile", backupFile);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string GetBackupFileName(string databaseName, string? path)
        {
#if NET5_0
            var fileName = databaseName
                .Replace("[", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("]", string.Empty, StringComparison.OrdinalIgnoreCase) + ".bak";
#else
            var fileName = databaseName
                .Replace("[", string.Empty)
                .Replace("]", string.Empty) + ".bak";
#endif
            if (string.IsNullOrWhiteSpace(path))
                return fileName;

            return Directory.Exists(path!)
                ? Path.Combine(path!, fileName)
                : path!;
        }
    }
}
