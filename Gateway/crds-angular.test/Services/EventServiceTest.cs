﻿using System;
using System.Collections.Generic;
using System.Linq;
using crds_angular.App_Start;
using crds_angular.Services;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services.Interfaces;
using Event = MinistryPlatform.Models.Event;
using IEventService = MinistryPlatform.Translation.Services.Interfaces.IEventService;
using IGroupService = MinistryPlatform.Translation.Services.Interfaces.IGroupService;
using TranslationEventService = MinistryPlatform.Translation.Services.Interfaces.IEventService;
using Moq;
using MvcContrib.TestHelper.Ui;
using NUnit.Framework;


namespace crds_angular.test.Services
{
    [TestFixture]
    public class EventServiceTest 
    {
        private Mock<IContactRelationshipService> _contactRelationshipService;
        private Mock<IContactService> _contactService;
        private Mock<IContentBlockService> _contentBlockService;
        private Mock<IAuthenticationService> _authenticationService;
        private Mock<IEventService> _eventService;
        private Mock<IParticipantService> _participantService;
        private Mock<IGroupParticipantService> _groupParticipantService;
        private Mock<IGroupService> _groupService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<IConfigurationWrapper> _configurationWrapper;
        private Mock<IApiUserService> _apiUserService;
        private EventService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();
          
            _contactRelationshipService = new Mock<IContactRelationshipService>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _apiUserService = new Mock<IApiUserService>(MockBehavior.Strict);
            _contentBlockService = new Mock<IContentBlockService>(MockBehavior.Strict);
            _contactService = new Mock<IContactService>(MockBehavior.Strict);
            _authenticationService = new Mock<IAuthenticationService>(MockBehavior.Strict);
            _groupService = new Mock<IGroupService>(MockBehavior.Strict);
            _communicationService = new Mock<ICommunicationService>(MockBehavior.Strict);
            _configurationWrapper = new Mock<IConfigurationWrapper>(MockBehavior.Strict);
            _apiUserService = new Mock<IApiUserService>(MockBehavior.Strict);
            _groupParticipantService = new Mock<IGroupParticipantService>(MockBehavior.Strict);
            _participantService = new Mock<IParticipantService>(MockBehavior.Strict);
            _eventService = new Mock<IEventService>();

            _configurationWrapper = new Mock<IConfigurationWrapper>();
            _configurationWrapper.Setup(mocked => mocked.GetConfigIntValue("EventsReadyForPrimaryContactReminder")).Returns(2205);
            _configurationWrapper.Setup(mocked => mocked.GetConfigIntValue("EventPrimaryContactReminderTemplateId")).Returns(14909);
            

            _fixture = new EventService(_eventService.Object,
                                        _groupService.Object,
                                        _communicationService.Object,
                                        _contactService.Object, 
                                        _contentBlockService.Object, 
                                        _configurationWrapper.Object,  
                                        _apiUserService.Object,
                                        _contactRelationshipService.Object,
                                        _groupParticipantService.Object,
                                        _participantService.Object);
        
        }
        

        [Test]
        public void ShouldSendPrimaryContactReminderEmails()
        {

            const int pageViewId = 2205;
            const string search = "";
            const string apiToken = "qwerty1234";
            const int defaultTemplateId = 14909;
            var defaultContact = new MyContact()
            {
                Contact_ID = 321,
                Email_Address = "default@email.com"
            };

            var testEvent = new Event ()
            {
                EventId = 32,
                EventStartDate = new DateTime(),
                EventEndDate = new DateTime().AddHours(2),
                PrimaryContact = new Contact()
                {
                    EmailAddress = "test@test.com",
                    ContactId = 4321
                }
            };

            var testEventList = new List<Event>()
            {
               testEvent
               
            };
       
            var token = _apiUserService.Setup(m => m.GetToken()).Returns(apiToken);
            _eventService.Setup(m => m.EventsByPageViewId(apiToken, pageViewId, search)).Returns(testEventList);
            var eventList = testEventList.Select(evt => new crds_angular.Models.Crossroads.Events.Event() 
            {
                name = evt.EventTitle,
                EventId = evt.EventId,
                EndDate = evt.EventEndDate,
                StartDate = evt.EventStartDate,
                EventType = evt.EventType,
                location = evt.EventLocation,
                PrimaryContactEmailAddress = evt.PrimaryContact.EmailAddress,
                PrimaryContactId = evt.PrimaryContact.ContactId
            });
            
            eventList.ForEach(evt =>
            {
                var mergeData = new Dictionary<string, object>
                {
                    {"Event_ID", evt.EventId},
                    {"Event_Title", evt.name},
                    {"Event_Start_Date", evt.StartDate.ToShortDateString()},
                    {"Event_Start_Time", evt.StartDate.ToShortTimeString()}               
                };

                var contact = new Contact() { ContactId = defaultContact.Contact_ID, EmailAddress = defaultContact.Email_Address };
                var fakeCommunication = new Communication()
                {
                    AuthorUserId = defaultContact.Contact_ID,
                    DomainId = 1,
                    EmailBody = "Some Email Body",
                    EmailSubject = "Whatever",
                    FromContact = contact,
                    MergeData = mergeData,
                    ReplyToContact = contact,
                    TemplateId = defaultTemplateId,
                    ToContacts = new List<Contact>() { contact }
                };

                var testContact = new MyContact()
                {
                    Contact_ID = 9876,
                    Email_Address = "ghj@cr.net"

                };
              
                _contactService.Setup(m => m.GetContactById(123456)).Returns(testContact);
                _communicationService.Setup(m => m.GetTemplateAsCommunication(14909,
                                                                              testContact.Contact_ID,
                                                                              testContact.Email_Address,
                                                                              evt.PrimaryContactId,
                                                                              evt.PrimaryContactEmailAddress,
                                                                              evt.PrimaryContactId,
                                                                              evt.PrimaryContactEmailAddress,
                                                                              mergeData)).Returns(fakeCommunication);
                _communicationService.Setup(m => m.SendMessage(fakeCommunication));
                _communicationService.Verify();

            });
            _eventService.Verify();
        }
        
    }
}
      
