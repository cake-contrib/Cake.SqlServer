# Cake.SqlServer
Cake Build addin for working with SqlServer. 


# Gotchas
Remember to always add `@` before your connection strings. Some connection strings (i.e. `(localdb)\v12.0`) can contain backward slash `\` and that is an escape symbol in C#. So you need to always add `@` before the string:

	var connString = @"(localDb)\v12.0";

`\v` is the C# escape sequence for vertical tab which is not what you want. Using the verbatim string syntax `@` prevents escape sequences from being interpreted and you get what you expect, verbatim `\` and `v` characters.


#Functionality

## Drop Database
`DropDatabase(string connectionString, string databaseName)`

Basically executes `Drop Database databasename` with some fail-safes. Actually it sets the database into offline mode - to cut off all existing connections. Then sets the database back online and then drops it. 
Reason for this dance - you can't drop a database if there are existing connections to the database.  

## Create Database If Not Exist
`CreateDatabaseIfNotExists(string connectionString, string databaseName)`

Here we check if the database exists first, if it does not exists - create it. Does not do anything if database with this name already exists.

## Create Database
`CreateDatabase(string connectionString, string databaseName)`

Creates database. If database with this name exists - SqlException is thrown. 


## Drop and Create Database
`DropAndCreateDatabase(String connectionString, String databaseName)`

Simply a short-hand for `DropDatabase(); CreateDatabase();`. I found these calls frequently together to create a short-hand. 


## Execute Sql Command
`ExecuteSqlCommand(String connectionString, string sqlCommands)` 

Does what it says on the tin: executes the sql query. But this method accommodates for `Go` within scripts. Usually executing long queries from .Net won't work when query has `GO` inside. This one does know what to do with it.

## Execute Command from SQL File

`ExecuteSqlFile(String connectionString, string sqlFile)`

Reads sql file and executes commands from it. Executes parts of scripts separated by `GO` as a separate command executions. 

#Usage

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

#Working with LocalDB 

This package includes a wrapper for working with `SqlLocalDb.exe` - that is for LocalDB. This is lightweight SQL Server version that is great for running tests against. This package includes commands to create, start, stop and delete instances of LocalDB. To be used like this:

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

#Creating Temp Database and Clean Up

This script is curtsey of [Joseph Musser](https://github.com/jnm2)

If you need to create and delete the database inside of your build script you can use this nice trick.

Somewhere in `lib.cake` put this class:

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

Then in your actual cake script you can do:

	IDisposable TempDatabase(string connectionString, string databaseName)
	{
	    CreateDatabaseIfNotExists(connectionString, databaseName);
	    return On.Dispose(() => DropDatabase(connectionString, databaseName));
	}

and
 
	var databaseName = "IntegrationTests";
    var masterConnectionString = new SqlConnectionStringBuilder { DataSource = @"(LocalDb)\MSSQLLocalDB" }.ToString(); 

	using (TempDatabase(masterConnectionString, databaseName))
	{
	    var connectionString = new SqlConnectionStringBuilder { DataSource = @"(LocalDb)\MSSQLLocalDB", InitialCatalog = databaseName }.ToString();
	    ExecuteSqlCommand(connectionString, "select * from products.......");
		// execute your SQL operations
		// or run tests
	    // ...
	}

#Reason to Develop
There is already a project that does similar things: [Cake.SqlTools](https://github.com/SharpeRAD/Cake.SqlTools). I have tried it and it was not enough for my purposes. I did look into extending functionality, but the way the project is structured - it won't let me do what I would like to do. The great idea in that project - be able to switch between MySql and SqlServer with a change of a single parameter.

But I wanted to implement things like creating a database if it does not exist. And syntax for that in MySql and SqlServer is different. So if I wanted to extend that project I had to come up with the same functionality in MySql. But I don't use MySql, hell I don't even have it installed on my dev-machines any more.