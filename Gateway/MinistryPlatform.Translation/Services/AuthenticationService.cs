﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Exceptions;
using MinistryPlatform.Translation.PlatformService;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Services
{
    public class AuthenticationService
    {
        public static Boolean ChangePassword(string token, string emailAddress, string firstName, string lastName, string password, string mobilephone)
        {
            var platformService = new PlatformServiceClient();
            using (new OperationContextScope((IClientChannel)platformService.InnerChannel))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                try
                {
                    UserInfo user = new UserInfo();
                    user.EmailAddress = emailAddress;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.NewPassword = password;
                    user.MobilePhone = mobilephone;
                    platformService.UpdateCurrentUserInfo(user);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

        }

        /// <summary>
        /// Change a users password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static Boolean ChangePassword(string token, string newPassword)
        {
            try
            {
                var record = MinistryPlatformService.GetRecordsDict(Convert.ToInt32(ConfigurationManager.AppSettings["ChangePassword"]), token).Single();
                record["Password"] = newPassword;
                MinistryPlatformService.UpdateRecord(Convert.ToInt32(ConfigurationManager.AppSettings["ChangePassword"]), record, token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //get token using logged in user's credentials
        public static Dictionary<string, object> authenticate(string username, string password)
        {
            var userCredentials =
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password},
                    {"client_id", "client"},
                    {"client_secret", "secret"},
                    {"grant_type", "password"}
                });
            var client = new HttpClient();
            var tokenUrl = ConfigurationManager.AppSettings["TokenURL"];
            var message = client.PostAsync(tokenUrl, userCredentials);
            try
            {
                var result = message.Result.Content.ReadAsStringAsync().Result;
                var obj = JObject.Parse(result);
                var token = (string)obj["access_token"];
                var exp = (string)obj["expires_in"];
                //ignorning refreshToken for now
                var refreshToken = (string)obj["refresh_token"];
                var authData = new Dictionary<string, object>
                {
                    {"token", token},
                    {"exp", exp}
                };
                return authData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //Get ID of currently logged in user
        public static int GetContactId(string token)
        {
            var platformService = new PlatformServiceClient();
            using (new OperationContextScope((IClientChannel)platformService.InnerChannel))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                var contactId = platformService.GetCurrentUserInfo();
                return contactId.ContactId;
            }
        }

        //Get Participant IDs of a contact
        public static Participant GetParticipantRecord(string token)
        {
            try
            {
                var platformService = new PlatformServiceClient();
                using (new OperationContextScope(platformService.InnerChannel))
                {
                    if (WebOperationContext.Current != null)
                        WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    var results =
                        MinistryPlatformService.GetRecordsDict(
                            Convert.ToInt32(ConfigurationManager.AppSettings["MyParticipantRecords"]), token);
                    var participant = new Participant
                    {
                        ParticipantId = int.Parse(results.Single()["dp_RecordID"].ToString())
                    };

                    return participant;
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Sequence contains more than one element")
                {
                    throw new MultipleRecordsException("Multiple Participant records found! Only one participant allowed per Contact.");
                }
            }

            return null;
        }
    }
}
