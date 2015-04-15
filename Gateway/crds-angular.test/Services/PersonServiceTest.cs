using System;
using System.Collections.Generic;
using crds_angular.App_Start;
using crds_angular.Models.Crossroads.Serve;
using crds_angular.Services;
using crds_angular.Services.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace crds_angular.test.Services
{
    internal class PersonServiceTest
    {
        private Mock<IContactRelationshipService> _contactRelationshipService;
        private Mock<IGroupService> _groupService;
        private Mock<IContactService> _contactService;
        private Mock<IOpportunityService> _opportunityService;
        private Mock<IAuthenticationService> _authenticationService;
        private Mock<IPersonService> _personService;

        private PersonService _fixture;

        [SetUp]
        public void SetUp()
        {
            _contactRelationshipService = new Mock<IContactRelationshipService>();
            _groupService = new Mock<IGroupService>();
            _contactService = new Mock<IContactService>();
            _opportunityService = new Mock<IOpportunityService>();
            _authenticationService = new Mock<IAuthenticationService>();
            _personService = new Mock<IPersonService>();

            _authenticationService.Setup(mocked => mocked.GetContactId(It.IsAny<string>())).Returns(123456);
            var myContact = new MyContact
            {
                Contact_ID = 123456,
                Email_Address = "contact@email.com",
                Last_Name = "last-name",
                Nickname = "nickname",
                First_Name = "first-name",
                Middle_Name = "middle-name",
                Maiden_Name = "maiden-name",
                Mobile_Phone = "mobile-phone",
                Mobile_Carrier = 999,
                Date_Of_Birth = "date-of-birth",
                Marital_Status_ID = 5,
                Gender_ID = 2,
                Employer_Name = "employer-name",
                Address_Line_1 = "address-line-1",
                Address_Line_2 = "address-line-2",
                City = "city",
                State = "state",
                Postal_Code = "postal-code",
                Anniversary_Date = "anniversary-date",
                Foreign_Country = "foreign-country",
                Home_Phone = "home-phone",
                Congregation_ID = 8,
                Household_ID = 7,
                Address_ID = 6
            };
            _contactService.Setup(mocked => mocked.GetMyProfile(It.IsAny<string>())).Returns(myContact);

            _fixture = new PersonService( _contactService.Object);

            //force AutoMapper to register
            AutoMapperConfig.RegisterMappings();
        }

        [Test]
        public void GetLoggedInUserProfileTest()
        {
            const string token = "some-string";


            var person = _fixture.GetLoggedInUserProfile(token);

            Assert.IsNotNull(person);

            Assert.AreEqual(123456, person.ContactId);
            Assert.AreEqual("contact@email.com", person.EmailAddress);
            Assert.AreEqual("nickname", person.NickName);
            Assert.AreEqual("first-name", person.FirstName);
            Assert.AreEqual("middle-name", person.MiddleName);
            Assert.AreEqual("last-name", person.LastName);
            Assert.AreEqual("maiden-name", person.MaidenName);
            Assert.AreEqual("mobile-phone", person.MobilePhone);
            Assert.AreEqual(999, person.MobileCarrierId);
            Assert.AreEqual("date-of-birth", person.DateOfBirth);
            Assert.AreEqual(5, person.MaritalStatusId);
            Assert.AreEqual(2, person.GenderId);
            Assert.AreEqual("employer-name", person.EmployerName);
            Assert.AreEqual("address-line-1", person.AddressLine1);
            Assert.AreEqual("address-line-2", person.AddressLine2);
            Assert.AreEqual("city", person.City);
            Assert.AreEqual("state", person.State);
            Assert.AreEqual("postal-code", person.PostalCode);
            Assert.AreEqual("anniversary-date", person.AnniversaryDate);
            Assert.AreEqual("foreign-country", person.ForeignCountry);
            Assert.AreEqual("home-phone", person.HomePhone);
            Assert.AreEqual(8, person.CongregationId);
            Assert.AreEqual(7, person.HouseholdId);
            Assert.AreEqual(6, person.AddressId);
        }

        
    }
}