# Cake.SqlServer
 
Cake Build addin for working with SqlServer and LocalDb.

[![Build status](https://ci.appveyor.com/api/projects/status/wgbuq8pw180w42t2/branch/master?svg=true)](https://ci.appveyor.com/project/trailmax/cake-sqlserver/branch/master)
[![NuGet version](https://badge.fury.io/nu/cake.sqlserver.svg)](https://badge.fury.io/nu/cake.sqlserver)

Show me the codez: [integration tests](https://github.com/AMVSoftware/Cake.SqlServer/blob/master/tests.cake) for live up to date examples.  
Or look through XML-generated documentation on [Cakebuild.net site](http://cakebuild.net/dsl/sqlserver/).

* [General Functionality](#general-functionality)
* [Working With Backup files](#working-with-backup-files)
* [Working with BACPAC and DACPAC](#working-with-bacpac-and-dacpac)
* [Working with LocalDB](#working-with-localdb)
* [Usage](#usage)
* [Reason to Develop](#reason-to-develop)


# General Functionality

### Database Exists
```c#
DatabaseExists(string connectionString, string databaseName)
```
Return true if the database exists, false otherwise.

### Create Database
```c#
CreateDatabase(string connectionString, string databaseName)
```
Creates database. If database with this name exists - SqlException is thrown. 

```c#
var createSettings = new CreateDatabaseSettings()
                        .WithPrimaryFile(@"C:\MyPath\MyCakeTest.mdf")
                        .WithLogFile(@"C:\MyPath\MyCakeTest.ldf");
CreateDatabase(masterConnectionString, "CreateCakeTest", createSettings);
```
Creates a database with the specified primary and log files locations.

### Create Database If Not Exist
```c#
CreateDatabaseIfNotExists(string connectionString, string databaseName)
```

Here we check if the database exists first, if it does not exists - create it. Does not do anything if database with this name already exists.

```c#
var createSettings = new CreateDatabaseSettings()
                        .WithPrimaryFile(@"C:\MyPath\MyCakeTest.mdf")
                        .WithLogFile(@"C:\MyPath\MyCakeTest.ldf");
CreateDatabaseIfNotExists(masterConnectionString, "MyCakeTest", createSettings)
```
This will check if database does not yet exist and will create a new one. And will place primary data file and log file into specified locations.


### Drop Database
```c#
DropDatabase(string connectionString, string databaseName)
```

Basically executes `Drop Database databasename` with some fail-safes. Actually it sets the database into offline mode - to cut off all existing connections. Then sets the database back online and then drops it. 
Reason for this dance - you can't drop a database if there are existing connections to the database.  

### Drop and Create Database
```c#
DropAndCreateDatabase(String connectionString, String databaseName)
```

Simply a short-hand for `DropDatabase(); CreateDatabase();`. I found these calls frequently together to create a short-hand. 

```c#
var createSettings = new CreateDatabaseSettings()
                        .WithPrimaryFile(@"C:\MyPath\MyCakeTest.mdf")
                        .WithLogFile(@"C:\MyPath\MyCakeTest.ldf");
DropAndCreateDatabase(masterConnectionString, "MyCakeTest", createSettings)
```
This will drop and re-create the database with provided locations for data and log files.

### Execute Sql Command
```c#
ExecuteSqlCommand(String connectionString, string sqlCommands);
ExecuteSqlCommand(SqlConnection connection, string sqlCommands);
```

Does what it says on the tin: executes the sql query. But this method accommodates for `Go` within scripts. Usually executing long queries from .Net won't work when query has `GO` inside. This one does know what to do with it.

### Execute Command from SQL File
```c#
ExecuteSqlFile(String connectionString, string sqlFile);  
ExecuteSqlFile(SqlConnection connection, string sqlFile);
```
Reads sql file and executes commands from it. Executes parts of scripts separated by `GO` as a separate command executions. 

### Open Connection for Use in Multiple Operations
```c#
OpenSqlConnection(String connectionString)
```

Allows you to use the same connection for multiple SQL commands. Close the connection (by disposing) once you're finished with it.  
Example:

```c#
using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase"))
{
    ExecuteSqlCommand(connection, "...");
    ExecuteSqlFile(connection, "./somePath/MyScript.sql");
}
```

### Set Default Execution Timeout
```c#
SetSqlCommandTimeout(int commandTimeout)
```

Allows you to specify the command timeout in seconds for all commands. This is used to set the `CommandTimeout` property on the underlying `SqlCommand`.

```c#
SetSqlCommandTimeout(60);
using (var connection = OpenSqlConnection(@"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=MyDatabase"))
{
    ExecuteSqlCommand(connection, "..."); // <- execute long-running command
}
```

# Working With Backup files

### Restore Database Backup File
```c#
RestoreSqlBackup(String connectionString, FilePath backupFile, RestoreSqlBackupSettings settings)
RestoreSqlBackup(String connectionString, FilePath backupFile)
```

Restores the database from a `.bak` file. Options include to rename the target database, specify path where the data/log files are stored and ability to replace existing database. 

If new database name is not provided, db-name is extracted from the backup file; if new storage location for data files is not provided, system default folder is used.

Example:

```c#
Task("Restore-Database")
	.Does(() => {
		var connString = @"data source=(localdb)\MSSqlLocalDb";

		var backupFilePath = new FilePath(@".\src\Tests\multiFileBackup.bak");
		backupFilePath = backupFilePath.MakeAbsolute(Context.Environment);

		RestoreSqlBackup(connString, backupFilePath, new RestoreSqlBackupSettings() 
			{
				NewDatabaseName = "RestoredFromTest.Cake",
				NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()), // place files in special location
			}); 
	});
```

### Backup Database
```c#
BackupDatabase(string connectionString, string databaseName, BackupDatabaseSettings settings)
```

Backup a database to a `.bak` file. Options all you to compress the backup file and specify the path (or the specific filename). 

Example: 

```c#
Task("Backup-Database")
    .Does(() =>
    {
        var connString = @"data source=(localdb)\MSSqlLocalDb";
        var databaseName = "MyDatabase";
        BackupDatabase(connString, databaseName, new BackupDatabaseSettings() 
           {
                 Compress = false,
				 // you can specify either a folder or a file
                 Path = System.IO.Path.GetTempPath()
           }); 
    });
```

# Working with BACPAC and DACPAC 

This addin includes a thin wrapper around `Microsoft.SqlServer.DacFx` to provide ability to work with BACPAC and DACPAC files

### Working with BACPAC files

To create a bacpac file from a database call:

```c#
Task("Create-Bacpac")
	.Does(() =>{
		var connString = @"data source=(localdb)\MSSqlLocalDb";

		var dbName = "ForBacpac";

        var resultingFile = new FilePath(@".\ForBacpac.bacpac");

		CreateBacpacFile(connString, dbName, resultingFile);
	});
```

To restore from bacpac file into a database use this:

```c#
Task("Restore-From-Bacpac")
	.Does(() =>{
		var connString = @"data source=(localdb)\MSSqlLocalDb";

		var dbName = "FromBacpac";

		var file = new FilePath(@".\path\to\my.bacpac");

		RestoreBacpac(connString, dbName, file);
	})
```

### Working with DACPAC files

To extract a dacpac file from a database call:

```c#
Task("Extract-Dacpac")
	.Does(() =>{
		var connString = @"data source=(localdb)\MSSqlLocalDb";
     
		var dbName = "ForDacpac";
     
		CreateDatabase(connString, dbName);
 
		var settings = new ExtractDacpacSettings("MyAppName", "2.0.0.0") { 
			OutputFile = new FilePath(@".\Nsaga.dacpac")
		};
     
		ExtractDacpacFile(connString, dbName, settings);
	});
});
```

To publish from dacpac file into a database use this:

```c#
Task("Create-Bacpac")
	.Does(() =>{
		var connString = @"data source=(localdb)\MSSqlLocalDb";

		var dbName = "ForDacpac";

		var file = new FilePath(@".\src\Tests\Nsaga.dacpac");

		var settings = new PublishDacpacSettings { 
			GenerateDeploymentScript = true
		};

		PublishDacpacFile(connString, dbName, file, settings);
	});
});       
```

# Working with LocalDB 
Samples show here are using `LocalDb\v12.0`. This used to be default name for LocalDB instance when installed with SQL Server 2012. Since Sql Server 2014 the default name for LocalDB instance is `MSSQLLocalDB`, making the default instance name for LocalDB looking like this: `(LocalDB)\MSSQLLocalDB`. So before using `v12.0` double check what instance you have installed and go from there. 

This package includes a wrapper for working with LocalDB. LocalDB is a lightweight SQL Server version that is great for running tests against. 

Also please don't be alarmed that all the examples are using LocalDB. The plugin is capable of working with any SQL Server installation. This package includes commands to `Create`, `Start`, `Stop` and `Delete` instances of LocalDB. To be used like this:

```c#
#addin "nuget:?package=Cake.SqlServer"

Task("Create-LocalDB")
     .Does(() =>
     {
		// creates and starts instance
		// you don't need to start the instance separately
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
```


# Usage

You can also check our [integration tests](https://github.com/AMVSoftware/Cake.SqlServer/blob/master/tests.cake) for live examples.


### Gotchas

Remember to always add `@` before your connection strings. Some connection strings (i.e. `(localdb)\MSSqlLocalDb`) can contain backward slash `\` and that is an escape symbol in C#. So you need to always add `@` before the string:

	var connString = @"(localdb)\MSSqlLocalDb";

`\v` is the C# escape sequence for vertical tab which is not what you want. Using the verbatim string syntax `@` prevents escape sequences from being interpreted and you get what you expect, verbatim `\` and `v` characters.

### Generating Connection String

If you have complex connection strings, please consider using [SqlConnectionStringBuilder](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnectionstringbuilder)
for creating your connection strings:

```c#
 var connectionString = new SqlConnectionStringBuilder 
 { 
    DataSource = @"(LocalDb)\MSSQLLocalDB", 
    InitialCatalog = databaseName,
 }.ToString();
```
This class adds a lot of sugar around creating a connection string. 

### Creating Temp Database and Clean Up

This script is courtesy of [Joseph Musser](https://github.com/jnm2)

If you need to create and delete the database inside of your build script you can use this nice trick.

Somewhere your cake script (usually I put it in `lib.cake` and reference it from the main script via `#load "./lib.cake"`) put this class:

```c#
public static class On
{
    public static IDisposable Dispose(Action action)
    {
        return new OnDisposeAction(action);
    }


    private sealed class OnDisposeAction : IDisposable
    {
        private Action action;

        public OnDisposeAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            var exchange = System.Threading.Interlocked.Exchange(ref action, null);
            if (exchange != null)
            {
                exchange.Invoke();
            }
        }
    }
}
```

Then in your actual cake script you can do:
```c#
IDisposable TempDatabase(string connectionString, string databaseName)
{
    CreateDatabaseIfNotExists(connectionString, databaseName);
    return On.Dispose(() => DropDatabase(connectionString, databaseName));
}
```
and

```c#
var masterConnectionString = @"data source=(LocalDb)\MSSQLLocalDB;";
var databaseName = "Tests";
using (TempDatabase(masterConnectionString, databaseName))
{
    var connectionString = @"data source=(LocalDb)\MSSQLLocalDB;Database=Tests";

    ExecuteSqlCommand(connectionString, "select * from products.......");
    // execute your SQL operations
    // or run tests
}
```
This technique will make sure your temp database will be dropped after the payload/integration tests are executed.

# Reason to Develop
There is already a project that does similar things: [Cake.SqlTools](https://github.com/SharpeRAD/Cake.SqlTools). I have tried it and it was not enough for my purposes. I did look into extending functionality, but the way the project is structured - it won't let me do what I would like to do. The great idea in that project - be able to switch between MySql and SqlServer with a change of a single parameter.

But I wanted to implement things like creating a database if it does not exist. And syntax for that in MySql and SqlServer is different. So if I wanted to extend that project I had to come up with the same functionality in MySql. But I don't use MySql, hell I don't even have it installed on my dev-machines any more.

# How To Contribute

Install [.Net Framwork 4.7.2 Developer Pack](https://www.microsoft.com/net/download/thank-you/net472-developer-pack)

Open a command prompt in the root folder, and run these commands (Only need to run this once for any project. It will allow powershell scripts to execute):

```
powershell Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
powershell Get-ExecutionPolicy -List
```

# Running Cake Scripts

```
# compile and run unit tests
powershell .\build.ps1 -target run-unit-tests

# publish NUGET package
powershell .\build.ps1
```
