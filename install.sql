create table dbo.Numbers(number int not null, name nvarchar(50) not null)

Go
insert into dbo.Numbers(number, name) values (1, 'One')
insert into dbo.Numbers(number, name) values (2, 'Two')
insert into dbo.Numbers(number, name) values (3, 'Three')
Go
Select * from Numbers

drop table Numbers