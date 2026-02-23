-- Migration script to update TimeLogs table structure
-- Run this script when the application is stopped

USE [TimeTrack];
GO

-- Step 1: Drop existing foreign key constraint if it exists
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TimeLogs_Users_UserId')
BEGIN
    ALTER TABLE [dbo].[TimeLogs] DROP CONSTRAINT [FK_TimeLogs_Users_UserId];
END
GO

-- Step 2: Create a backup table with new structure
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TimeLogs_New')
BEGIN
    CREATE TABLE [dbo].[TimeLogs_New] (
        [LogId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [Date] DATE NOT NULL,
        [StartTime] TIME NOT NULL,
        [EndTime] TIME NOT NULL,
        [BreakDuration] INT NOT NULL DEFAULT 0,
        [TotalHours] DECIMAL(18,2) NOT NULL,
        [Activity] NVARCHAR(MAX) NULL,
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [PK_TimeLogs] PRIMARY KEY ([LogId])
    );
END
GO

-- Step 3: Migrate data from old table to new table (if old table exists and has data)
-- Note: This assumes you have a Users table with GUID-based UserId
-- You may need to adjust the migration logic based on your actual data

-- Option A: If your Users table already uses GUIDs, migrate the data
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TimeLogs')
AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TimeLogs') AND name = 'UserId' AND system_type_id = 56) -- 56 is INT type
BEGIN
    PRINT 'Old TimeLogs table uses INT UserId. Manual data migration required.';
    PRINT 'Please create a mapping between old INT UserIds and new GUID UserIds.';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TimeLogs')
BEGIN
    -- Migrate data if structure matches
    INSERT INTO [dbo].[TimeLogs_New] ([LogId], [UserId], [Date], [StartTime], [EndTime], [BreakDuration], [TotalHours], [Activity], [Notes])
    SELECT 
        NEWID() as [LogId],
        [UserId],
        CAST([Date] AS DATE) as [Date],
        [StartTime],
        [EndTime],
        CASE 
            WHEN DATEPART(HOUR, [BreakDuration]) * 60 + DATEPART(MINUTE, [BreakDuration]) > 0 
            THEN DATEPART(HOUR, [BreakDuration]) * 60 + DATEPART(MINUTE, [BreakDuration])
            ELSE 0
        END as [BreakDuration],
        [TotalHours],
        NULL as [Activity],
        [Notes]
    FROM [dbo].[TimeLogs]
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[TimeLogs_New]);
END
GO

-- Step 4: Drop old TimeLogs table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TimeLogs')
BEGIN
    DROP TABLE [dbo].[TimeLogs];
END
GO

-- Step 5: Rename new table to TimeLogs
EXEC sp_rename 'dbo.TimeLogs_New', 'TimeLogs';
GO

-- Step 6: Add foreign key constraint
ALTER TABLE [dbo].[TimeLogs]
ADD CONSTRAINT [FK_TimeLogs_Users_UserId] 
FOREIGN KEY ([UserId]) 
REFERENCES [dbo].[Users] ([UserId]) 
ON DELETE CASCADE;
GO

-- Step 7: Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TimeLogs_UserId' AND object_id = OBJECT_ID('TimeLogs'))
BEGIN
    CREATE INDEX [IX_TimeLogs_UserId] ON [dbo].[TimeLogs] ([UserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TimeLogs_Date' AND object_id = OBJECT_ID('TimeLogs'))
BEGIN
    CREATE INDEX [IX_TimeLogs_Date] ON [dbo].[TimeLogs] ([Date]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TimeLogs_UserId_Date' AND object_id = OBJECT_ID('TimeLogs'))
BEGIN
    CREATE INDEX [IX_TimeLogs_UserId_Date] ON [dbo].[TimeLogs] ([UserId], [Date]);
END
GO

PRINT 'TimeLogs table structure updated successfully!';
GO
