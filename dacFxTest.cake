#addin nuget:https://myget.org/f/cake-sqlserver/?package=Cake.SqlServer
#addin "System.Data.SqlClient"
using System.Data.SqlClient

var target = Argument("target", "Default");


Setup(context =>
{
    Information("Starting integration tests");
});

Teardown(context =>
{
    Information("Finished with integration tests");
});
Task("Debug")
    .Does(() => 
    {
        Information("Welcome to debug");
    });



Task("Create-Bacpac")
    .Does(() =>{
        var connString = @"data source=(localdb)\MSSqlLocalDb";

        var dbName = "ForBacpac";

        using (var conn = new SqlConnection(connString))
        {
            conn.Open();
            using(var command = new SqlCommand("create database ForBacpac", conn))
            command.ExecuteNonQuery();
        }
        // CreateDatabase(connString, dbName);

        CreateBacpacFile(connString, "ForBacpac", new FilePath(@".\ForBacpac.bacpac"));
    })
    .Finally(() =>
    {  
        // Cleanup
        // DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "ForBacpac");
        using (var conn = new SqlConnection(@"data source=(localdb)\MSSqlLocalDb"))
        {
            conn.ExecuteSqlCommand("drop database ForBacpac");
        }

        if(FileExists(@".\ForBacpac.bacpac"))
        {
            DeleteFile(@".\ForBacpac.bacpac");
        }
    });


// Task("Restore-From-Bacpac")
//     .Does(() =>{
//         var connString = @"data source=(localdb)\MSSqlLocalDb";

//         var dbName = "FromBacpac";

//         var file = new FilePath(@".\src\Tests\Nsaga.bacpac");
//         RestoreBacpac(connString, dbName, file);
//     })
//     .Finally(() =>
//     {  
//         // Cleanup
//         DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "FromBacpac");
//     });


// Task("Dacpac-Publish")
//     .Does(() => 
//     {
//         var connectionString = @"data source=(localdb)\MSSqlLocalDb";
//         var dbName = "DacpacTestDb";
//         var dacpacFile = new FilePath(@".\src\Tests\Nsaga.dacpac");

//         CreateDatabase(connectionString, dbName);

//         PublishDacpacFile(connectionString, dbName, dacpacFile);
//     })
//     .Finally(() => 
//     {
//         // Cleanup
//         DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "DacpacTestDb");
//     });


// Task("Dacpac-Extract")
//     .Does(() => 
//     {
//         var dbName = "DacpacTestDb";
//         var connectionString = @"data source=(localdb)\MSSqlLocalDb";
//         var resultingFilePath = "NsagaCreated.dacpac";
//         var settings = new ExtractDacpacSettings("TestApp", "1.0.0.0") { OutputFile = resultingFilePath };

//         CreateDatabase(connectionString, dbName);
//         var sql = $@"use {dbName}
//         GO
//         create table [{dbName}].[dbo].[Table1] (id int null, name nvarchar(max) null);
//         Go";

//         ExecuteSqlCommand(connectionString, sql);

//         ExtractDacpacFile(connectionString, dbName, settings);
//     })
//     .Finally(() => 
//     {
//         DropDatabase(@"data source=(localdb)\MSSqlLocalDb", "DacpacTestDb");
//         //if(FileExists(@".\NsagaCreated.dacpac"))
//         {
//             DeleteFile(@".\NsagaCreated.dacpac");
//         }
    
//     });


Task("Default")
    .IsDependentOn("Create-Bacpac")
    .IsDependentOn("Restore-From-Bacpac")
    .IsDependentOn("Dacpac-Extract")
    .IsDependentOn("Dacpac-Publish")
    .IsDependentOn("Create-With-Parameters")
    ;    

RunTarget(target);
