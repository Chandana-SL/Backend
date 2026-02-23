using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimeLogsStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop columns that are no longer needed (if they exist)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'CreatedAt')
                    ALTER TABLE [TimeLogs] DROP COLUMN [CreatedAt];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'IsApproved')
                    ALTER TABLE [TimeLogs] DROP COLUMN [IsApproved];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'UpdatedAt')
                    ALTER TABLE [TimeLogs] DROP COLUMN [UpdatedAt];
            ");

            // Alter TotalHours precision (safe operation)
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "TimeLogs",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            // Alter Date to date type (safe operation)
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "TimeLogs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            // Handle BreakDuration conversion from TIME to INT - Step 1: Add temp column if needed
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 
                    FROM sys.columns c
                    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID('dbo.TimeLogs') 
                    AND c.name = 'BreakDuration' 
                    AND t.name = 'time'
                )
                BEGIN
                    ALTER TABLE [TimeLogs] ADD [BreakDurationMinutes] INT NOT NULL DEFAULT 0;
                END
            ");

            // Step 2: Convert data
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'BreakDurationMinutes')
                BEGIN
                    UPDATE [TimeLogs] 
                    SET [BreakDurationMinutes] = (DATEPART(HOUR, [BreakDuration]) * 60) + DATEPART(MINUTE, [BreakDuration]);
                END
            ");

            // Step 3: Drop old column
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'BreakDurationMinutes')
                BEGIN
                    ALTER TABLE [TimeLogs] DROP COLUMN [BreakDuration];
                END
            ");

            // Step 4: Rename new column
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'BreakDurationMinutes')
                BEGIN
                    EXEC sp_rename 'dbo.TimeLogs.BreakDurationMinutes', 'BreakDuration', 'COLUMN';
                END
            ");

            // Add Activity column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TimeLogs') AND name = 'Activity')
                    ALTER TABLE [TimeLogs] ADD [Activity] NVARCHAR(MAX) NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "TimeLogs");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "TimeLogs",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "TimeLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "BreakDuration",
                table: "TimeLogs",
                type: "time",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TimeLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "TimeLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TimeLogs",
                type: "datetime2",
                nullable: true);
        }
    }
}
