﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using MinistryPlatform.Models;

namespace MinistryPlatform.Translation.Services
{
    public class OpportunityService
    {
        public static int RespondToOpportunity(string token, int opportunityId, string comments)
        {
                var participant = AuthenticationService.GetParticipantRecord(token);
                var participantId = participant.ParticipantId;
                var pageId = Convert.ToInt32(ConfigurationManager.AppSettings["OpportunityResponses"]);

                var values = new Dictionary<string, object>
                {
                    {"Response_Date", DateTime.Now},
                    {"Opportunity_ID", opportunityId},
                    {"Participant_ID", participantId},
                    {"Closed", false},
                    {"Comments", comments}
                };

                var recordId = MinistryPlatformService.CreateRecord(pageId, values, token, true);
                return recordId;
        }

        public static List<Opportunity> GetOpportunitiesForGroup(int groupId, string token)
        {
            var subPageId = Convert.ToInt32(ConfigurationManager.AppSettings["GroupOpportunities"]);
            var subPageRecords = MinistryPlatformService.GetSubPageRecords(subPageId, groupId, token);
            var opportunities = new List<Opportunity>();

            foreach (var record in subPageRecords)
            {
                var opportunity = new Opportunity
                {
                    Opportunity_Date = (DateTime) record["Opportunity_Date"],
                    Opportunity_ID = (int) record["dp_RecordID"],
                    Opportunity_Name = (string) record["Opportunity_Title"]
                };
                opportunities.Add(opportunity);
            }
            return opportunities;
        }

        public static Response GetMyOpportunityResponses(int contactId, int opportunityId, string token)
        {
            var subPageViewId = Convert.ToInt32(ConfigurationManager.AppSettings["ContactOpportunityResponses"]);
            var subpageViewRecords = MinistryPlatformService.GetSubpageViewRecords(subPageViewId, opportunityId, token,
                ",,,," + contactId).ToList().Single();
            var response = new Response
            {
                Opportunity_ID = (int) subpageViewRecords["Opportunity ID"],
                Participant_ID = (int) subpageViewRecords["Participant ID"],
                Response_Date = (DateTime) subpageViewRecords["Response Date"],
                Response_Result_ID = (int?) subpageViewRecords["Response Result ID"],
                Opportunity_Date = (DateTime) subpageViewRecords["Opportunity Date"]
            };
            return response;
        }
    }
}