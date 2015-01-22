﻿using crds_angular.Models;
using crds_angular.Models.Crossroads;
using MinistryPlatform.Translation;
using MinistryPlatform.Translation.Services;
using MinistryPlatform.Translation.PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MinistryPlatform.Translation.Models;
using crds_angular.Models.MP;
using System.Configuration;

namespace crds_angular.Services
{
    public class AccountService : MinistryPlatformBaseService
    {
        public bool ChangePassword(string token, string newPassword)
        {
            return AuthenticationService.ChangePassword(token, newPassword);
        }

        //TODO: Put this logic in the Translation Layer?
        public bool SaveCommunicationPrefs(string token, AccountInfo accountInfo)
        {
            var contactId = MinistryPlatform.Translation.AuthenticationService.GetContactId(token);
            var contact = MinistryPlatform.Translation.Services.GetPageRecordService.GetRecordDict(Convert.ToInt32(ConfigurationManager.AppSettings["MyContact"]), contactId, token);
            try
            {                
                var emailsmsDict = getDictionary(new EmailSMSOptOut
                    {
                        Contact_ID = contactId,
                        Bulk_Email_Opt_Out = accountInfo.EmailNotifications,
                        Bulk_SMS_Opt_Out = accountInfo.TextNotifications
                    });
                var mailDict = getDictionary(new MailOptOut 
                {
                    Household_ID = (int)contact["Household_ID"],
                    Bulk_Mail_Opt_Out = accountInfo.PaperlessStatements 
                });
                CommunicationService.SetEmailSMSPreferences(token, emailsmsDict);
                CommunicationService.SetMailPreferences(token, mailDict);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AccountInfo getAccountInfo(string token)
        {
            var contactId = MinistryPlatform.Translation.AuthenticationService.GetContactId(token);
            CommunicationPreferences contact = CommunicationService.GetPreferences(token, contactId);
            var accountInfo = new AccountInfo
            {
                EmailNotifications = contact.Bulk_Email_Opt_Out,
                TextNotifications = contact.Bulk_SMS_Opt_Out,
                PaperlessStatements = contact.Bulk_Mail_Opt_Out
            };
            return accountInfo;


       }

        private static int CreateHouseholdRecord(User newUserData, string token){
            int recordId;
            int householdsPageID = Convert.ToInt32(ConfigurationManager.AppSettings["Households"]);

            Dictionary<string, object> householdDictionary = new Dictionary<string, object>();
            householdDictionary["Household_Name"] = newUserData.lastName;
            householdDictionary["Congregation_ID"] = 5; // Not Site Specific (default value at registration)
            householdDictionary["Household_Source_ID"] = 30; // Unknown (default value at registration)

            recordId = MinistryPlatform.Translation.Services.CreatePageRecordService.CreateRecord(householdsPageID, householdDictionary, token);

            return recordId;
        }

        private static int CreateContactRecord(User newUserData, string token, int householdRecordID)
        {
            int recordId;
            int contactsPageID = Convert.ToInt32(ConfigurationManager.AppSettings["Contacts"]);

            Dictionary<string, object> contactDictionary = new Dictionary<string, object>();
            contactDictionary["First_Name"] = newUserData.firstName;
            contactDictionary["Last_Name"] = newUserData.lastName;
            contactDictionary["Email_Address"] = newUserData.email;
            contactDictionary["Company"] = false; // default
            contactDictionary["Display_Name"] = newUserData.lastName + ", " + newUserData.firstName;
            contactDictionary["Nickname"] = newUserData.firstName;
            contactDictionary["Household_Position_ID"] = 1; // Head of Household (default value at registration)
            contactDictionary["Household_ID"] = householdRecordID;

            recordId = MinistryPlatform.Translation.Services.CreatePageRecordService.CreateRecord(contactsPageID, contactDictionary, token);

            return recordId;
        }

        private static int CreateContactHouseholdRecord(string token, int householdRecordID, int contactRecordID)
        {
            int recordId;
            int contactHouseholdsPageID = Convert.ToInt32(ConfigurationManager.AppSettings["ContactHouseholds"]);

            Dictionary<string, object> contactHouseholdDictionary = new Dictionary<string, object>();
            contactHouseholdDictionary["Contact_ID"] = contactRecordID;
            contactHouseholdDictionary["Household_ID"] = householdRecordID;
            contactHouseholdDictionary["Household_Position_ID"] = 1; // Head of Household (default value at registration)
            contactHouseholdDictionary["Household_Type_ID"] = 1; // Primary (default value at registration)

            recordId = MinistryPlatform.Translation.Services.CreatePageRecordService.CreateRecord(contactHouseholdsPageID, contactHouseholdDictionary, token);

            return recordId;
        }

        private static int CreateUserRecord(User newUserData, string token, int contactRecordID)
        {
            int recordId;
            int usersPageID = Convert.ToInt32(ConfigurationManager.AppSettings["Users"]);

            Dictionary<string, object> userDictionary = new Dictionary<string, object>();
            userDictionary["First_Name"] = newUserData.firstName;
            userDictionary["Last_Name"] = newUserData.lastName;
            userDictionary["User_Email"] = newUserData.email;
            userDictionary["Password"] = newUserData.password;
            userDictionary["Company"] = false; // default
            userDictionary["Display_Name"] = userDictionary["First_Name"];
            userDictionary["Domain_ID"] = 1;
            userDictionary["User_Name"] = userDictionary["User_Email"];
            userDictionary["Contact_ID"] = contactRecordID;

            recordId = MinistryPlatform.Translation.Services.CreatePageRecordService.CreateRecord(usersPageID, userDictionary, token);

            return recordId;
        }

        private static int CreateUserRoleSubRecord(string token, int userRecordID)
        {
            int recordId;
            int usersRolesPageID = Convert.ToInt32(ConfigurationManager.AppSettings["Users_Roles"]);

            Dictionary<string, object> userRoleDictionary = new Dictionary<string, object>();
            userRoleDictionary["Role_ID"] = 39; // All Platform Users (Default value for all users)
            recordId = MinistryPlatform.Translation.Services.CreatePageRecordService.CreateSubRecord(usersRolesPageID,userRecordID, userRoleDictionary, token);

            return recordId;
        }

        private static int CreateParticipantRecord(string token, int contactRecordID)
        {
            int recordId;
            int participantsPageID = Convert.ToInt32(ConfigurationManager.AppSettings["Participants"]);

            Dictionary<string, object> participantDictionary = new Dictionary<string, object>();
            participantDictionary["Participant_Type_ID"] = 4; // Guest (default value at registration)
            participantDictionary["Participant_Start_Date"] = DateTime.Now;
            participantDictionary["Contact_Id"] = contactRecordID;

            recordId = MinistryPlatform.Translation.Services.CreatePageRecordService.CreateRecord(participantsPageID, participantDictionary, token);

            return recordId;
        }

        public static Dictionary<int, int>RegisterPerson(User newUserData)
        {
            //TODO Move hardcoded DB IDs for default values out of here
            string token = AuthenticationService.authenticate(ConfigurationManager.AppSettings["ApiUser"], ConfigurationManager.AppSettings["ApiPass"]);

            int householdRecordID = CreateHouseholdRecord(newUserData,token);
            int contactRecordID = CreateContactRecord(newUserData,token,householdRecordID);
            int contactHouseholdRecordID = CreateContactHouseholdRecord(token,householdRecordID,contactRecordID);
            int userRecordID = CreateUserRecord(newUserData, token, contactRecordID);
            int userRoleRecordID = CreateUserRoleSubRecord(token, userRecordID);
            int participantRecordID = CreateParticipantRecord(token, contactRecordID);
            
            Dictionary<int, int> returnValues = new Dictionary<int, int>();
            returnValues[Convert.ToInt32(ConfigurationManager.AppSettings["Contacts"])] = contactRecordID;
            returnValues[Convert.ToInt32(ConfigurationManager.AppSettings["Participants"])] = participantRecordID;
            returnValues[Convert.ToInt32(ConfigurationManager.AppSettings["Users"])] = userRecordID;
            returnValues[Convert.ToInt32(ConfigurationManager.AppSettings["Households"])] = householdRecordID;
            returnValues[Convert.ToInt32(ConfigurationManager.AppSettings["ContactHouseholds"])] = contactHouseholdRecordID;
            return returnValues;
        }

    }
}