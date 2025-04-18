-- Create Audit Table (if not exists)
IF OBJECT_ID('dbo.LoanInfo_LoanAmountAudit', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoanInfo_LoanAmountAudit (
        AuditID INT IDENTITY PRIMARY KEY,
        LoanID INT,
        LoanUID UNIQUEIDENTIFIER,
        OldLoanAmount DECIMAL(18,2),
        NewLoanAmount DECIMAL(18,2),
        ModifiedDate DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(100)
    );
END

-- Drop trigger if it exists
IF OBJECT_ID('dbo.trg_LoanInfo_Update', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_LoanInfo_Update;

-- Create update trigger to detect LoanAmount changes
CREATE TRIGGER trg_LoanInfo_Update
ON dbo.LoanInfo
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert records into audit table if LoanAmount is modified
    INSERT INTO dbo.LoanInfo_LoanAmountAudit (LoanID, LoanUID, OldLoanAmount, NewLoanAmount, ModifiedDate, ModifiedBy)
    SELECT d.LoanID, d.LoanUID, d.LoanAmount, i.LoanAmount, GETDATE(), SUSER_NAME()
    FROM deleted d
    INNER JOIN inserted i ON d.LoanID = i.LoanID
    WHERE d.LoanAmount <> i.LoanAmount;

    -- Optional: Send email notification (ensure Database Mail is configured)
    IF EXISTS (SELECT 1 FROM deleted d INNER JOIN inserted i ON d.LoanID = i.LoanID WHERE d.LoanAmount <> i.LoanAmount)
    BEGIN
        EXEC msdb.dbo.sp_send_dbmail
            @profile_name = 'YourMailProfile',
            @recipients = 'admin@example.com',
            @subject = 'Loan Amount Modification Alert',
            @body = 'A loan amount has been modified. Please review the LoanInfo_LoanAmountAudit table.';
    END
END;