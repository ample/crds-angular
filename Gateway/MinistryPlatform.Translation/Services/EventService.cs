﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.PlatformService;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class EventService : BaseService, IEventService
    {
        private readonly log4net.ILog logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly int EventParticipantSubPageId = Convert.ToInt32(AppSettings("EventsParticipants"));
        private readonly int EventParticipantPageId = Convert.ToInt32(AppSettings("EventParticipant"));

        private readonly int EventParticipantStatusDefaultID =
            Convert.ToInt32(AppSettings("Event_Participant_Status_Default_ID"));

        private IMinistryPlatformService ministryPlatformService;

        public EventService(IMinistryPlatformService ministryPlatformService)
        {
            this.ministryPlatformService = ministryPlatformService;
        }

        public int registerParticipantForEvent(int participantId, int eventId)
        {
            logger.Debug("Adding participant " + participantId + " to event " + eventId);
            var values = new Dictionary<string, object>
            {
                {"Participant_ID", participantId},
                {"Event_ID", eventId},
                {"Participation_Status_ID", EventParticipantStatusDefaultID},
            };

            int eventParticipantId;
            try
            {
                eventParticipantId =
                    WithApiLogin<int>(
                        apiToken =>
                        {
                            return
                                (ministryPlatformService.CreateSubRecord(EventParticipantSubPageId, eventId, values,
                                    apiToken,
                                    true));
                        });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("registerParticipantForEvent failed.  Participant Id: {0}, Event Id: {1}",
                        participantId, eventId), ex.InnerException);
            }

            logger.Debug(string.Format("Added participant {0} to event {1}; record id: {2}", participantId, eventId,
                eventParticipantId));
            return (eventParticipantId);
        }

        public int unRegisterParticipantForEvent(int participantId, int eventId)
        {
            logger.Debug("Removing participant " + participantId + " from event " + eventId);
            
            int eventParticipantId;
            try
            {
                // go get record id to delete
                var recordId = GetEventParticipantRecordId(eventId, participantId);
                eventParticipantId = ministryPlatformService.DeleteRecord(EventParticipantPageId, recordId, null, apiLogin());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format("unRegisterParticipantForEvent failed.  Participant Id: {0}, Event Id: {1}",
                        participantId, eventId), ex.InnerException);
            }

            logger.Debug(string.Format("Removed participant {0} from event {1}; record id: {2}", participantId, eventId,
                eventParticipantId));
            return (eventParticipantId);
        }

        private int GetEventParticipantRecordId(int eventId, int participantId)
        {
            var search = "," + eventId + "," + participantId;
            var participants = ministryPlatformService.GetPageViewRecords("EventParticipantByEventIdAndParticipantId", apiLogin(), search).Single();
            return (int) participants["Event_Participant_ID"];
        }

        public List<Event> GetEvents(string eventType, string token)
        {
            //this is using the basic Events page, any concern there?
            var pageId = Convert.ToInt32(ConfigurationManager.AppSettings["Events"]);
            var search = ",," + eventType;
            var records = ministryPlatformService.GetRecordsDict(pageId, token, search);

            return records.Select(record => new Event
            {
                EventTitle = (string) record["Event_Title"],
                EventType = (string) record["Event_Type"],
                EventStartDate = (DateTime) record["Event_Start_Date"],
                EventEndDate = (DateTime) record["Event_End_Date"],
                EventId = (int) record["dp_RecordID"]
            }).ToList();
        }

        public List<Event> GetEventsByTypeForRange(int eventTypeId, DateTime startDate, DateTime endDate, string token)
        {
            const string viewKey = "EventsWithEventTypeId";
            var search = ",," + eventTypeId;
            var eventRecords = ministryPlatformService.GetPageViewRecords(viewKey, token, search);

            var events = eventRecords.Select(record => new Event
            {
                EventTitle = record.ToString("Event Title"),
                EventType = record.ToString("Event Type"),
                EventStartDate = record.ToDate("Event Start Date", true),
                EventEndDate = record.ToDate("Event End Date", true),
                EventId = record.ToInt("dp_RecordID")
            }).ToList();

            //now we have a list, filter by date range.
            var filteredEvents =
                events.Where(e => e.EventStartDate.Date >= startDate.Date && e.EventStartDate.Date <= endDate.Date)
                    .ToList();
            return filteredEvents;
        }
    }
}