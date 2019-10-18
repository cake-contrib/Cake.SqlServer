use master
go

drop database CakeRestoreTest
create database CakeRestoreTest

BACKUP DATABASE [CakeRestoreTest] TO 
DISK = 'C:\multiFileBackup.bak'
WITH INIT --, NOUNLOAD , NAME = 'foo2', NOSKIP , STATS = 10, NOFORMAT


BACKUP DATABASE [CakeRestoreTest] TO 
DISK = 'C:\multiFilesBackup1.bak', 
DISK = 'C:\multiFilesBackup2.bak', 
DISK = 'C:\multiFilesBackup3.bak'
WITH INIT --, NOUNLOAD , NAME = 'foo2', NOSKIP , STATS = 10, NOFORMAT

use CakeRestoreTest
Go
create table test (msg nvarchar(max))

BACKUP DATABASE [CakeRestoreTest] TO 
DISK = 'C:\differentialMultiFilesBackup1.bak', 
DISK = 'C:\differentialMultiFilesBackup2.bak', 
DISK = 'C:\differentialMultiFilesBackup3.bak'
WITH  DIFFERENTIAL --, INIT , NOUNLOAD , NAME = 'foo2', NOSKIP , STATS = 10, NOFORMAT