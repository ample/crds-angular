USE [MinistryPlatform]
GO

IF NOT EXISTS  (SELECT NULL FROM SYS.EXTENDED_PROPERTIES WHERE [major_id] = OBJECT_ID('cr_Organizations') AND [name] = N'MS_Description' AND [minor_id] = (SELECT [column_id] FROM SYS.COLUMNS WHERE [name] = 'Open_Signup' AND [object_id] = OBJECT_ID('cr_Organizations')))
BEGIN
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Allow other Organizations to participate in this Organizations Intitatives' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'cr_Organizations', @level2type=N'COLUMN',@level2name=N'Open_Signup'
END
ELSE
BEGIN
	EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Allow other Organizations to participate in this Organizations Intitatives' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'cr_Organizations', @level2type=N'COLUMN',@level2name=N'Open_Signup'
END
