-- Check if the login exists at the server level
IF NOT EXISTS (SELECT * FROM master.sys.server_principals WHERE name = 'WorldCities')
BEGIN
    PRINT 'Server-level login does not exists.'
	CREATE LOGIN WorldCities
	WITH PASSWORD='MyVeryOwn$721'
END
ELSE
BEGIN
    PRINT 'Server-level login exists.'
END

-- Check if the user exists in a specific database
USE WorldCities;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'WorldCities')
BEGIN
    PRINT 'Database-level user does not exists.'
	CREATE USER WorldCities
		FOR LOGIN WorldCities
		WITH DEFAULT_SCHEMA=dbo
END
ELSE
BEGIN 
	PRINT 'Database-level user does exists.'
END

USE WorldCities;
-- Check if the user is a member of a specific role
DECLARE @UserName NVARCHAR(128) = 'WorldCities';
DECLARE @RoleName NVARCHAR(128) = 'db_owner';
IF NOT EXISTS (SELECT * 
           FROM sys.database_role_members drm
           INNER JOIN sys.database_principals dp1 ON drm.role_principal_id = dp1.principal_id
           INNER JOIN sys.database_principals dp2 ON drm.member_principal_id = dp2.principal_id
           WHERE dp1.name = @RoleName AND dp2.name = @UserName)
BEGIN
    PRINT @UserName + ' is not a member of the ' + @RoleName + ' role. Addin now role.'
	EXEC sp_addrolemember @RoleName, @Username
END
ELSE
BEGIN
    PRINT @UserName + ' is a member of the ' + @RoleName + ' role.'
END
