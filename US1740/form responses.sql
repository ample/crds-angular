/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [Form_Response_Answer_ID]
      ,[Form_Field_ID]
      ,[Response]
      ,[Domain_ID]
      ,[Form_Response_ID]
      ,[Event_Participant_ID]
      ,[Pledge_ID]
      ,[Opportunity_Response]
      ,[Placed]
  FROM [MinistryPlatform].[dbo].[Form_Response_Answers] fra
  where fra.Form_Field_ID = 1439

  SELECT Response FROM [MinistryPlatform].[dbo].[Form_Response_Answers] fra where fra.Form_Field_ID = 1439