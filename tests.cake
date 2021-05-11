//#tool nuget:?package=Microsoft.Data.SqlClient.SNI&loaddependencies=true
#addin nuget:https://myget.org/f/cake-sqlserver/?package=Cake.SqlServer&loaddependencies=true
// #r "src/Cake.SqlServer/bin/release/Cake.SqlServer.dll"

var target = Argument("target", "Default");

public static class NativeMethods
{
    public const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;
	
    [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    public static extern bool SetDefaultDllDirectories(uint DirectoryFlags);

    [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
    public static extern int AddDllDirectory(string NewDirectory);
}

Setup(context =>
{
    var res = context.Tools.Resolve("Microsoft.Data.SqlClient.SNI.x64.dll").GetDirectory().FullPath;
    NativeMethods.SetDefaultDllDirectories(NativeMethods.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
    NativeMethods.AddDllDirectory(res);
    Information("Starting integration tests");
});

Teardown(context =>
{
    Information("Finished with integration tests");
});



Task("Create-LocalDB")
     .Does(() =>
     {
        // creates and starts instance
        // you don't need to start the instance separately
        //LocalDbCreateInstance("Cake-Test", LocalDbVersion.V12);
         LocalDbCreateInstance("Cake-Test");
     });


Task("Start-LocalDB")
     .Does(() =>
     {
        LocalDbStartInstance("Cake-Test");
    });


Task("Stop-LocalDB")
     .Does(() =>
     {
        LocalDbStopInstance("Cake-Test");
    });


Task("Delete-LocalDB")
     .Does(() =>
     {
        LocalDbDeleteInstance("Cake-Test");
    });

Task("Debug")
    .Does(() =>
    {
        Information("Welcome to debug");
    });


Task("Database-Operations")
    .Does(() =>
    {
        var masterConnectionString = @"data source=(localdb)\MSSqlLocalDb;";
        var connectionString = @"data source=(localdb)\MSSqlLocalDb;Database=OpsCakeTest";

        var dbName = "OpsCakeTest";

        // drop the db to be sure
        DropDatabase(masterConnectionString, dbName);

        // first create database
        CreateDatabase(masterConnectionString, dbName);

        // try the database again
        CreateDatabaseIfNotExists(masterConnectionString, dbName);

        // and recreate the db again
        DropAndCreateDatabase(masterConnectionString, dbName);

        // and create some tables
        ExecuteSqlCommand(connectionString, "create table dbo.Products(id int null)");

        // and execute sql from a file
        ExecuteSqlFile(connectionString, "install.sql");

        // then drop the database
        DropDatabase(masterConnectionString, dbName);
    });

Task("Create-With-Parameters")
    .WithCriteria(() => !BuildSystem.AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        // excluding these tests from appveyor because they are causing timeout on their SQL
        var masterConnectionString = @"data source=(localdb)\MSSqlLocalDb;";

        var dbName = "CreateCakeTest";
        var mdfFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "MyCakeTest.mdf");
        var logFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "MyCakeTest.ldf");

        var createSettings = new CreateDatabaseSettings().WithPrimaryFile(mdfFilePath).WithLogFile(logFilePath);

        // making sure we use DatabaseExists feature here
        if(DatabaseExists(masterConnectionString, dbName))
        {
            DropDatabase(masterConnectionString, dbName);
        }

        CreateDatabase(masterConnectionString, dbName, createSettings);

        DropAndCreateDatabase(masterConnectionString, dbName, createSettings);

        CreateDatabaseIfNotExists(masterConnectionString, dbName, createSettings);
    })
    .Finally(() =>
    {
        // Cleanup
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "CreateCakeTest");
    });


Task("SqlConnection")
    .Does(() => {
        var masterConnectionString = @"data source=(localdb)\MSSqlLocalDb;";
        var connectionString = @"data source=(localdb)\MSSqlLocalDb;Database=OpenConnection";

        var dbName = "OpenConnection";

        CreateDatabase(masterConnectionString, dbName);

        using (var connection = OpenSqlConnection(connectionString))
        {
            ExecuteSqlCommand(connection, "create table dbo.Products(id int null)");

            ExecuteSqlFile(connection, "install.sql");
        }

    })
    .Finally(() =>
    {
        // Cleanup
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb;", "OpenConnection");
    });


Task("SqlTimeout")
    .Does(() => {
        SetSqlCommandTimeout(3);
        using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;"))
        {
            ExecuteSqlCommand(connection, "WAITFOR DELAY '00:00:02'");
        }
    });


