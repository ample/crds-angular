USE [MinistryPlatform]

IF NOT EXISTS (SELECT * FROM [dbo].[Opportunities] WHERE Opportunity_ID = 115)
BEGIN
SET IDENTITY_INSERT [dbo].[Opportunities] ON
INSERT INTO [dbo].[Opportunities]
           ([Opportunity_ID]
		   ,[Opportunity_Title]
           ,[Description]
           ,[Group_Role_ID]
           ,[Program_ID]
           ,[Visibility_Level_ID]
           ,[Contact_Person]
           ,[Publish_Date]
           ,[Add_to_Group]
           ,[Minimum_Needed]
           ,[Domain_ID]
           ,[Shift_Start]
           ,[Shift_End])
     VALUES
           (115
		   ,'Kids'' Club'
           ,'A place for kids of young ages to learn about God'
           ,16
           ,109
           ,4
           ,2
           ,'2015-01-28'
           ,27705
           ,1
           ,1
           ,'07:00'
           ,'19:00')
	
SET IDENTITY_INSERT [dbo].[Opportunities] OFF
END