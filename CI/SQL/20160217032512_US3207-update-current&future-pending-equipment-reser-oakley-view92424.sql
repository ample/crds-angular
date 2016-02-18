USE [MinistryPlatform]
GO

UPDATE [dbo].[dp_Page_Views]
SET [View_Title] = 'Pending Equipment Reservations - Oakley'
  ,[Page_ID] = 302
  ,[View_Clause] = 'Event_Equipment.Cancelled = 0 AND Event_ID_Table.Event_End_Date >= Getdate() AND Event_Equipment.[_Approved] IS NULL AND Event_ID_Table_Congregation_ID_Table.[Congregation_ID] = 1'
WHERE [Page_View_ID] = 92424;
GO