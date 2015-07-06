USE [MinistryPlatform]
GO

INSERT INTO [dbo].[dp_Page_Views]
           ([View_Title]
           ,[Page_ID]
           ,[Description]        
           ,[View_Clause]
           ,[Order_By])
     VALUES
           ('West Side Current & Future Reserv.'
           ,384
           ,'Current & future room reservations for West Side only'
           ,'Event_ID_Table.Event_End_Date > GetDate() AND Room_ID_Table.Building_ID=6'
           ,'Event_ID_Table.Event_Start_Date')
           
GO
