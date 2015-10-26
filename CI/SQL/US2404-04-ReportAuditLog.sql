USE [MinistryPlatform]
GO
/****** Object:  StoredProcedure [dbo].[report_cr_AuditLog]    Script Date: 10/22/2015 2:57:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.report_cr_AuditLog', 'p') IS NULL
    EXEC('CREATE PROCEDURE dbo.report_cr_AuditLog AS SELECT 1')
GO

ALTER PROCEDURE [dbo].[report_cr_AuditLog]
	@User_Name nvarchar(254),
	@Days_Before Int
AS
BEGIN

SELECT * 
FROM dp_Audit_Log al INNER JOIN dp_Audit_Detail ald ON al.Audit_Item_ID = ald.Audit_Item_ID 
WHERE al.User_Name=@User_Name AND (al.Date_Time >= DATEADD(day, @Days_Before, GETDATE()))

END