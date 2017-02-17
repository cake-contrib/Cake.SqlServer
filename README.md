# Cake.SqlServer
Cake Build addin for working with SqlServer and LocalDb.


# Functionality

## Create Database
```c#
CreateDatabase(string connectionString, string databaseName)
```
Creates database. If database with this name exists - SqlException is thrown. 

## Create Database If Not Exist
```c#
CreateDatabaseIfNotExists(string connectionString, string databaseName)
```

Here we check if the database exists first, if it does not exists - create it. Does not do anything if database with this name already exists.


## Drop Database
```c#
DropDatabase(string connectionString, string databaseName)
```

Basically executes `Drop Database databasename` with some fail-safes. Actually it sets the database into offline mode - to cut off all existing connections. Then sets the database back online and then drops it. 
Reason for this dance - you can't drop a database if there are existing connections to the database.  

## Drop and Create Database
```c#
DropAndCreateDatabase(String connectionString, String databaseName)
```

Simply a short-hand for `DropDatabase(); CreateDatabase();`. I found these calls frequently together to create a short-hand. 


## Execute Sql Command
```c#
ExecuteSqlCommand(String connectionString, string sqlCommands);
ExecuteSqlCommand(SqlConnection connection, string sqlCommands);
```

Does what it says on the tin: executes the sql query. But this method accommodates for `Go` within scripts. Usually executing long queries from .Net won't work when query has `GO` inside. This one does know what to do with it.

## Execute Command from SQL File
```c#
ExecuteSqlFile(String connectionString, string sqlFile);  
ExecuteSqlFile(SqlConnection connection, string sqlFile);
```
Reads sql file and executes commands from it. Executes parts of scripts separated by `GO` as a separate command executions. 

## Open Connection for Use in Multiple Operations
```c#
OpenSqlConnection(String connectionString)
```

Allows you to use the same connection for multiple SQL commands. Close the connection (by disposing) once you're finished with it.  
Example:

```c#
using (var connection = OpenSqlConnection(@"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase"))
{
    ExecuteSqlCommand(connection, "...");
    ExecuteSqlFile(connection, "./somePath/MyScript.sql");
}
```

## Set Default Execution Timeout
```c#
SetSqlCommandTimeout(int commandTimeout)
```

Allows you to specify the command timeout in seconds for all commands. This is used to set the `CommandTimeout` property on the underlying `SqlCommand`.

```c#
SetSqlCommandTimeout(60);
using (var connection = OpenSqlConnection(@"Data Source=(LocalDb)\v12.0;Initial Catalog=MyDatabase"))
{
    ExecuteSqlCommand(connection, "..."); // <- execute long-running command
}
```

## Restore Database Backup File
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
		var connString = @"data source=(LocalDb)\v12.0";

		var backupFilePath = new FilePath(@".\src\Tests\multiFileBackup.bak");
		backupFilePath = backupFilePath.MakeAbsolute(Context.Environment);

		RestoreSqlBackup(connString, backupFilePath, new RestoreSqlBackupSettings() 
			{
				NewDatabaseName = "RestoredFromTest.Cake",
				NewStorageFolder = new DirectoryPath(System.IO.Path.GetTempPath()), // place files in special location
			}); 
	});
```


## Usage
Samples show here are using `LocalDb\v12.0`. This used to be default name for LocalDB instance when installed with SQL Server 2012. Since Sql Server 2014 the default name for LocalDB instance is `MSSQLLocalDB`, making the default instance name for LocalDB looking like this: `(LocalDB)\MSSQLLocalDB`. So before using `v12.0` double check what instance you have installed and go from there. 

Also please don't be alarmed that all the examples are using LocalDB. The plugin is capable of working with any SQL Server installation.

You can also check our [integration tests](https://github.com/AMVSoftware/Cake.SqlServer/blob/master/tests.cake) for live examples.

```c#
#addin "nuget:?package=Cake.SqlServer"

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
```

# Working with BACPAC files
This package also includes a wrapper to create bacpac files and restore them back into database. This is a thin wrapper around `Microsoft.SqlServer.DacFx` package. 

To create a bacpac file from a database call 

```c#
Task("Create-Bacpac")
	.Does(() =>{
		var connString = @"data source=(LocalDb)\v12.0";

		var dbName = "ForBacpac";

       var resultingFile = new FilePath(@".\ForBacpac.bacpac")

		CreateBacpacFile(connString, dbName, resultingFile);
	});
```

To restore from bacpac file into a database use this:

```c#
Task("Restore-From-Bacpac")
	.Does(() =>{
		var connString = @"data source=(LocalDb)\v12.0";

		var dbName = "FromBacpac";

		var file = new FilePath(@".\path\to\my.bacpac");

		RestoreBacpac(connString, dbName, file);
	})
```


# Working with LocalDB 
This package includes a wrapper for working with LocalDB. LocalDB is a lightweight SQL Server version that is great for running tests against. This package includes commands to `Create`, `Start`, `Stop` and `Delete` instances of LocalDB. To be used like this:

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

# Gotchas

Remember to always add `@` before your connection strings. Some connection strings (i.e. `(localdb)\v12.0`) can contain backward slash `\` and that is an escape symbol in C#. So you need to always add `@` before the string:

	var connString = @"(localDb)\v12.0";

`\v` is the C# escape sequence for vertical tab which is not what you want. Using the verbatim string syntax `@` prevents escape sequences from being interpreted and you get what you expect, verbatim `\` and `v` characters.

# Patterns of Use

## Generating Connection String

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

## Creating Temp Database and Clean Up

This script is curtesy of [Joseph Musser](https://github.com/jnm2)

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
