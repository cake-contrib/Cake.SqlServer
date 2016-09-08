# Cake.SqlServer
Cake Build addin for working with SqlServer. 


# Gotchas
Remember to always add `@` before your connection strings. Some connection strings (i.e. `(localdb)\v12.0`) can contain backward slash `\` and that is an escape symbol in C#. So you need to always add `@` before the string:

	var connString = @"(localDb)\v12.0";

If you are in VisualStudio, it'll tell you that the "escape sequence is not valid", but Cake won't tell you that - it'll just transform your string into something you don't expect.


#Functionality

## Drop Database
`DropDatabase(string connectionString, string databaseName)`

Basically executes `Drop Database databasename` with some fail-safes. Actually it sets the database into offline mode - to cut off all existing connections. Then sets the database back online and then drops it.
 
Reason for this dance - you can't drop a database if there are existing connections to the database.  


## Create Database
`CreateDatabaseIfNotExists(string connectionString, string databaseName)`

Here we check if the database exists first, if it does not exists - create it. Does not do anything if database with this name already exists.

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
			
			// first create database
			CreateDatabaseIfNotExists(masterConnectionString, dbName);
			
			// then drop the database
			DropDatabase(masterConnectionString, dbName);
			
			// and recreate the db again
			DropAndCreateDatabase(masterConnectionString, dbName);

			// and create some tables
			ExecuteSqlCommand(connectionString, "create table dbo.Products(id int null)");
			
			// and execute sql from a file 
			ExecuteSqlCommand(connectionString, "./src/install.sql");
	    });


#Reason to Develop
There is already a project that does similar things: [Cake.SqlTools](https://github.com/SharpeRAD/Cake.SqlTools). I have tried it and it was not enough for my purposes. I did look into extending functionality, but the way the project is structured - it won't let me do what I would like to do. The great idea in that project - be able to switch between MySql and SqlServer with a change of a single parameter.

But I wanted to implement things like creating a database if it does not exist. And syntax for that in MySql and SqlServer is different. So if I wanted to extend that project I had to come up with the same functionality in MySql. But I don't use MySql, hell I don't even have it installed on my dev-machines any more.