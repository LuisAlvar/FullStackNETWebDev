USE [LoanManager]
GO

-- Drop table if it exists
IF OBJECT_ID('dbo.LoanInfo', 'U') IS NOT NULL
    DROP TABLE dbo.LoanInfo;

-- Create table with LoanUID as an alternate key
CREATE TABLE dbo.LoanInfo (
    [LoanID]                INT PRIMARY KEY,
    [LoanUID]               UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL UNIQUE,
    [Name]                  NVARCHAR(50),
    [Type]                  NVARCHAR(50)
    [Status]                NVARCHAR(50)
    [Subsidy]               NVARCHAR(50)
    [OriginalAmount]        DECIMAL(18,2),
    [InterestRate]          DECIMAL(18,2),
    [LoanTerm]              INT,
    [LastDisbursementDate]  DATE,
    [Active]                BIT DEFAULT 1,
    [InsertId]              NVARCHAR(25)) DEFAULT SUSER_NAME(),
    [InsertDate]            DATETIME DEFAULT GETDATE(), 
    [UpdateId]              NVARCHAR(25) DEFAULT SUSER_NAME(),
    [UpdateDate]            DATETIME DEFAULT GETDATE()
);

-- Drop trigger if it exists
IF OBJECT_ID('dbo.trg_LoanInfo_Update', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_LoanInfo_Update;

-- Create update trigger
CREATE TRIGGER trg_LoanInfo_Update
ON dbo.LoanInfo
AFTER UPDATE
AS
BEGIN
    -- Example: Log updates into another table (adjust as needed)
    INSERT INTO dbo.LoanInfo_UpdateLog (LoanID, LoanUID, ModifiedDate)
    SELECT LoanID, LoanUID, GETDATE()
    FROM inserted;
END;