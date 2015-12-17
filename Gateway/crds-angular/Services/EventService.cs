﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using crds_angular.Models.Crossroads.Events;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities.Functions;
using Crossroads.Utilities.Interfaces;
using Crossroads.Utilities.Services;
using MinistryPlatform.Translation.Models.People;
using MinistryPlatform.Translation.Services.Interfaces;
using WebGrease.Css.Extensions;
using Event = MinistryPlatform.Models.Event;
using IEventService = crds_angular.Services.Interfaces.IEventService;
using IGroupService = MinistryPlatform.Translation.Services.Interfaces.IGroupService;
using TranslationEventService = MinistryPlatform.Translation.Services.Interfaces.IEventService;

namespace crds_angular.Services
{
    public class EventService : MinistryPlatformBaseService, IEventService
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly TranslationEventService _eventService;
        private readonly IGroupService _groupService;
        private readonly ICommunicationService _communicationService;
        private readonly IContactService _contactService;
        private readonly IContentBlockService _contentBlockService;
        private readonly IApiUserService _apiUserService;
        private readonly IChildcareService _childcareService;
        private readonly IContactRelationshipService _contactRelationshipService;
        private readonly IGroupParticipantService _groupParticipantService;

        private readonly List<string> TABLE_HEADERS = new List<string>()
        {
            "Event Date",
            "Registered User",
            "Start Time",
            "End Time",
            "Location"
        };



        public EventService(TranslationEventService eventService,
                            IGroupService groupService,
                            ICommunicationService communicationService,
                            IContactService contactService,
                            IContentBlockService contentBlockService,
                            IConfigurationWrapper configurationWrapper,
                            IApiUserService apiUserService,
                            IChildcareService childcareService,
                            IContactRelationshipService contactRelationshipService,
                            IGroupParticipantService groupParticipantService)
        {
            _eventService = eventService;
            _groupService = groupService;
            _communicationService = communicationService;
            _contactService = contactService;
            _contentBlockService = contentBlockService;
            _configurationWrapper = configurationWrapper;
            _apiUserService = apiUserService;
            _childcareService = childcareService;
            _contactRelationshipService = contactRelationshipService;
            _groupParticipantService = groupParticipantService;
        }

        public Event GetEvent(int eventId)
        {
            return _eventService.GetEvent(eventId);
        }

        public void RegisterForEvent(List<EventRsvpDTO> eventDto, String token)
        {
            try
            {
                var saved = eventDto.Select(dto =>
                {
                    var groupParticipantId = _groupParticipantService.Get(dto.GroupId, dto.ParticipantId);
                    if (groupParticipantId == 0)
                    {
                        groupParticipantId = _groupService.addParticipantToGroup(dto.ParticipantId, dto.GroupId, AppSetting("Group_Role_Default_ID"), dto.ChildCareNeeded, DateTime.Today);
                    }

                    // validate that there is not a participant record before creating
                    var retVal = Functions.IntegerReturnValue(() => !_eventService.EventHasParticipant(dto.EventId, dto.ParticipantId) ? _eventService.RegisterParticipantForEvent(dto.ParticipantId, dto.EventId, dto.GroupId, groupParticipantId) : 1);

                    return new RegisterEventObj()
                    {
                        EventId = dto.EventId,
                        ParticipantId = dto.ParticipantId,
                        RegisterResult = retVal,
                        ChildcareRequested = dto.ChildCareNeeded
                    };
                }).ToList();

                SendRsvpMessage(saved, token);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to add event participant: " + e.Message);
            }
        }

        public IList<Models.Crossroads.Events.Event> EventsReadyForReminder(string token)
        {
            var pageId = AppSetting("EventsReadyForReminder");
            var events = _eventService.EventsByPageId(token, pageId);
            var eventList = AutoMapper.Mapper.Map<List<crds_angular.Models.Crossroads.Events.Event>>(events);
            
            // Childcare will be included in the email for event, so don't send a duplicate.
            return eventList.Where(evt => evt.EventType != "Childcare").ToList();
        }

        public IList<Participant> EventPaticpants(int eventId, string token)
        {
            return _eventService.EventParticipants(token, eventId).ToList();
        }

        public void SendReminderEmails()
        {
            var token = _apiUserService.GetToken();
            var eventList = EventsReadyForReminder(token);
            
            eventList.ForEach(evt =>
            {
                // get the participants...
                var participants = EventPaticpants(evt.EventId, token);

                // does the event have a childcare event?
                var childcare = _childcareService.GetChildcareEvent(evt.EventId);
                var childcareParticipants = childcare != null ? EventPaticpants(childcare.EventId, token) : new List<Participant>();

                participants.ForEach(participant => SendEventReminderEmail(evt, participant, childcare, childcareParticipants, token));
                _eventService.SetReminderFlag(evt.EventId, token);
            });
            
        }

