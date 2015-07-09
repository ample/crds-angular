USE [MinistryPlatform]
GO

INSERT INTO [dbo].[dp_Page_Views]
           ([View_Title]
           ,[Page_ID]
           ,[Description]        
           ,[View_Clause]
           ,[Order_By])
     VALUES
           ('Oakley Annex Current & Future Reserv.'
           ,384
           ,'Current & future room reservations for Oakley Annex only'
           ,'Event_ID_Table.Event_End_Date > GetDate() AND Room_ID_Table.Building_ID=16'
           ,'Event_ID_Table.Event_Start_Date')
           
GO
