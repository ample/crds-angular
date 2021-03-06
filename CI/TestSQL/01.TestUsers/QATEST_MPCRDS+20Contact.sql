--Registered Account / Guest Give Account - This is an account that started as a guest giving account and then registered as a new user.
USE [MinistryPlatform]
GO

DECLARE @processorID as varchar(255);

IF (SELECT URL from DP_Bookmarks where name = 'crossroads.net') like '%demo%'
	SET @processorID = 'cus_6YjXxCzFsV300g';
ELSE
	SET @processorID = 'cus_8Lez4HV887Qnwo';
	
--Guest Giving ACCOUNT
--Contact Record
INSERT INTO [dbo].Contacts 
(Company,Company_Name,Display_Name ,Prefix_ID,First_Name,Middle_Name,Last_Name,Suffix_ID,Nickname     ,Date_of_Birth,Gender_ID,Marital_Status_ID,Contact_Status_ID,Household_ID,Household_Position_ID,Participant_Record,Donor_Record,Email_Address        ,Email_Unlisted,Bulk_Email_Opt_Out,Bulk_SMS_Opt_Out,Mobile_Phone,Mobile_Carrier,Mobile_Phone_Unlisted,Company_Phone,Pager_Phone,Fax_Phone,User_Account,Web_Page,Remove_From_Directory,Industry_ID,Occupation_ID,Employer_Name,[SSN/EIN],Anniversary_Date,HS_Graduation_Year,Current_School,Contact_GUID,ID_Card,Domain_ID,__ShelbyID,__ExternalHouseholdID,__ExternalPersonID,__ExternalUserID,__ExternalBusinessID,Maiden_Name,__LastLegacyLogin,__LegacyUserName,__LegacyUserID,__LegacyEmailAddress) VALUES
(0      ,null        ,'Guest Giver',null     ,null      ,null       ,null     ,null     ,'Guest Giver',null         ,null     ,null             ,1                ,null        ,1                    ,null              ,null        ,'mpcrds+20@gmail.com',null          ,0                 ,0               ,null        ,null          ,null                 ,null         ,null       ,null     ,null        ,null    ,null                 ,null       ,null         ,null         ,null     ,null            ,null              ,null          ,NEWID()     ,null   ,1        ,null      ,null                 ,null              ,null            ,null                ,null       ,null             ,null            ,null          ,null                );

DECLARE @contactID as int
SET @contactID = (select contact_id from contacts where email_address = 'mpcrds+20@gmail.com' and last_name is null);

--Donor RECORD
INSERT INTO [dbo].Donors 
(Contact_ID,Statement_Frequency_ID,Statement_Type_ID,Statement_Method_ID,Setup_Date                ,Envelope_No,Cancel_Envelopes,Notes,First_Contact_Made,Domain_ID,__ExternalPersonID,_First_Donation_Date,_Last_Donation_Date,Processor_ID) VALUES
(@contactID,3                     ,1                ,4                  ,{ts '2015-07-06 12:03:37'},null       ,0               ,null ,null              ,1        ,null              ,null                ,null               ,@processorID);

--Update Contact Record
UPDATE [dbo].Contacts set Donor_Record = (select donor_id from donors where contact_id = @contactID) where contact_id = @contactID;
GO

--Registered Account - Mpcrds+20@gmail.com contact record
DECLARE @processorID as varchar(255);

IF (SELECT URL from DP_Bookmarks where name = 'crossroads.net') like '%demo%'
 SET @processorID = 'cus_6Woe7iX2PlkGeb';
ELSE
 SET @processorID = 'cus_8Lf397wd4AxozZ';


DECLARE @contactID as int
set @contactID = (select contact_id from contacts where Email_Address = 'mpcrds+20@gmail.com' and Last_Name = 'Kendricks');

INSERT INTO [dbo].Donors 
(Contact_ID,Statement_Frequency_ID,Statement_Type_ID,Statement_Method_ID,Setup_Date                ,Envelope_No,Cancel_Envelopes,Notes,First_Contact_Made,Domain_ID,__ExternalPersonID,_First_Donation_Date,_Last_Donation_Date,Processor_ID) VALUES
(@contactID,1                     ,1                ,2                  ,{ts '2015-07-01 09:13:17'},null       ,0               ,null ,null              ,1        ,null              ,null                ,null               ,@processorID);

DECLARE @donor_id as int
set @donor_id = (Select donor_ID from donors where contact_id = @contactID);

--Update Contact Record
update [dbo].Contacts set Donor_Record = @donor_id where contact_id = @contactID;
GO
