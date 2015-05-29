using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MinistryPlatform.Models;
using MinistryPlatform.Models.DTO;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class OpportunityServiceImpl : BaseService, IOpportunityService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly int _eventPage = Convert.ToInt32(AppSettings("Events"));
        private readonly IEventService _eventService;

        private readonly int _groupOpportunitiesEventsPageViewId =
            Convert.ToInt32(AppSettings("GroupOpportunitiesEvents"));

        private readonly int _groupParticpantsSubPageView = Convert.ToInt32(AppSettings("GroupsParticipantsSubPage"));
        private readonly IMinistryPlatformService _ministryPlatformService;
        private readonly int _opportunityPage = Convert.ToInt32(AppSettings("OpportunityPage"));
        private readonly int _opportunityResponses = Convert.ToInt32(AppSettings("OpportunityResponses"));
        private readonly int _signedupToServeSubPageViewId = Convert.ToInt32(AppSettings("SignedupToServe"));
        private readonly int _contactOpportunityResponses = Convert.ToInt32(AppSettings("ContactOpportunityResponses"));

        public OpportunityServiceImpl(IMinistryPlatformService ministryPlatformService, IEventService eventService,
            IAuthenticationService authenticationService)
        {
            _ministryPlatformService = ministryPlatformService;
            _eventService = eventService;
            _authenticationService = authenticationService;
        }

        public Response GetMyOpportunityResponses(int contactId, int opportunityId, string token)
        {
            var searchString = ",,,," + contactId;
            var subpageViewRecords = MinistryPlatformService.GetSubpageViewRecords(_contactOpportunityResponses,
                opportunityId, token, searchString);
            var list = subpageViewRecords.ToList();
            var s = list.SingleOrDefault();
            if (s == null) return null;
            var response = new Response
            {
                Opportunity_ID = (int) s["Opportunity ID"],
                Participant_ID = (int) s["Participant ID"],
                Response_Date = (DateTime) s["Response Date"],
                Response_Result_ID = (int?) s["Response Result ID"]
            };
            return response;
        }

        public Opportunity GetOpportunityById(int opportunityId, string token)
        {
            var opp = _ministryPlatformService.GetRecordDict(_opportunityPage, opportunityId, token);
            var opportunity = new Opportunity
            {
                OpportunityId = opportunityId,
                OpportunityName = opp.ToString("Opportunity_Title"),
                EventType = opp.ToString("Event_Type_ID_Text"),
                EventTypeId = opp.ToInt("Event_Type_ID"),
                RoleTitle = opp.ToString("Group_Role_ID_Text"),
                MaximumNeeded = opp.ToNullableInt("Maximum_Needed"),
                MinimumNeeded = opp.ToNullableInt("Minimum_Needed"),
                GroupContactId = opp.ToInt("Contact_Person"),
                GroupContactName = opp.ToString("Contact_Person_Text"),
                GroupName = opp.ToString("Add_to_Group_Text"),
                ShiftStart = TimeSpan.Parse(opp.ToString("Shift_Start")),
                ShiftEnd = TimeSpan.Parse(opp.ToString("Shift_End")),
                Room = opp.ToString("Room")
            };
            return opportunity;
        }

        public Response GetOpportunityResponse(int opportunityId, int eventId, Participant participant)
        {
            var searchString = string.Format(",{0},{1},{2}", opportunityId, eventId, participant.ParticipantId);
            List<Dictionary<string, object>> dictionaryList;
            try
            {
                dictionaryList =
                    WithApiLogin(
                        apiToken =>
                            (_ministryPlatformService.GetPageViewRecords("ResponseByOpportunityAndEvent", apiToken,
                                searchString, "", 0)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format(
                        "GetOpportunityResponse failed.  Participant Id: {0}, Opportunity Id: {1}, Event Id: {2}",
                        participant, opportunityId, eventId), ex.InnerException);
            }

            if (dictionaryList.Count == 0)
            {
                return new Response();
            }

            var response = new Response();
            try
            {
                var dictionary = dictionaryList.First();
                response.Response_ID = dictionary.ToInt("dp_RecordID");
                response.Opportunity_ID = dictionary.ToInt("Opportunity_ID");
                response.Participant_ID = dictionary.ToInt("Participant_ID");
                response.Response_Result_ID = dictionary.ToInt("Response_Result_ID");
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    string.Format("RespondToOpportunity failed.  Participant Id: {0}, Opportunity Id: {1}",
                        participant, opportunityId), ex.InnerException);
            }


            return response;
        }

        public List<Response> GetOpportunityResponses(int opportunityId, string token)
        {
            var records = _ministryPlatformService.GetSubpageViewRecords(_signedupToServeSubPageViewId, opportunityId,
                token, "");

            var responses = new List<Response>();
            foreach (var r in records)
            {
                var response = new Response();
                response.Event_ID = r.ToInt("Event_ID");
                responses.Add(response);
            }

            return responses;
        }

        public int GetOpportunitySignupCount(int opportunityId, int eventId, string token)
        {
            var search = ",,," + eventId;
            var records = _ministryPlatformService.GetSubpageViewRecords(_signedupToServeSubPageViewId, opportunityId,
                token, search);

            return records.Count();
        }

        public List<DateTime> GetAllOpportunityDates(int opportunityId, string token)
        {
            //First get the event type
            var opp = _ministryPlatformService.GetRecordDict(_opportunityPage, opportunityId, token);
            var eventType = opp["Event_Type_ID_Text"];

            //Now get all the events for this type
            var searchString = ",," + eventType;
            var events = _ministryPlatformService.GetRecordsDict(_eventPage, token, searchString);
            var filteredEvents =
                events.Select(e => DateTime.Parse(e["Event_Start_Date"].ToString()))
                    .Where(eDate => eDate >= DateTime.Today)
                    .OrderBy(d => d)
                    .ToList();
            filteredEvents.Sort();
            return filteredEvents;
        }

        public DateTime GetLastOpportunityDate(int opportunityId, string token)
        {
            var events = GetAllOpportunityDates(opportunityId, token);
            //grab the last one
            try
            {
                return events.Last();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("No events found. Cannot return the last event date.");
            }
        }

        public void RespondToOpportunity(RespondToOpportunityDto opportunityResponse)
        {
            foreach (var participant in opportunityResponse.Participants)
            {
                var comments = string.Format("Request on {0}", DateTime.Now.ToString(CultureInfo.CurrentCulture));
                var values = new Dictionary<string, object>
                {
                    {"Response_Date", DateTime.Now},
                    {"Opportunity_ID", opportunityResponse.OpportunityId},
                    {"Participant_ID", participant},
                    {"Closed", false},
                    {"Comments", comments}
                };
                _ministryPlatformService.CreateRecord("OpportunityResponses", values, apiLogin(), true);
            }
        }

        public int RespondToOpportunity(string token, int opportunityId, string comments)
        {
            var participant = _authenticationService.GetParticipantRecord(token);
            var participantId = participant.ParticipantId;

            var values = new Dictionary<string, object>
            {
                {"Response_Date", DateTime.Now},
                {"Opportunity_ID", opportunityId},
                {"Participant_ID", participantId},
                {"Closed", false},
                {"Comments", comments}
            };

            var recordId = _ministryPlatformService.CreateRecord("OpportunityResponses", values, token, true);
            return recordId;
        }

        public int DeleteResponseToOpportunities(int participantId, int opportunityId, int eventId)
        {
            var participant = new Participant { ParticipantId = participantId };

            try
            {
                var prevResponse = GetOpportunityResponse(opportunityId, eventId, participant);
                if (prevResponse.Response_ID != 0)
                {
                    _ministryPlatformService.DeleteRecord(_opportunityResponses, prevResponse.Response_ID, null, apiLogin());
                    return prevResponse.Response_ID;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("Delete Response failed.  Participant Id: {0}, Opportunity Id: {1}",
                        participantId, opportunityId), ex.InnerException);
            }

        }

        public int RespondToOpportunity(int participantId, int opportunityId, string comments, int eventId, bool response)
        {
            var participant = new Participant {ParticipantId = participantId};

            var values = new Dictionary<string, object>
            {
                {"Response_Date", DateTime.Now},
                {"Opportunity_ID", opportunityId},
                {"Participant_ID", participantId},
                {"Closed", false},
                {"Comments", comments},
                {"Event_ID", eventId},
                {"Response_Result_ID", (response) ? 1 : 2}
            };

            //Go see if there are existing responses for this opportunity that we are updating
            int recordId = 0;

            try
            {
                var prevResponse = GetOpportunityResponse(opportunityId, eventId, participant);
                if (prevResponse.Response_ID != 0)
                {
                    recordId = prevResponse.Response_ID;
                    values.Add("Response_ID", recordId);
                    _ministryPlatformService.UpdateRecord(_opportunityResponses, values, apiLogin());
                }
                else
                {
                    WithApiLogin(apiToken => (_ministryPlatformService.CreateRecord("OpportunityResponses", values, apiToken, true)));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("RespondToOpportunity failed.  Participant Id: {0}, Opportunity Id: {1}",
                        participantId, opportunityId), ex.InnerException);
            }
            return recordId;
        }

        public Group GetGroupParticipantsForOpportunity(int opportunityId, string token)
        {
            var opp = _ministryPlatformService.GetRecordDict(_opportunityPage, opportunityId, token);
            var groupId = opp.ToInt("Add_to_Group");
            var groupName = opp.ToString("Add_to_Group_Text");
            var searchString = ",,,," + opp.ToString("Group_Role_ID");
            var eventTypeId = opp.ToInt("Event_Type_ID");
            var group = _ministryPlatformService.GetSubpageViewRecords(_groupParticpantsSubPageView, groupId, token,
                searchString);
            var participants = new List<GroupParticipant>();
            foreach (var groupParticipant in group)
            {
                participants.Add(new GroupParticipant
                {
                    ContactId = groupParticipant.ToInt("Contact_ID"),
                    GroupRoleId = groupParticipant.ToInt("Group_Role_ID"),
                    GroupRoleTitle = groupParticipant.ToString("Role_Title"),
                    LastName = groupParticipant.ToString("Last_Name"),
                    NickName = groupParticipant.ToString("Nickname"),
                    ParticipantId = groupParticipant.ToInt("dp_RecordID")
                });
            }
            var retGroup = new Group
            {
                GroupId = groupId,
                Name = groupName,
                Participants = participants,
                EventTypeId = eventTypeId
            };
            return retGroup;
        }
    }
}
