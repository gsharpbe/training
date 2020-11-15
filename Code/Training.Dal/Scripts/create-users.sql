-- master DB 

CREATE LOGIN App_Training WITH PASSWORD = 'r4D7fJNcxGTi6gR'
GO

CREATE LOGIN App_TrainingAdmin WITH PASSWORD = 'Yh5r7UKK47lnxsip'
GO

-- training db

CREATE USER App_Training FOR LOGIN App_Training
GO

EXEC sp_addrolemember db_datawriter, App_Training
GO

EXEC sp_addrolemember db_datareader, App_Training
GO


CREATE USER App_TrainingAdmin FOR LOGIN App_TrainingAdmin
GO

EXEC sp_addrolemember db_owner, App_TrainingAdmin
GO