        private void SendEventReminderEmail(Models.Crossroads.Events.Event evt, Participant participant, Event childcareEvent, IList<Participant> children, string token)
        {
            var mergeData = new Dictionary<string, object>
            {
                {"Nickname", participant.Nickname},
                {"Event_Title", evt.name},
                {"Event_Start_Date", evt.StartDate.ToShortDateString()},
                {"Event_Start_Time", evt.StartDate.ToShortTimeString()},
                {"cmsChildcareEventReminder", string.Empty},
                {"Childcare_Children", string.Empty},
                {"Childcare_Contact", string.Empty} // Set these three parameters no matter what...
            };

            if (children.Any())
            {
                // determine if any of the children are related to the participant
                var relationships = _contactRelationshipService.GetMyCurrentRelationships(participant.ContactId, token);
                var mine = children.Where(child => relationships.Any(rel => rel.Contact_Id == child.ContactId)).ToList();
                // build the HTML for the [Childcare] data
                if (mine.Any())
                {
                    mergeData.Add("cmsChildcareEventReminder", _contentBlockService["cmsChildcareEventReminder"].Content);
                    var childcareString = ChildcareData(mine);
                    mergeData.Add("Childcare_Children", childcareString);
                    mergeData.Add("Childcare_Contact", new HtmlElement("span", "If you need to cancel, please email " + childcareEvent.PrimaryContact.EmailAddress).Build());
                }
            }
            var defaultContact = _contactService.GetContactById(AppSetting("DefaultContactEmailId"));
            var comm = _communicationService.GetTemplateAsCommunication(
               AppSetting("EventReminderTemplateId"),
               defaultContact.Contact_ID,               
               defaultContact.Email_Address,
               evt.PrimaryContactId,
               evt.PrimaryContactEmailAddress,
               participant.ContactId,
               participant.EmailAddress,
               mergeData
               );

            _communicationService.SendMessage(comm);
        }
 
        private String ChildcareData(IList<Participant> children)
        {
            var el = new HtmlElement("span",
                                     new Dictionary<string, string>(),
                                     "You have indicated that you need childcare for the following children:")
                                     .Append(new HtmlElement("ul").Append(children.Select(child => new HtmlElement("li", child.DisplayName)).ToList()));
            return el.Build();
        }

        private void SendRsvpMessage(List<RegisterEventObj> saved, string token)
        {
            var evnt = _eventService.GetEvent(saved.First().EventId);
            var childcareRequested = saved.Any(s => s.ChildcareRequested);
            var loggedIn = _contactService.GetMyProfile(token);

            var childcareHref = new HtmlElement("a", 
                new Dictionary<string, string>()
                {
                    {
                        "href", 
                        string.Format("https://{0}/childcare/{1}", _configurationWrapper.GetConfigValue("BaseUrl"), evnt.EventId)
                    }
                }, 
                "this link").Build(); 
            var childcare = _contentBlockService["eventRsvpChildcare"].Content.Replace("[url]", childcareHref);

            var mergeData = new Dictionary<string, object>
            {
                {"Event_Name", evnt.EventTitle},
                {"HTML_Table", SetupTable(saved, evnt).Build()},
                {"Childcare", (childcareRequested) ? childcare : ""}
            };
            var defaultContact = _contactService.GetContactById(AppSetting("DefaultContactEmailId"));
            var comm = _communicationService.GetTemplateAsCommunication(
                AppSetting("OneTimeEventRsvpTemplate"),
                defaultContact.Contact_ID,
                defaultContact.Email_Address,
                evnt.PrimaryContact.ContactId,
                evnt.PrimaryContact.EmailAddress,
                loggedIn.Contact_ID,
                loggedIn.Email_Address,
                mergeData
                );

            _communicationService.SendMessage(comm);
        }

        private HtmlElement SetupTable(List<RegisterEventObj> regData, Event evnt)
        {
            var tableAttrs = new Dictionary<string, string>()
            {
                {"width", "100%"},
                {"border", "1"},
                {"cellspacing", "0"},
                {"cellpadding", "5"}
            };

            var cellAttrs = new Dictionary<string, string>()
            {
                {"align", "center"}
            };

            var htmlrows = regData.Select(rsvp =>
            {
                var p = _contactService.GetContactByParticipantId(rsvp.ParticipantId);
                return new HtmlElement("tr")
                    .Append(new HtmlElement("td", cellAttrs, evnt.EventStartDate.ToShortDateString()))
                    .Append(new HtmlElement("td", cellAttrs, p.First_Name + " " + p.Last_Name))
                    .Append(new HtmlElement("td", cellAttrs, evnt.EventStartDate.ToShortTimeString()))
                    .Append(new HtmlElement("td", cellAttrs, evnt.EventEndDate.ToShortTimeString()))
                    .Append(new HtmlElement("td", cellAttrs, evnt.EventLocation));
            }).ToList();

            return new HtmlElement("table", tableAttrs)
                .Append(SetupTableHeader)
                .Append(htmlrows);
        }

        private HtmlElement SetupTableHeader()
        {
            var headers = TABLE_HEADERS.Select(el => new HtmlElement("th", el)).ToList();
            return new HtmlElement("tr", headers);
        }

        private class RegisterEventObj
        {
            public int RegisterResult { get; set; }
            public int ParticipantId { get; set; }
            public int EventId { get; set; }
            public bool ChildcareRequested { get; set; }
        }
    }
}