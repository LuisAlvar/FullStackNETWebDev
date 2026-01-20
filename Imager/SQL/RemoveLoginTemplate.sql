/* ============================================================
   1. Remove the database user from PortfolioMain
   ------------------------------------------------------------
   This must be done BEFORE dropping the server login
   ============================================================ */
USE PortfolioMain
GO

-- Remove role memberships if they exist
IF EXISTS(SELECT * FROM sys.database_principals WHERE name = 'ImagerAppUser')
BEGIN  
	ALTER ROLE db_datareader DROP MEMBER ImagerAppUser;
	ALTER ROLE db_datawriter DROP MEMBER ImagerAppUser;
   ALTER ROLE db_ddladmin DROP MEMBER ImagerAppUser;
	-- If you added db_owner earlier, uncomment:
	-- ALTER ROLE db_owner DROP MEMBER ImagerAppUser;
END
GO

-- Drop the database user
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ImagerAppUser')
BEGIN 
	DROP USER ImagerAppUser
END 
GO

/* ============================================================
   2. Drop the SQL Server login (server-level)
   ------------------------------------------------------------
   Only possible after the database user is removed.
   ============================================================ */
IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ImagerAppLogin')
BEGIN 
	DROP LOGIN ImagerAppLogin;
END
GO