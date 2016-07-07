Use [MinistryPlatform]
GO

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO

If NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Online_Meetings' AND Object_ID = Object_ID(N'Groups'))
BEGIN
ALTER TABLE dbo.Groups ADD
	Online_Meetings bit NOT NULL DEFAULT (0);
End
GO
COMMIT