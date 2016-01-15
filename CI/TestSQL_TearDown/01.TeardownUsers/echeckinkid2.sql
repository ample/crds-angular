USE [MinistryPlatform]
GO

--Get the required data to add to our contact. 
Declare @contactID as int
Set @contactID = (select top 1 contact_id from contacts where display_name = 'ECheckIn, Kid2');

Declare @houseHoldID as int
set @houseHoldID = (select houseHold_ID from contacts where contact_id = @contactID);

Declare @participantID as int
set @participantID = (select participant_record from contacts where contact_id = @contactID);

Declare @userAccount as int
set @userAccount = (select user_account from contacts where contact_id = @contactID);

Declare @donorID as int
set @donorID = (select donor_record from contacts where contact_id = @contactID);

--Update old contact record so we can delete it. 
UPDATE [dbo].Contacts
SET Household_ID = null, Participant_Record = null, User_Account = null, Donor_record = null
WHERE display_name = 'ECheckIn, Kid2';

--Delete the address if it exists.
IF  (select address_id from households where Household_ID = @houseHoldID) is not Null
BEGIN
DECLARE @addressID as int
set @addressID = (select address_ID from households where houseHold_ID = @houseHoldID);

update households set address_id = null where houseHold_ID = @houseHoldID;

Delete from addresses where address_id = @addressID;
END

--Just get rid of this so we can delete EcheckInHusband's old contact record
DELETE From [dbo].CONTACT_HOUSEHOLDS
WHERE CONTACT_ID = @contactID;

DECLARE @communicationID as int
set @communicationID = (Select Communication_ID from dp_Communications where TO_CONTACT = @contactID);

DELETE from [dbo].dp_commands 
WHERE communication_id = @communicationID;

DELETE from [dbo].dp_Contact_Publications 
WHERE contact_id = @contactID;

DELETE from [dbo].dp_communication_messages 
WHERE Communication_ID = @communicationID;

Delete from [dbo].dp_Communications
WHERE Communication_ID = @communicationID;

Delete from [dbo].Activity_Log
WHERE Contact_id = @contactID;

Delete from [dbo].Activity_Log
WHERE houseHold_ID = @houseHoldID;

--Delete the household
UPDATE [dbo].Contacts Set houseHold_ID = null
WHERE Household_ID = @houseHoldID;

DELETE from [dbo].CONTACT_HOUSEHOLDS 
WHERE houseHold_ID = @houseHoldID;

Delete from [dbo].Households where houseHold_ID = @houseHoldID;

--Lets start getting rid of the participant record
delete from Form_Response_Answers where form_response_id = (select form_response_id from form_responses where contact_id = @contactID);

delete from form_responses where contact_id = @contactID;

delete from response_attributes where response_id in (select response_id from responses where participant_id = @participantID);

delete from responses where participant_id = @participantID;

delete from event_participants where participant_id = @participantID;

delete from group_participants where participant_id = @participantID;

delete from participants where participant_id = @participantID;
--delete relationships
delete from [dbo].contact_relationships where contact_id = @contactID;

delete from [dbo].contact_relationships where related_contact_id = @contactID;
GO

--Delete EcheckInHusband's old contact record
DELETE FROM [dbo].Contacts where contact_id = (select top 1 contact_id from contacts where display_name = 'ECheckIn, Kid2');
GO