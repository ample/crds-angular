﻿using crds_angular.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using MinistryPlatform.Translation.Services;

namespace crds_angular.Services
{
    public class PersonService : MinistryPlatformBaseService       
    {
        public void setProfile(String token, Person person)
        {
            var contactDictionary = getDictionary(person.GetContact());
            var householdDictionary = getDictionary(person.GetHousehold());
            var addressDictionary = getDictionary(person.GetAddress());
            addressDictionary.Add("State/Region", addressDictionary["State"]);
            
            MinistryPlatformService.UpdateRecord(Convert.ToInt32(ConfigurationManager.AppSettings["MyContact"]), contactDictionary, token);
            MinistryPlatformService.UpdateRecord(Convert.ToInt32(ConfigurationManager.AppSettings["MyHousehold"]), householdDictionary, token);
            MinistryPlatformService.UpdateRecord(Convert.ToInt32(ConfigurationManager.AppSettings["MyAddresses"]), addressDictionary, token); 
        }

        public List<Models.Crossroads.Skill> getLoggedInUserSkills(int contactId, string token)
        {

            return GetSkills(contactId, token);
        }

        public Person getLoggedInUserProfile(String token)
        {
            var contactId = AuthenticationService.GetContactId(token);
            var pageId = Convert.ToInt32(ConfigurationManager.AppSettings["MyProfile"]);
            JArray contact = MinistryPlatformService.GetRecordsArr(pageId, token);
            if (contact.Count == 0)
            {
                throw new InvalidOperationException("getLoggedInUserProfile - no data returned.");
            }
            var contactJson = TranslationService.DecodeJson(contact.ToString());
                      
            var person = new Person            
            {
                Contact_Id = contactJson.Contact_Id,
                Email_Address = contactJson.Email_Address,
                NickName = contactJson.Nickname,
                First_Name = contactJson.First_Name,
                Middle_Name = contactJson.Middle_Name,
                Last_Name = contactJson.Last_Name,
                Maiden_Name = contactJson.Maiden_Name,
                Mobile_Phone = contactJson.Mobile_Phone,
                Mobile_Carrier = contactJson.Mobile_Carrier_ID,
                Date_of_Birth = contactJson.Date_of_Birth,
                Marital_Status_Id = contactJson.Marital_Status_ID,
                Gender_Id = contactJson.Gender_ID,
                Employer_Name = contactJson.Employer_Name,
                Address_Line_1 = contactJson.Address_Line_1,
                Address_Line_2 = contactJson.Address_Line_2,
                City = contactJson.City,
                State = contactJson.State,
                Postal_Code = contactJson.Postal_Code,
                Anniversary_Date = contactJson.Anniversary_Date,
                Foreign_Country = contactJson.Foreign_Country,
                County = contactJson.County,
                Home_Phone = contactJson.Home_Phone,
                Congregation_ID = contactJson.Congregation_ID,
                Household_ID = contactJson.Household_ID,
                Address_Id = contactJson.Address_ID
            };

            return person;
        }

        private List<Models.Crossroads.Skill> GetSkills(int recordId, string token)
        {
            var attributes = GetMyRecords.GetMyAttributes(recordId, token);

            var skills = AutoMapper.Mapper.Map<List<MinistryPlatform.Models.Attribute>, List<Models.Crossroads.Skill>>(attributes);

            return skills;

        }
        

        

    }
}