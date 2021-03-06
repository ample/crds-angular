USE [MinistryPlatform]
GO

/****** Object:  StoredProcedure [dbo].[report_CRDS_TeamSummaryGroupDetails]    Script Date: 12/10/2015 4:45:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[report_CRDS_TeamSummaryGroupDetails]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[report_CRDS_TeamSummaryGroupDetails] AS' 
END
GO

-- =============================================

ALTER PROCEDURE [dbo].[report_CRDS_TeamSummaryGroupDetails]
	-- Add the parameters for the stored procedure here
	@GroupID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT g.Group_Name, c.Nickname, c.Last_Name, g.Description  FROM Groups g
	JOIN Contacts c on g.Primary_Contact = c.Contact_ID
	WHERE g.Group_ID = @GroupID
END

GO


