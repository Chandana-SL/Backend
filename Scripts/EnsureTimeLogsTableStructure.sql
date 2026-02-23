-- Script to ensure TimeLogs table has the correct structure
-- Run this in SQL Server Management Studio or Azure Data Studio

USE [TimeTrack];
GO

-- Check if TimeLogs table exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TimeLogs' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Creating TimeLogs table...';
    
    CREATE TABLE [dbo].[TimeLogs] (
        [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [Date] DATE NOT NULL,
        [StartTime] TIME NOT NULL,
        [EndTime] TIME NOT NULL,
        [BreakDuration] INT NOT NULL DEFAULT 0,
        [TotalHours] DECIMAL(5,2) NOT NULL,
        [Activity] NVARCHAR(MAX) NULL,
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [FK_TimeLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE NO ACTION
    );
    
    PRINT 'TimeLogs table created successfully.';
END
ELSE
BEGIN
    PRINT 'TimeLogs table already exists. Checking columns...';
    
    -- Check and add LogId if missing (as PRIMARY KEY)
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'LogId')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [LogId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY;
        PRINT '✓ Added LogId column';
    END
    ELSE
        PRINT '✓ LogId exists';
    
    -- Check and add UserId if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'UserId')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [UserId] UNIQUEIDENTIFIER NOT NULL;
        PRINT '✓ Added UserId column';
    END
    ELSE
        PRINT '✓ UserId exists';
    
    -- Check and add Date if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'Date')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [Date] DATE NOT NULL;
        PRINT '✓ Added Date column';
    END
    ELSE
        PRINT '✓ Date exists';
    
    -- Check and add StartTime if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'StartTime')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [StartTime] TIME NOT NULL;
        PRINT '✓ Added StartTime column';
    END
    ELSE
        PRINT '✓ StartTime exists';
    
    -- Check and add EndTime if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'EndTime')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [EndTime] TIME NOT NULL;
        PRINT '✓ Added EndTime column';
    END
    ELSE
        PRINT '✓ EndTime exists';
    
    -- Check BreakDuration and ensure it's INT
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'BreakDuration')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [BreakDuration] INT NOT NULL DEFAULT 0;
        PRINT '✓ Added BreakDuration column (INT)';
    END
    ELSE
    BEGIN
        -- Check if it's the wrong type (TIME instead of INT)
        DECLARE @breakType NVARCHAR(128);
        SELECT @breakType = t.name
        FROM sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.TimeLogs') AND c.name = 'BreakDuration';
        
        IF @breakType = 'time'
        BEGIN
            PRINT '⚠ Converting BreakDuration from TIME to INT...';
            ALTER TABLE dbo.TimeLogs ADD BreakDurationMinutes INT NOT NULL DEFAULT 0;
            UPDATE dbo.TimeLogs SET BreakDurationMinutes = (DATEPART(HOUR, BreakDuration) * 60) + DATEPART(MINUTE, BreakDuration);
            ALTER TABLE dbo.TimeLogs DROP COLUMN BreakDuration;
            EXEC sp_rename 'dbo.TimeLogs.BreakDurationMinutes', 'BreakDuration', 'COLUMN';
            PRINT '✓ BreakDuration converted to INT';
        END
        ELSE
            PRINT '✓ BreakDuration exists (INT)';
    END
    
    -- Check and add TotalHours if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'TotalHours')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [TotalHours] DECIMAL(5,2) NOT NULL DEFAULT 0;
        PRINT '✓ Added TotalHours column';
    END
    ELSE
        PRINT '✓ TotalHours exists';
    
    -- Check and add Activity if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'Activity')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [Activity] NVARCHAR(MAX) NULL;
        PRINT '✓ Added Activity column';
    END
    ELSE
        PRINT '✓ Activity exists';
    
    -- Check and add Notes if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'Notes')
    BEGIN
        ALTER TABLE dbo.TimeLogs ADD [Notes] NVARCHAR(500) NULL;
        PRINT '✓ Added Notes column';
    END
    ELSE
        PRINT '✓ Notes exists';
END
GO

-- Display final table structure
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable,
    c.is_identity AS IsIdentity
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.TimeLogs')
ORDER BY c.column_id;

PRINT '';
PRINT '✅ TimeLogs table structure verification complete!';
GO
