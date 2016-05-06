--SELECT Attribute_Name, C.First_Name  
--     FROM Contact_Attributes CA
--      INNER JOIN Attributes A ON A.Attribute_ID = CA.Attribute_ID	
--	  INNER JOIN Contacts C ON C.Contact_ID = CA.Contact_ID	
--     WHERE A.Attribute_ID = CA.Attribute_ID
--	   AND C.Contact_ID = CA.Contact_ID	
--	   AND A.Attribute_Type_ID = 20		
		
		--,STUFF((Attribute_Name  
			 --FROM Contact_Attributes CA
			 -- INNER JOIN Attributes A ON A.Attribute_ID = CA.Attribute_ID	
			 -- INNER JOIN Contacts C ON C.Contact_ID = CA.Contact_ID	
			 --WHERE A.Attribute_ID = CA.Attribute_ID
			 --  AND C.Contact_ID = CA.Contact_ID	
			 --  AND A.Attribute_Type_ID = 20
			 --FOR XML PATH('''')), 1, 1, '''') AS [Ethnicity]	

USE [MinistryPlatform]	
GO

SET IDENTITY_INSERT [dbo].[dp_Page_Views] ON

INSERT INTO [dp_page_views]([Page_View_ID],[View_Title],[Page_ID],[Description],[Field_List],[View_Clause],[Order_By],[User_ID],[User_Group_ID])
VALUES(92146,'Undivided - Registered Facilitators',316,'Staff members can view the users that have registered to be an Undivided Facilitator.'
	,'Group_Participants.[Start_Date] AS [Registration Date] 
		,Participant_ID_Table_Contact_ID_Table.[First_Name]
		,Participant_ID_Table_Contact_ID_Table.[Last_Name]
		,Participant_ID_Table_Contact_ID_Table_Gender_ID_Table.[Gender]
		,DATEDIFF(hour,Participant_ID_Table_Contact_ID_Table.[Date_of_Birth],GETDATE())/8766 AS [AGE]
		,(SELECT Attribute_Name 
		 FROM Group_Participant_Attributes GPA, Attributes A 
		 WHERE GPA.Attribute_ID = A.Attribute_ID and 
		GPA.Group_Participant_ID = Group_Participants.Group_Participant_ID and
		Attribute_Type_ID = 84) AS [Preferred Undivided Session]
		,(SELECT Attribute_Name 
		 FROM Group_Participant_Attributes GPA, Attributes A 
		 WHERE GPA.Attribute_ID = A.Attribute_ID and 
		GPA.Group_Participant_ID = Group_Participants.Group_Participant_ID and
		Attribute_Type_ID = 85) AS [Facilitator Training]		
		,(SELECT Notes 
		 FROM Group_Participant_Attributes GPA, Attributes A 
		 WHERE GPA.Attribute_ID = A.Attribute_ID and 
		GPA.Group_Participant_ID = Group_Participants.Group_Participant_ID and
		Attribute_Type_ID = 87) AS [Preferred Co-Facilitator]	
		,Group_Participants.[Child_Care_Requested] AS [Child Care Requested]'
,'Group_ID_Table.Group_ID = 166572 AND (Group_Participants.End_Date > GetDate() OR Group_Participants.End_Date IS NULL)'
,'Group_Participants.[Start_Date]' 
,NULL
,NULL
)
SET IDENTITY_INSERT [dbo].[dp_Page_Views] OFF

--delete dp_page_views
--where Page_View_ID = 92146


			    