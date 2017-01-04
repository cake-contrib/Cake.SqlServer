using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cake.Core;
using Cake.Core.IO;


namespace Cake.SqlServer
{
    // todo logging
    internal class RestoreDatabaseImpl
    {
        // if database name is not provided, dbname from the backup is used.
        // if newStoragePath is not provided, system defaults are used
        internal static void RestoreDatabase(ICakeContext context, String connectionString, FilePath backupFile, string newDatabaseName = null, DirectoryPath newStorageFolder = null)
        {
            using (var connection = SqlServerAliasesImpl.OpenSqlConnection(context, connectionString))
            {
                var oldDbName = GetDatabaseName(backupFile, connection);
                newDatabaseName = newDatabaseName ?? oldDbName;
                var logicalNames = GetLogicalNames(backupFile, connection);

                var sql = $"Restore database {Sql.EscapeName(newDatabaseName)} from disk = @backupFile with \r\n";

                for (var i = 0; i < logicalNames.Count; i++)
                {
                    sql += $" move @LName{i} to @LPath{i}";
                    if (i < logicalNames.Count - 1)
                    {
                        sql += ", \r\n"; // only need comma before penultimate list
                    }
                }

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@backupFile", backupFile.ToString());
                for (var i = 0; i < logicalNames.Count; i++)
                {
                    command.Parameters.AddWithValue("@LName" + i, logicalNames[i].LogicalName);
                    command.Parameters.AddWithValue("@LPath" + i, GetFilePath(connection, oldDbName, newDatabaseName, newStorageFolder, logicalNames[i]));
                }
                command.ExecuteNonQuery();
            }
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


        private static String GetFilePath(SqlConnection connection, string newDatabaseName, string oldDatabaseName, DirectoryPath newPath, LogicalNames logicalName)
        {
            var fileName = System.IO.Path.GetFileName(logicalName.PhysicalName);
            fileName = fileName.Replace(oldDatabaseName, newDatabaseName);

            var folder = newPath;

            if (logicalName.Type.Equals("L", StringComparison.OrdinalIgnoreCase))
            {
                folder = folder ?? GetDefaultLogPath(connection);
            }
            if (logicalName.Type.Equals("D", StringComparison.OrdinalIgnoreCase))
            {
                folder = folder ?? GetDefaultDataPath(connection);
            }

            var fullPath = folder + "\\" + fileName;

            return fullPath.Replace("/", "\\");
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
