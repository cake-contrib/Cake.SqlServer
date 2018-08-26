### New in 1.13.0
* Update to Cake v0.30.0
* Fix for incorrect path separators when restoring backup on Linux

### New in 1.11.0
* Update to Cake v0.28.0
* Adding LocalDb path that works with SqlServer2017

### New in 1.10.0
* Updated to Cake v0.27.1

### New in 1.9.0
* Update to Cake v0.26.0
* Adding DatabaseExists
* Adding file parameters for database creation
* Other minor fixes

### New in 1.8.4
* Added SqlCommandVariableValues to PublishDacpacSettings

### New in 1.8
* Option to disable switch to single user mode on backup restoring #31
* Fix for ignored timeout on Backup Restore #33

### New in 1.7.2
* Update of Cake.Core package to v0.20.0
* Change of NuGet icon
* Minor corrections to documentation

### New in 1.7.0
* Wroking with DACPAC files
* Restructuring documentation
* Updated to Cake 0.19.2

### New in 1.6.0
* Creating and restoring BACPAC files

### New in 1.5.0
* `RestoreSqlBackup` provides ability to restore database backups from `.bak` files

### New in 1.4.0
* `SetSqlCommandTimeout` provides ability to set default timeout for all SQL operations


### New in 1.3.0 
* `OpenSqlConnection` provides an open connection to the database - for re-using in multiple commands execution.
* Overrides for `ExecuteSqlCommand` and `ExecuteSqlFile` to take `SqlConnection` object instaed of connection string.

### New in 1.2.1
* Alias to CreateDatabase - fails if the database already exists.

### New in 1.1.1 (Released 2016/10/20)
* Functionality to work with LocalDB instances.

### New in 1.0.1 (Released 2016/09/08)
* Initial methods for `DropDatabase`, `CreateDatabaseIfNotExists`, `DropAndCreateDatabase`, `ExecuteSqlCommand`, `ExecuteSqlFile`.

