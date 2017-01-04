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
                newDatabaseName = newDatabaseName ?? GetDatabaseName(backupFile, connection);
                var logicalNames = GetLogicalNames(backupFile, connection);
                var storageFolders = new StorageFolders(connection, newDatabaseName, newStorageFolder);

                var sql = $"Restore database {Sql.EscapeName(newDatabaseName)} from disk = @backupFile with \r\n";

                for (var i = 0; i < logicalNames.Count; i++)
                {
                    sql += $" move @LName{i} to @LPath{i}";
                    if (i < logicalNames.Count - 1)
                    {
                        sql += ", \r\n";
                    }
                }

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@backupFile", backupFile.ToString());
                for (var i = 0; i < logicalNames.Count; i++)
                {
                    command.Parameters.AddWithValue($"@LName" + i, logicalNames[i].LogicalName);
                    command.Parameters.AddWithValue($"@LPath" + i, storageFolders.GetPath(logicalNames[i].Type));
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


        internal class StorageFolders
        {
            private readonly DirectoryPath newPath;
            private readonly String databaseName;
            private readonly SqlConnection connection;

            public StorageFolders(SqlConnection connection, string databaseName, DirectoryPath newPath)
            {
                this.databaseName = databaseName;
                this.newPath = newPath;
                this.connection = connection;
            }

            public String GetPath(String logicalType)
            {
                if (logicalType.Equals("L", StringComparison.OrdinalIgnoreCase))
                {
                    return LogPath;
                }
                if (logicalType.Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    return DataPath;
                }

                throw new Exception($"Unable to determine type of logical name: {logicalType}");
            }

            public String LogPath
            {
                get
                {
                    var folder = newPath ?? GetDefaultLogPath(connection);
                    var file = folder + "\\" + databaseName + ".ldf";
                    return file.Replace("/", "\\");
                }
            }

            public String DataPath
            {
                get
                {
                    var folder = newPath ?? GetDefaultDataPath(connection);
                    var file = folder + "\\" + databaseName + ".mdf";
                    return file.Replace("/", "\\");
                }
            }
        }
    }
}