Task("Restore-Database")
    .Does(() => {
        var connString = @"data source=(localdb)\MSSqlLocalDb";

        var backupFilePath = new FilePath(@".\src\Tests\TestData\multiFileBackup.bak");
        backupFilePath = backupFilePath.MakeAbsolute(Context.Environment);

        RestoreSqlBackup(connString, backupFilePath);

        RestoreSqlBackup(connString, backupFilePath, new RestoreSqlBackupSettings()
            {
                NewDatabaseName = "RestoredFromTest.Cake",
                NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()), // place files in special location
            });
    })
    .Finally(() =>
    {
        // Cleanup
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "RestoredFromTest.Cake");
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "CakeRestoreTest");
    });

Task("Backup-Database")
    .Does(() => {
		var connString = @"data source=(localdb)\MSSqlLocalDb";

        var backupFilePath = new FilePath(@".\src\Tests\TestData\multiFileBackup.bak");
        backupFilePath = backupFilePath.MakeAbsolute(Context.Environment);

        RestoreSqlBackup(connString, backupFilePath);

		var databaseName = "CakeRestoreTest";
		BackupDatabase(connString, databaseName, new BackupDatabaseSettings()
		{
			Compress = false,
			// you can specify a folder or a file
			Path = System.IO.Path.GetTempPath()
		});
	})
    .Finally(() =>
    {
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "CakeRestoreTest");
    });


Task("Create-Bacpac")
    .Does(() =>{
        var connString = @"data source=(localdb)\MSSqlLocalDb";

        var dbName = "ForBacpac";

        CreateDatabase(connString, dbName);

        CreateBacpacFile(connString, dbName, new FilePath(@".\ForBacpac.bacpac"));
    })
    .Finally(() =>
    {
        // Cleanup
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "ForBacpac");
        if(FileExists(@".\ForBacpac.bacpac"))
        {
            DeleteFile(@".\ForBacpac.bacpac");
        }
    });


Task("Restore-From-Bacpac")
    .Does(() =>{
        var connString = @"data source=(localdb)\MSSqlLocalDb";

        var dbName = "FromBacpac";

        var file = new FilePath(@".\src\Tests\TestData\Nsaga.bacpac");
        RestoreBacpac(connString, dbName, file);
    })
    .Finally(() =>
    {
        // Cleanup
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "FromBacpac");
    });


Task("Dacpac-Publish")
    .Does(() =>
    {
        var connectionString = @"data source=(localdb)\MSSqlLocalDb";
        var dbName = "DacpacTestDb";
        var dacpacFile = new FilePath(@".\src\Tests\TestData\Nsaga.dacpac");

        CreateDatabase(connectionString, dbName);

        PublishDacpacFile(connectionString, dbName, dacpacFile);
    })
    .Finally(() =>
    {
        // Cleanup
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "DacpacTestDb");
    });


Task("Dacpac-Extract")
    .Does(() =>
    {
        var dbName = "DacpacTestDb";
        var connectionString = @"data source=(localdb)\MSSqlLocalDb";
        var resultingFilePath = "NsagaCreated.dacpac";
        var settings = new ExtractDacpacSettings("TestApp", "1.0.0.0") { OutputFile = resultingFilePath };

        CreateDatabase(connectionString, dbName);
        var sql = $@"use {dbName}
        GO
        create table [{dbName}].[dbo].[Table1] (id int null, name nvarchar(max) null);
        Go";

        ExecuteSqlCommand(connectionString, sql);

        ExtractDacpacFile(connectionString, dbName, settings);
    })
    .Finally(() =>
    {
        DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "DacpacTestDb");
        //if(FileExists(@".\NsagaCreated.dacpac"))
        {
            DeleteFile(@".\NsagaCreated.dacpac");
        }

    });


Task("Default")
    .IsDependentOn("Create-LocalDB")
    .IsDependentOn("Start-LocalDB")
    .IsDependentOn("Stop-LocalDB")
    .IsDependentOn("Delete-LocalDB")
    .IsDependentOn("Database-Operations")
    .IsDependentOn("SqlConnection")
    .IsDependentOn("SqlTimeout")
    .IsDependentOn("Restore-Database")
    .IsDependentOn("Backup-Database")
    .IsDependentOn("Create-Bacpac")
    .IsDependentOn("Restore-From-Bacpac")
    .IsDependentOn("Dacpac-Extract")
    .IsDependentOn("Dacpac-Publish")
    .IsDependentOn("Create-With-Parameters")
    ;

RunTarget(target);
