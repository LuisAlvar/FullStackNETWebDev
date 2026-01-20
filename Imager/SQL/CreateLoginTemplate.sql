/* ============================================================
   1. Create a SQL Server Login (server-level)
   ------------------------------------------------------------
   NOTE: Replace YourStrongPasswordHere with a secure password.
   ============================================================ */

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ImagerAppLogin')
BEGIN
	CREATE LOGIN ImagerAppLogin
	WITH PASSWORD = '[StrongPasswordHere]';
END
GO

/* ============================================================
   2. Create a User inside the PortfolioMain database
   ------------------------------------------------------------
   This maps the server login to a database user
   ============================================================ */
USE PortfolioMain
GO 

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ImagerAppUser')
BEGIN
	CREATE USER ImagerAppUser FOR LOGIN ImagerAppLogin;
END 
GO

/* ============================================================
   3. Grant permissions
   ------------------------------------------------------------
   Choose the appropriate permission level:
   - db_datareader: can read all tables
   - db_datawriter: can write to all tables
   - db_ddladmin: can create and drop tables
   - db_owner: full control (not recommended unless necessary)
   ============================================================ */
-- Basic read/write permissions (recommended)
ALTER ROLE db_datareader ADD MEMBER ImagerAppUser;
ALTER ROLE db_datawriter ADD MEMBER ImagerAppUser;
ALTER ROLE db_ddladmin ADD MEMBER ImagerAppUser;
GO 

/* if your app need full control (not recommended unless required)
ALTER ROLE db_owner ADD MEMBER ImagerAppUser;
GO
*/
