﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using log4net;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Exceptions;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class CommunicationService : BaseService, ICommunicationService

    {
        private readonly int _messagePageId = Convert.ToInt32(AppSettings("MessagesPageId"));
        private readonly int _recipientsSubPageId = Convert.ToInt32(AppSettings("RecipientsSubpageId"));
        private readonly int _communicationStatusId = Convert.ToInt32(AppSettings("CommunicationStatusId"));
        private readonly int _actionStatusId = Convert.ToInt32(AppSettings("ActionStatusId"));
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMinistryPlatformService _ministryPlatformService;

        public CommunicationService(IMinistryPlatformService ministryPlatformService)
        {
            _ministryPlatformService = ministryPlatformService;
        }

        public CommunicationPreferences GetPreferences(String token, int userId)
        {
            int pNum = Convert.ToInt32( ConfigurationManager.AppSettings["MyContact"]);
            int hNum = Convert.ToInt32(ConfigurationManager.AppSettings["MyHousehold"]);
            var profile = _ministryPlatformService.GetRecordDict(pNum, userId, token);
            var household = _ministryPlatformService.GetRecordDict(hNum, (int)profile["Household_ID"], token);
            return new CommunicationPreferences
            {
                Bulk_Email_Opt_Out = (bool)profile["Bulk_Email_Opt_Out"],
                Bulk_Mail_Opt_Out = (bool)household["Bulk_Mail_Opt_Out"],
                Bulk_SMS_Opt_Out = (bool)profile["Bulk_SMS_Opt_Out"]
            };
        }

        public bool SetEmailSMSPreferences(String token, Dictionary<string,object> prefs){
            int pId = Convert.ToInt32(ConfigurationManager.AppSettings["MyContact"]);
            _ministryPlatformService.UpdateRecord(pId, prefs, token);
            return true;
        }

        public bool SetMailPreferences(string token, Dictionary<string,object> prefs){
            int pId = Convert.ToInt32(ConfigurationManager.AppSettings["MyHousehold"]);
            _ministryPlatformService.UpdateRecord(pId, prefs, token);
            return true;
        }

        /// <summary>
        /// Creates the correct record in MP so that the mail service can pick it up and send 
        /// it during the scheduled run
        /// </summary>
        /// <param name="communication">The message properties </param>
        /// <param name="mergeData">A dictionary of varible names and their values that MP will place in the template</param>
        public void SendMessage(Communication communication, Dictionary<string, object> mergeData)
        {
            var token = apiLogin();
            var communicationId = AddCommunication(communication, token);
            AddCommunicationMessage(communication, communicationId, mergeData, token);
        }

        private int AddCommunication(Communication communication, string token)
        {
            var dictionary = new Dictionary<string, object>
            {
                {"Subject", communication.EmailSubject},
                {"Body", communication.EmailBody},
                {"Author_User_Id", communication.AuthorUserId},
                {"Start_Date", DateTime.Now},
                {"From_Contact", communication.FromContactId},
                {"Reply_to_Contact", communication.ReplyContactId},
                {"Communication_Status_ID", _communicationStatusId}
            };
            var communicationId = _ministryPlatformService.CreateRecord(_messagePageId, dictionary, token);
            return communicationId;
        }

        private void AddCommunicationMessage(Communication communication, int communicationId, Dictionary<string, object> mergeData, string token)
        {
            var dictionary = new Dictionary<string, object>
            {
                {"Action_Status_ID", _actionStatusId},
                {"Action_Status_Time", DateTime.Now},
                {"Contact_ID", communication.ToContactId},
                {"From", communication.FromEmailAddress},
                {"To", communication.ToEmailAddress},
                {"Reply_To", communication.ReplyToEmailAddress},
                {"Subject", communication.EmailSubject},
                {"Body", ParseTemplateBody(communication.EmailBody, mergeData)}
            };
            _ministryPlatformService.CreateSubRecord(_recipientsSubPageId, communicationId, dictionary, token);
        }

        public MessageTemplate GetTemplate(int templateId)
        {
            var pageRecords = _ministryPlatformService.GetRecordDict(_messagePageId, templateId, apiLogin());

            if (pageRecords == null)
            {
                throw new InvalidOperationException("Couldn't find message template.");
            }

            var template = new MessageTemplate
            {
                Body = pageRecords["Body"].ToString(),
                Subject = pageRecords["Subject"].ToString()
            };

            return template;
        }

        public string ParseTemplateBody(string templateBody, Dictionary<string, object> record)
        {
            try
            {
                return record.Aggregate(templateBody,
                    (current, field) => current.Replace("[" + field.Key + "]", field.Value.ToString()));
            }
            catch (Exception ex)
            {
                _logger.Debug(string.Format("Failed to parse the template"));
                throw new TemplateParseException("Failed to parse the template", ex);
            }
        }
    }
}