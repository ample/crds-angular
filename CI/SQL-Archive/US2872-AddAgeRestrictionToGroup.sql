USE [MinistryPlatform]
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Online_RSVP_Minimum_Age' AND Object_ID = Object_ID(N'dbo.Groups'))
BEGIN
	ALTER TABLE [dbo].[Groups]
	ADD Online_RSVP_Minimum_Age int
END
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Groups', N'COLUMN', N'Online_RSVP_Minimum_Age'))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Participant Age Restriction for Online RSVP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Groups', @level2type=N'COLUMN',@level2name=N'Online_RSVP_Minimum_Age'
GO