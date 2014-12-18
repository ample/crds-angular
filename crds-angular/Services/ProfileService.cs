﻿using crds_angular.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace crds_angular.Services
{
    public static class ProfileService
    {

        public static void setProfile(String token, Profile profile)
        {
            MinistryPlatform.Translation.Services.UpdatePageRecordService.UpdateRecord(455, getDictionary(profile.person), token);
            MinistryPlatform.Translation.Services.UpdatePageRecordService.UpdateRecord(465, getDictionary(profile.household), token);       
        }

        private static Dictionary<string, object> getDictionary(Object input)
        {
            var dictionary = input.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(input, null));
            return dictionary;
        }

        public static Profile getLoggedInUserProfile(String token)
        {
            dynamic contactJson;
            int householdId;
            var person = GetPerson(token, out householdId);
            
            Household house = new Household();
            Address address = new Address();
            try
            {
                var household = crds_angular.Services.TranslationService.GetMyHousehold(householdId, token);
                var houseJson = TranslationService.DecodeJson(household);
                house.Household_ID = householdId.ToString();
                house.Home_Phone = houseJson.Home_Phone;
                house.Congregation_ID = houseJson.Congregation_ID.ToString();

                var addressId = houseJson.Address_ID;
                var addr = crds_angular.Services.TranslationService.GetMyAddress(addressId, token);
                var addressJson = TranslationService.DecodeJson(addr);
                address.Street = addressJson.Address_Line_1;
                address.Street2 = addressJson.Address_Line_2;
                address.City = addressJson.City;
                address.State = addressJson["State/Region"];
                address.Zip = addressJson.Postal_Code;
                address.Country = addressJson.Foreign_Country;
                address.County = addressJson.County;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            var profile = new Profile();
            profile.person = person;
            profile.household = house;
            return profile;        
        }

        private static Person GetPerson(String token, out int householdId)
        {
            var contactId = MinistryPlatform.Translation.AuthenticationService.GetContactId(token);
            JArray contact = MinistryPlatform.Translation.Services.GetPageRecordService.GetRecord(455, contactId, token);
            var contactJson = TranslationService.DecodeJson(contact.ToString());
            householdId = contactJson.Household_ID;
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
                Mobile_Carrier = contactJson.Mobile_Carrier,
                Date_of_Birth = contactJson.Date_of_Birth,
                Marital_Status_Id = contactJson.Marital_Status_ID,
                Gender_Id = contactJson.Gender_ID,
                Employer_Name = contactJson.Employer_Name,
                Anniversary_Date = contactJson.Anniversary_Date
            };
            return person;
        }
    }
}