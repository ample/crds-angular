USE [MinistryPlatform]
GO

INSERT INTO [dbo].[dp_Communications]
           ([Author_User_ID]
           ,[Subject]
           ,[Body]
           ,[Domain_ID]
           ,[Start_Date]
           ,[Expire_Date]
           ,[Communication_Status_ID]
           ,[From_Contact]
           ,[Reply_to_Contact]
           ,[_Sent_From_Task]
           ,[Selection_ID]
           ,[Template]
           ,[Active]
           ,[To_Contact])
     VALUES
           (5
           ,'Kids Club Application'
           ,'Please go to http://int.crossroads.net/volunteer-application/kids-club/[Contact_ID] to fill out the application. <br /><br />If you are filling out this application for a child between the ages of 10 and 13, please have the child with you as you fill out the application. <br /><br />Thanks'
           ,1
           ,GETDATE()
           ,NULL
           ,NULL
           ,7
           ,7
           ,NULL
           ,NULL
           ,1
           ,1
           ,NULL)
GO
