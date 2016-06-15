﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Models.Childcare;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class ChildcareRequestService :  IChildcareRequestService
    {
        private readonly IMinistryPlatformService _ministryPlatformService;
        private readonly IApiUserService _apiUserService;
        private readonly IEventService _eventService;
        private readonly int _childcareRequestDatesId;
        private readonly int _myChildcareRequestDatesId;
        private readonly int _childcareRequestPageId;
        private readonly int _childcareRequestDatesPageId;
        private readonly int _childcareRequestStatusPending;
        private readonly int _childcareRequestStatusApproved;
        private readonly int _childcareEmailPageViewId;
        private readonly int _childcareEventType;
        private readonly IGroupService _groupService;

        public ChildcareRequestService(IConfigurationWrapper configurationWrapper, IMinistryPlatformService ministryPlatformService, IApiUserService apiUserService, IEventService eventService, IGroupService groupService)
        {
            _ministryPlatformService = ministryPlatformService;
            _apiUserService = apiUserService;
            _eventService = eventService;
            _childcareRequestPageId = configurationWrapper.GetConfigIntValue("ChildcareRequestPageId");
            _childcareRequestDatesPageId = configurationWrapper.GetConfigIntValue("ChildcareRequestDatesPageId");
            _childcareEmailPageViewId = configurationWrapper.GetConfigIntValue("ChildcareEmailPageView");
            _childcareRequestStatusPending = configurationWrapper.GetConfigIntValue("ChildcareRequestPending");
            _childcareRequestStatusApproved = configurationWrapper.GetConfigIntValue("ChildcareRequestApproved");
            _childcareEventType = configurationWrapper.GetConfigIntValue("ChildcareEventType");
            _groupService = groupService;
            _childcareRequestDatesId = configurationWrapper.GetConfigIntValue("ChildcareRequestDates");
            _myChildcareRequestDatesId = configurationWrapper.GetConfigIntValue("MyChildcareRequestDates");
        }

        public int CreateChildcareRequest(ChildcareRequest request)
        {
            var apiToken = _apiUserService.GetToken();

            var requestDict = new Dictionary<string, object>
            {
                {"Requester_ID", request.RequesterId},
                {"Congregation_ID", request.LocationId },
                {"Ministry_ID", request.MinistryId },
                {"Group_ID", request.GroupId },
                {"Start_Date", request.StartDate },
                {"End_Date", request.EndDate },
                {"Frequency", request.Frequency },
                {"Childcare_Session", request.PreferredTime },
                {"Notes", request.Notes },
                {"Request_Status_ID", _childcareRequestStatusPending }
            };
            var childcareRequestId = _ministryPlatformService.CreateRecord(_childcareRequestPageId, requestDict, apiToken);

            return childcareRequestId;
        }

        public ChildcareRequestEmail GetChildcareRequest(int childcareRequestId, string token)
        {
            var searchString = string.Format("{0},", childcareRequestId);
            var r = _ministryPlatformService.GetPageViewRecords(_childcareEmailPageViewId, token,searchString);
            if (r.Count == 1)
            {
                var record = r[0];

                var c = new ChildcareRequestEmail
                {
                    RequestId = record.ToInt("Childcare_Request_ID"),
                    RequesterId = record.ToInt("Contact_ID"),
                    RequesterEmail = record.ToString("Email_Address"),
                    RequesterNickname = record.ToString("Nickname"),
                    RequesterLastName = record.ToString("Last_Name"),
                    GroupName = record.ToString("Group_Name"),
                    MinistryName = record.ToString("Ministry_Name"),
                    StartDate = record.ToDate("Start_Date"),
                    EndDate = record.ToDate("End_Date"),
                    ChildcareSession = record.ToString("Childcare_Session"),
                    Requester = record.ToString("Display_Name"),
                    ChildcareContactEmail = record.ToString("Childcare_Contact_Email_Address"),
                    ChildcareContactId = record.ToInt("Childcare_Contact_ID"),
                    CongregationName = record.ToString("Congregation_Name")
                };

                return c;

            }
            if (r.Count == 0)
            {
                return null;
            }
            throw new ApplicationException(string.Format("Duplicate Childcare Request ID detected: {0}", childcareRequestId));
        }

        public void CreateChildcareRequestDates(int childcareRequestId, ChildcareRequest request, string token)
        {
          var datesList = request.DatesList;
          foreach (var date in datesList)
          {
            var requestDate = new DateTime(date.Year,date.Month,date.Day,0,0,0);
            var requestDatesDict = new Dictionary<String, Object>
            {
              {"Childcare_Request_ID", childcareRequestId },
                {"Childcare_Request_Date", requestDate},
                {"Approved", false }
            };
            _ministryPlatformService.CreateSubRecord(_childcareRequestDatesId, childcareRequestId, requestDatesDict, token, false);
          }
        }

        public List<ChildcareRequestDate> GetChildcareRequestDates(int childcareRequestId)
        {
            var apiToken = _apiUserService.GetToken();
            var searchString = String.Format("{0},", childcareRequestId);
            var records = _ministryPlatformService.GetRecordsDict(_childcareRequestDatesPageId, apiToken, searchString);

            return records.Select(rec => new ChildcareRequestDate
            {
                Approved = rec.ToBool("Approved"),
                ChildcareRequestDateId = rec.ToInt("dp_RecordID"),
                ChildcareRequestId = rec.ToInt("Childcare_Request_ID"),
                RequestDate = rec.ToDate("Childcare_Request_Date")
            }).ToList();
        }

        public void DecisionChildcareRequest(int childcareRequestId, int requestStatusId, ChildcareRequest childcareRequest)
        {
            var apiToken = _apiUserService.GetToken();
            

            var requestDict = new Dictionary<string, object>
            {
                {"Childcare_Request_ID", childcareRequestId },                        
                {"Request_Status_ID", requestStatusId }
            };
            if (childcareRequest.DecisionNotes != null)
            {
                requestDict.Add("Decision_Notes", childcareRequest.DecisionNotes );
            }

           _ministryPlatformService.UpdateRecord(_childcareRequestPageId, requestDict, apiToken);
        }

        public void DecisionChildcareRequestDate(int childcareRequestDateId, bool decision)
        {
            var apiToken = _apiUserService.GetToken();

            var requestDateDict = new Dictionary<string, object>
            {
                {"Childcare_Request_Date_ID", childcareRequestDateId},
                {"Approved", decision}
            };

            _ministryPlatformService.UpdateRecord(_childcareRequestDatesPageId, requestDateDict, apiToken);
        }

        public ChildcareRequestDate GetChildcareRequestDates(int childcareRequestId, DateTime date, string token)
        {
            var apiToken = _apiUserService.GetToken();
            var searchString = String.Format("{0},", childcareRequestId);
            var mpRecords = _ministryPlatformService.GetRecordsDict(_childcareRequestDatesPageId, apiToken, searchString);
            var requestedDate = new ChildcareRequestDate();

            foreach (var mpRecord in from mpRecord in mpRecords let mpDate = mpRecord.ToDate("Childcare_Request_Date") where date.Date == mpDate.Date select mpRecord)
            {
                requestedDate.RequestDate = mpRecord.ToDate("Childcare_Request_Date");
                requestedDate.ChildcareRequestDateId = mpRecord.ToInt("dp_RecordID");
                requestedDate.ChildcareRequestId = childcareRequestId;
                requestedDate.Approved = mpRecord.ToBool("Approved");
            }
            return requestedDate;
        }

        public Dictionary<int, int> FindChildcareEvents(int childcareRequestId, List<ChildcareRequestDate> requestedDates)
        {
            var apiToken = _apiUserService.GetToken();

            var request = GetChildcareRequestForReview(childcareRequestId);
            var prefTime = request.PreferredTime.Substring(request.PreferredTime.IndexOf(',') + 1).Split('-');
            var requestStartTime = DateTime.ParseExact(prefTime[0].Trim(), "h:mmtt", CultureInfo.InvariantCulture );
            var requestEndTime = DateTime.ParseExact(prefTime[1].Trim(), "h:mmtt", CultureInfo.InvariantCulture);

            var events = _eventService.GetEventsByTypeForRange(_childcareEventType, request.StartDate, request.EndDate, apiToken);

            var reqEvents = new Dictionary<int, int>();
            foreach (var date in requestedDates)
            {
                var foundEvent = events.SingleOrDefault(
                    e => (e.EventStartDate == date.RequestDate.Date.Add(requestStartTime.TimeOfDay)) && 
                    (e.EventEndDate == date.RequestDate.Date.Add(requestEndTime.TimeOfDay)) && 
                    (e.CongregationId == request.LocationId));
                if (foundEvent != null)
                {
                    reqEvents.Add(date.ChildcareRequestDateId, foundEvent.EventId);
                }
            }
            return reqEvents;
        }

        public ChildcareRequest GetChildcareRequestForReview(int childcareRequestId)
        {
            var apiToken = _apiUserService.GetToken();
            var record = _ministryPlatformService.GetRecordDict(_childcareRequestPageId, childcareRequestId, apiToken);
            List<ChildcareRequestDate> daterecords = GetChildcareRequestDates(childcareRequestId);
            var datesList = daterecords.Select(dateRec => dateRec.RequestDate).ToList();

            if (record == null)
            {
                return null;
            }
            var childcareRequest = new ChildcareRequest
            {
                RequesterId = record.ToInt("Requester_ID"),
                LocationId = record.ToInt("Congregation_ID"),
                MinistryId = record.ToInt("Ministry_ID"),
                GroupId = record.ToInt("Group_ID"),
                GroupName = record.ToString("Group_ID_Text"),
                StartDate = record.ToDate("Start_Date"),
                EndDate = record.ToDate("End_Date"),
                Frequency = record.ToString("Frequency"),
                PreferredTime = record.ToString("Childcare_Session"),
                Status = record.ToString("Request_Status_ID_Text"),
                Notes = record.ToString("Notes"),
                DecisionNotes = record.ToString("Decision_Notes"),
                DatesList = datesList
            };

            return childcareRequest;
        }

        public List<ChildcareRequestDate> GetChildcareRequestDatesForReview(int childcareRequestId)
        {
            var apiToken = _apiUserService.GetToken();
            var searchString = String.Format("{0},", childcareRequestId);
            var records = _ministryPlatformService.GetRecordsDict(_childcareRequestDatesPageId, apiToken, searchString);

            return records.Select(rec => new ChildcareRequestDate
            {
                ChildcareRequestDateId = rec.ToInt("dp_RecordID"),
                RequestDate = rec.ToDate("Childcare_Request_Date")
            }).ToList();
        }
    }
}
