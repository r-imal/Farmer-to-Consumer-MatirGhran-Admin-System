USE [FarmerToConsumerDb];
GO

IF COL_LENGTH('dbo.Users', 'EmergencyContactPhone') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Users] DROP COLUMN [EmergencyContactPhone];
END
GO

IF COL_LENGTH('dbo.Users', 'NID') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Users] DROP COLUMN [NID];
END
GO

IF COL_LENGTH('dbo.Users', 'Gender') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Users] DROP COLUMN [Gender];
END
GO

IF COL_LENGTH('dbo.Users', 'Upazila') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Users] DROP COLUMN [Upazila];
END
GO
