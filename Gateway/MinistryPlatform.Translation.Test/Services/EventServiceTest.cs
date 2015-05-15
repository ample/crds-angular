﻿using System;
using System.Collections.Generic;
using System.IO;
using Crossroads.Utilities.Services;
using MinistryPlatform.Translation.PlatformService;
using MinistryPlatform.Translation.Services;
using MinistryPlatform.Translation.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Services
{
    [TestFixture]
    public class EventServiceTest
    {
        private EventService fixture;
        private Mock<IMinistryPlatformService> ministryPlatformService;
        private const int EventParticipantId = 12345;
        private readonly int EventParticipantPageId = 281;
        private readonly int EventParticipantStatusDefaultID = 2;
        private readonly int EventsPageId = 308;
        private readonly string EventsWithEventTypeId = "EventsWithEventTypeId";

        [SetUp]
        public void SetUp()
        {
            ministryPlatformService = new Mock<IMinistryPlatformService>();
            fixture = new EventService(ministryPlatformService.Object);
        }

        [Test]
        public void testRegisterParticipantForEvent()
        {
            ministryPlatformService.Setup(mocked => mocked.CreateSubRecord(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(),
                It.IsAny<string>(), It.IsAny<Boolean>())).Returns(987);

            var expectedValues = new Dictionary<string, object>
            {
                {"Participant_ID", 123},
                {"Event_ID", 456},
                {"Participation_Status_ID", EventParticipantStatusDefaultID},
            };

            int eventParticipantId = fixture.registerParticipantForEvent(123, 456);

            ministryPlatformService.Verify(mocked => mocked.CreateSubRecord(
                EventParticipantPageId, 456, expectedValues, It.IsAny<string>(), true));

            Assert.AreEqual(987, eventParticipantId);
        }

        [Test]
        public void GetEventsByType()
        {
            const string eventType = "Oakley: Saturday at 4:30";

            var search = ",," + eventType;
            ministryPlatformService.Setup(mock => mock.GetRecordsDict(EventsPageId, It.IsAny<string>(), search, ""))
                .Returns(MockEventsDictionary());

            var events = fixture.GetEvents(eventType, It.IsAny<string>());
            Assert.IsNotNull(events);
        }

        [Test]
        public void GetEventsByTypeAndRange()
        {
            var eventTypeId = 1;
            var search = ",," + eventTypeId;
            ministryPlatformService.Setup(mock => mock.GetPageViewRecords(EventsWithEventTypeId, It.IsAny<string>(), search, "", 0))
                .Returns(MockEventsDictionaryByEventTypeId());

            var startDate = new DateTime(2015,4,1);
            var endDate = new DateTime(2015,4,30);
            var events = fixture.GetEventsByTypeForRange(eventTypeId, startDate, endDate, It.IsAny<string>());
            Assert.IsNotNull(events);
            Assert.AreEqual(3,events.Count);
            Assert.AreEqual("event-title-200", events[0].EventTitle);
        }

        private List<Dictionary<string, object>> MockEventsDictionaryByEventTypeId()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 100},
                    {"Event Title", "event-title-100"},
                    {"Event Type", "event-type-100"},
                    {"Event Start Date", new DateTime(2015, 3, 28, 8, 30, 0)},
                    {"Event End Date", new DateTime(2015, 3, 28, 8, 30, 0)}
                },
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 200},
                    {"Event Title", "event-title-200"},
                    {"Event Type", "event-type-200"},
                    {"Event Start Date", new DateTime(2015, 4, 1, 8, 30, 0)},
                    {"Event End Date", new DateTime(2015, 4, 1, 8, 30, 0)}
                },
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 300},
                    {"Event Title", "event-title-300"},
                    {"Event Type", "event-type-300"},
                    {"Event Start Date", new DateTime(2015, 4, 2, 8, 30, 0)},
                    {"Event End Date", new DateTime(2015, 4, 2, 8, 30, 0)}
                }
                ,
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 400},
                    {"Event Title", "event-title-400"},
                    {"Event Type", "event-type-400"},
                    {"Event Start Date", new DateTime(2015, 4, 30, 8, 30, 0)},
                    {"Event End Date", new DateTime(2015, 4, 30, 8, 30, 0)}
                }
                ,
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 500},
                    {"Event Title", "event-title-500"},
                    {"Event Type", "event-type-500"},
                    {"Event Start Date", new DateTime(2015, 5, 1, 8, 30, 0)},
                    {"Event End Date", new DateTime(2015, 5, 1, 8, 30, 0)}
                }
            };
        }

        private List<Dictionary<string, object>> MockEventsDictionary()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 100},
                    {"Event_Title", "event-title-100"},
                    {"Event_Type", "event-type-100"},
                    {"Event_Start_Date", new DateTime(2015, 3, 28, 8, 30, 0)},
                    {"Event_End_Date", new DateTime(2015, 3, 28, 8, 30, 0)}
                },
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 200},
                    {"Event_Title", "event-title-200"},
                    {"Event_Type", "event-type-200"},
                    {"Event_Start_Date", new DateTime(2015, 3, 28, 8, 30, 0)},
                    {"Event_End_Date", new DateTime(2015, 3, 28, 8, 30, 0)}
                },
                new Dictionary<string, object>
                {
                    {"dp_RecordID", 300},
                    {"Event_Title", "event-title-300"},
                    {"Event_Type", "event-type-300"},
                    {"Event_Start_Date", new DateTime(2015, 3, 28, 8, 30, 0)},
                    {"Event_End_Date", new DateTime(2015, 3, 28, 8, 30, 0)}
                }
            };
        }

        [Test]
        public void GetEventParticipant()
        {
            const int eventId = 1234;
            const int participantId = 5678;
            const string pageKey = "EventParticipantByEventIdAndParticipantId";
            var mockEventParticipants = MockEventParticipantsByEventIdAndParticipantId();

            ministryPlatformService.Setup(m => m.GetPageViewRecords(pageKey, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(mockEventParticipants);

            var participant = fixture.GetEventParticipantRecordId(eventId, participantId);
            
            ministryPlatformService.VerifyAll();
            Assert.IsNotNull(participant);
            Assert.AreEqual(8634, participant);
        }

        private List<Dictionary<string, object>> MockEventParticipantsByEventIdAndParticipantId()
        {
            return new List<Dictionary<string, object>>{
                new Dictionary<string, object>
                {
                    {"Event_Participant_ID", 8634},
                    {"Event_ID", 93},
                    {"Participant_ID", 134}
                }    
            };
        }
    }
}