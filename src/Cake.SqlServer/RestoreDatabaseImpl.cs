using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    internal class RestoreDatabaseImpl
    {
        internal static void RestoreDatabase(ICakeContext context, String connectionString, string newDatabaseName, FilePath backupFile, DirectoryPath newStorageFolder)
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, connectionString))
            {
                var logicalNames = GetLogicalNames(backupFile, connection);
                var dbName = logicalNames.First(n => !n.EndsWith("_log"));
                var logName = logicalNames.First(n => n.EndsWith("_log"));

                var sql = $@"
Restore database {Sql.EscapeName(newDatabaseName)} from disk = @backupFile with 
move @dbName to @mdfPath,
move @logName to @ldfPath
";
                //TODO need to sort out paths - this is horrible
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@backupFile", backupFile.ToString());
                command.Parameters.AddWithValue("@dbName", dbName);
                command.Parameters.AddWithValue("@mdfPath", (newStorageFolder + "\\" + newDatabaseName + ".mdf").Replace("/", "\\"));

                command.Parameters.AddWithValue("@logName", logName);
                command.Parameters.AddWithValue("@ldfPath", (newStorageFolder + "\\" + newDatabaseName + ".ldf").Replace("/", "\\"));

                command.ExecuteNonQuery();
            }
        }

        private static List<string> GetLogicalNames(FilePath backupFile, SqlConnection connection)
        {
            var fileListSql = @"RESTORE FILELISTONLY from Disk = @backupFile";
            var command = new SqlCommand(fileListSql, connection);
            command.Parameters.AddWithValue("@backupFile", backupFile.ToString());
            var logicalNames = new List<String>();
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    throw new Exception(
                        $"Unable to read logical names from the backup at {backupFile}. Are you sure it is SQL backup file?");
                }
                while (reader.Read())
                {
                    var logicalName = reader.GetString(reader.GetOrdinal("LogicalName"));
                    logicalNames.Add(logicalName);
                }
            }
            return logicalNames;
        }
    }
}
