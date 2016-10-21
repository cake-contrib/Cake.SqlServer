#r "build-results/IntergrationTests/Cake.SqlServer.dll"

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
        LocalDbCreateInstance("Cake-Test", LocalDbVersion.V11);
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


Task("Default")
    .IsDependentOn("Create-LocalDB")
    .IsDependentOn("Start-LocalDB")
    .IsDependentOn("Stop-LocalDB")
    .IsDependentOn("Delete-LocalDB")
    ;    

RunTarget(target);