#r "build-results/IntergrationTests/Cake.SqlServer.dll"
//#r "src/Cake.SqlServer/bin/debug/Cake.SqlServer.dll"

var target = Argument("target", "Default");


Setup(context =>
{
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
	    var masterConnectionString = @"data source=(LocalDb)\v12.0;";
	    var connectionString = @"data source=(LocalDb)\v12.0;Database=CakeTest";

		var dbName = "CakeTest";

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


Task("Default")
    .IsDependentOn("Create-LocalDB")
    .IsDependentOn("Start-LocalDB")
    .IsDependentOn("Stop-LocalDB")
    .IsDependentOn("Delete-LocalDB")
    .IsDependentOn("Database-Operations")
    ;    

RunTarget(target);