USE [MinistryPlatform]
GO
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE [dbo].[Donations] ADD
[Registered_Donor] bit NULL
GO
ALTER TABLE [dbo].[Donations] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
