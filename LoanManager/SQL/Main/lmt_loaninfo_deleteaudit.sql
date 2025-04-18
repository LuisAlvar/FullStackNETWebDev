-- Create Delete Audit Table (if not exists)
IF OBJECT_ID('dbo.LoanInfo_DeleteAudit', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoanInfo_DeleteAudit (
        AuditID INT IDENTITY PRIMARY KEY,
        LoanID INT,
        LoanUID UNIQUEIDENTIFIER,
        BorrowerName NVARCHAR(100),
        LoanAmount DECIMAL(18,2),
        LoanDate DATE,
        Status NVARCHAR(50),
        DeletedDate DATETIME DEFAULT GETDATE(),
        DeletedBy NVARCHAR(100)
    );
END

-- Drop trigger if it exists
IF OBJECT_ID('dbo.trg_LoanInfo_Delete', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_LoanInfo_Delete;

-- Create delete trigger to log deleted records
CREATE TRIGGER trg_LoanInfo_Delete
ON dbo.LoanInfo
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert deleted records into audit table
    INSERT INTO dbo.LoanInfo_DeleteAudit (LoanID, LoanUID, BorrowerName, LoanAmount, LoanDate, Status, DeletedDate, DeletedBy)
    SELECT d.LoanID, d.LoanUID, d.BorrowerName, d.LoanAmount, d.LoanDate, d.Status, GETDATE(), SUSER_NAME()
    FROM deleted d;

    -- Optional: Send email notification
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        EXEC msdb.dbo.sp_send_dbmail
            @profile_name = 'YourMailProfile',
            @recipients = 'admin@example.com',
            @subject = 'Loan Record Deletion Alert',
            @body = 'A loan record has been deleted. Please review the LoanInfo_DeleteAudit table.';
    END
END;