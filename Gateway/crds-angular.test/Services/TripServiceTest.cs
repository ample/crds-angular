﻿using System;
using System.Collections.Generic;
using System.Linq;
using crds_angular.Services;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace crds_angular.test.Services
{
    [TestFixture]
    public class TripServiceTest
    {
        private Mock<IEventParticipantService> _eventParticipantService;
        private TripService _fixture;

        [SetUp]
        public void SetUp()
        {
            _eventParticipantService = new Mock<IEventParticipantService>();
            _fixture = new TripService(_eventParticipantService.Object);
        }

        [Test]
        public void Search()
        {
            _eventParticipantService.Setup(m => m.TripParticipants(It.IsAny<string>())).Returns(MockMpSearchResponse());

            var searchResults = _fixture.Search(It.IsAny<string>());

            _eventParticipantService.VerifyAll();
            Assert.AreEqual(2, searchResults.Count);

            var p1 = searchResults.FirstOrDefault(s => s.ParticipantId == 9999);
            Assert.IsNotNull(p1);
            Assert.AreEqual(2,p1.Trips.Count);

            var p2 = searchResults.FirstOrDefault(s => s.ParticipantId == 5555);
            Assert.IsNotNull(p2);
            Assert.AreEqual(1, p2.Trips.Count);
        }

        private static List<TripParticipant> MockMpSearchResponse()
        {
            return new List<TripParticipant>
            {
                new TripParticipant
                {
                    EmailAddress = "test@aol.com",
                    EventEndDate = new DateTime(2015, 7, 1),
                    EventId = 1,
                    EventParticipantId = 7777,
                    EventStartDate = new DateTime(2015, 7, 1),
                    EventTitle = "Test Trip 1",
                    EventType = "Go Trip",
                    Lastname = "Subject",
                    Nickname = "Test",
                    ParticipantId = 9999
                },
                new TripParticipant
                {
                    EmailAddress = "test@aol.com",
                    EventEndDate = new DateTime(2015, 8, 1),
                    EventId = 2,
                    EventParticipantId = 888,
                    EventStartDate = new DateTime(2015, 8, 1),
                    EventTitle = "Test Trip 2",
                    EventType = "Go Trip",
                    Lastname = "Subject",
                    Nickname = "Test",
                    ParticipantId = 9999
                },
                new TripParticipant
                {
                    EmailAddress = "spec@aol.com",
                    EventEndDate = new DateTime(2015, 7, 1),
                    EventId = 1,
                    EventParticipantId = 4444,
                    EventStartDate = new DateTime(2015, 7, 1),
                    EventTitle = "Test Trip 1",
                    EventType = "Go Trip",
                    Lastname = "Dummy",
                    Nickname = "Crash",
                    ParticipantId = 5555
                }
            };
        }
    }
}