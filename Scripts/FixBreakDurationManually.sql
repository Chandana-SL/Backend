-- Manual fix script to clean up partial migration state
-- Run this script in your SQL Server Management Studio or Azure Data Studio

USE [TimeTrack];
GO

-- Check if Activity column exists
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'Activity')
BEGIN
    PRINT 'Activity column already exists. No action needed.';
END
ELSE
BEGIN
    PRINT 'Activity column does not exist. Will be added by migration.';
END
GO

-- Check current BreakDuration type
DECLARE @dataType NVARCHAR(128);
SELECT @dataType = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.TimeLogs') AND c.name = 'BreakDuration';

PRINT 'Current BreakDuration type: ' + ISNULL(@dataType, 'Column does not exist');

-- If BreakDuration is still TIME type, we need to convert it manually
IF @dataType = 'time'
BEGIN
    PRINT 'Converting BreakDuration from TIME to INT...';
    
    -- Add temporary column
    ALTER TABLE dbo.TimeLogs ADD BreakDurationMinutes INT NOT NULL DEFAULT 0;
    
    -- Convert data
    UPDATE dbo.TimeLogs 
    SET BreakDurationMinutes = (DATEPART(HOUR, BreakDuration) * 60) + DATEPART(MINUTE, BreakDuration);
    
    -- Drop old column
    ALTER TABLE dbo.TimeLogs DROP COLUMN BreakDuration;
    
    -- Rename new column
    EXEC sp_rename 'dbo.TimeLogs.BreakDurationMinutes', 'BreakDuration', 'COLUMN';
    
    PRINT 'BreakDuration conversion completed.';
END
ELSE IF @dataType = 'int'
BEGIN
    PRINT 'BreakDuration is already INT. No conversion needed.';
END
GO
