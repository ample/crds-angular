﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Crossroads.Utilities;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services;
using MinistryPlatform.Translation.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Services
{
    [TestFixture]
    public class DonorServiceTest
    {
        private Mock<IMinistryPlatformService> _ministryPlatformService;
        private Mock<IProgramService> _programService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<IAuthenticationService> _authService;
        private Mock<IConfigurationWrapper> _configuration;
        private Mock<ICryptoProvider> _crypto;

        private DonorService _fixture;

        [SetUp]
        public void SetUp()
        {
            _ministryPlatformService = new Mock<IMinistryPlatformService>();
            _programService = new Mock<IProgramService>();
            _communicationService = new Mock<ICommunicationService>();
            _authService = new Mock<IAuthenticationService>();
            _crypto = new Mock<ICryptoProvider>();
            _configuration = new Mock<IConfigurationWrapper>();
            _configuration.Setup(mocked => mocked.GetConfigIntValue("Donors")).Returns(299);
            _configuration.Setup(mocked => mocked.GetConfigIntValue("Donations")).Returns(297);
            _configuration.Setup(mocked => mocked.GetConfigIntValue("Distributions")).Returns(296);
            _configuration.Setup(mocked => mocked.GetConfigIntValue("DonorAccounts")).Returns(298);
            _configuration.Setup(mocked => mocked.GetConfigIntValue("FindDonorByAccountPageView")).Returns(2015);
            _configuration.Setup(m => m.GetEnvironmentVarAsString("API_USER")).Returns("uid");
            _configuration.Setup(m => m.GetEnvironmentVarAsString("API_PASSWORD")).Returns("pwd");

            _authService.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(new Dictionary<string, object> { { "token", "ABC" }, { "exp", "123" } });

            _fixture = new DonorService(_ministryPlatformService.Object, _programService.Object, _communicationService.Object, _authService.Object, _configuration.Object, _crypto.Object);
        }

        [Test]
        public void CreateDonorRecordTest()
        {
            var donorPageId = Convert.ToInt32(ConfigurationManager.AppSettings["Donors"]);
            var expectedDonorId = 585858;
            var setupDate = DateTime.Now;

            var expectedValues = new Dictionary<string, object>
            {
                {"Contact_ID", 888888},
                {"Statement_Frequency_ID", 1},//default to quarterly
                {"Statement_Type_ID", 1},     //default to individual
                {"Statement_Method_ID", 2},   //default to email/online
                {"Setup_Date", setupDate},    //default to current date/time
                {"Processor_ID", "cus_crds123456"}
            };

            var donorAccount = new DonorAccount
            {
                AccountNumber = "123",
                RoutingNumber = "456"
            };

            var acctBytes = Encoding.UTF8.GetBytes("acctNum");
            var rtnBytes = Encoding.UTF8.GetBytes("rtn");
            var expectedEncAcct = Convert.ToBase64String(acctBytes.Concat(rtnBytes).ToArray());

            _crypto.Setup(mocked => mocked.EncryptValue(donorAccount.AccountNumber)).Returns(acctBytes);
            _crypto.Setup(mocked => mocked.EncryptValue(donorAccount.RoutingNumber)).Returns(rtnBytes);

           _ministryPlatformService.Setup(mocked => mocked.CreateRecord(
              It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(),
              It.IsAny<string>(), true)).Returns(expectedDonorId);

           var expectedAcctValues = new Dictionary<string, object>
            {
                {"Institution_Name", "Bank"},
                {"Account_Number", "0"},
                {"Routing_Number", "0"},
                {"Encrypted_Account", expectedEncAcct},
                {"Donor_ID", expectedDonorId},
                {"Non-Assignable", false},
                {"Account_Type_ID", (int)donorAccount.Type},
                {"Closed", false}
            };

           _ministryPlatformService.Setup(mocked => mocked.CreateRecord(298, expectedAcctValues, It.IsAny<string>(), false)).Returns(102030);

           var response = _fixture.CreateDonorRecord(888888, "cus_crds123456", setupDate, 1, 1, 2, donorAccount);

           _ministryPlatformService.Verify(mocked => mocked.CreateRecord(donorPageId, expectedValues, It.IsAny<string>(), true));
           _ministryPlatformService.VerifyAll();

           _crypto.VerifyAll();

            Assert.AreEqual(response, expectedDonorId);
        }

        [Test]
        public void CreateDonorRecordWithNonDefaultValuesTest()
        {
            var donorPageId = Convert.ToInt32(ConfigurationManager.AppSettings["Donors"]);
            var expectedDonorId = 585858;
            var setupDate = DateTime.Now;

            var expectedValues = new Dictionary<string, object>
            {
                {"Contact_ID", 888888},
                {"Statement_Frequency_ID", 5},//default to quarterly
                {"Statement_Type_ID", 6},     //default to individual
                {"Statement_Method_ID", 7},   //default to email/online
                {"Setup_Date", setupDate},    //default to current date/time
                {"Processor_ID", "cus_crds123456"}    
            };

            _ministryPlatformService.Setup(mocked => mocked.CreateRecord(
               It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(),
               It.IsAny<string>(), true)).Returns(expectedDonorId);

            var response = _fixture.CreateDonorRecord(888888, "cus_crds123456", setupDate, 5, 6, 7);

            _ministryPlatformService.Verify(mocked => mocked.CreateRecord(donorPageId, expectedValues, It.IsAny<string>(), true));

            Assert.AreEqual(response, expectedDonorId);

        }

        [Test]
        public void CreateDonationAndDistributionRecord()
        {
            var donationAmt = 676767;
            var feeAmt = 5656;
            var donorId = 1234567;
            var programId = "3";
            var setupDate = DateTime.Now;
            var chargeId = "ch_crds1234567";
            var processorId = "cus_8675309";
            var pymtType = "cc";
            var expectedDonationId = 321321;
            var expectedDonationDistributionId = 231231;
            var checkScannerBatchName = "check scanner batch";
            const string viewKey = "DonorByContactId";
            const string sortString = "";
            var searchString = "," + donorId;
            var donationPageId = Convert.ToInt32(ConfigurationManager.AppSettings["Donations"]);
            var donationDistPageId = Convert.ToInt32(ConfigurationManager.AppSettings["Distributions"]);
           
            _ministryPlatformService.Setup(mocked => mocked.CreateRecord(
              donationPageId, It.IsAny<Dictionary<string, object>>(),
              It.IsAny<string>(), true)).Returns(expectedDonationId);

            _ministryPlatformService.Setup(mocked => mocked.CreateRecord(
                donationDistPageId, It.IsAny<Dictionary<string, object>>(),
                It.IsAny<string>(), true)).Returns(expectedDonationDistributionId);

            _communicationService.Setup(mocked => mocked.SendMessage(It.IsAny<Communication>()));

            var expectedDonationValues = new Dictionary<string, object>
            {
                {"Donor_ID", donorId},
                {"Donation_Amount", donationAmt},
                {"Processor_Fee_Amount", feeAmt /Constants.StripeDecimalConversionValue},
                {"Payment_Type_ID", 4}, //hardcoded as credit card until ACH stories are worked
                {"Donation_Date", setupDate},
                {"Transaction_code", chargeId},
                {"Registered_Donor", true}, 
                {"Processor_ID", processorId},
                {"Donation_Status_Date", setupDate},
                {"Donation_Status_ID", 1},
                {"Check_Scanner_Batch", checkScannerBatchName}
            };
            
            var expectedDistributionValues = new Dictionary<string, object>
            {
                {"Donation_ID", expectedDonationId},
                {"Amount", donationAmt},
                {"Program_ID", programId}
            };

            var programServiceResponse = new Program
            {
                CommunicationTemplateId = 1234,
                ProgramId = 3,
                Name = "Crossroads"
            };

            _programService.Setup(mocked => mocked.GetProgramById(It.IsAny<int>())).Returns(programServiceResponse);

            var dictList = new List<Dictionary<string, object>>();
            dictList.Add(new Dictionary<string, object>()
            {
                {"Email","test@test.com"},
                {"Contact_ID","1234"}
            });


            _ministryPlatformService.Setup(mocked => mocked.GetPageViewRecords(viewKey, It.IsAny<string>(), searchString, sortString, 0)).Returns(dictList);

            var getTemplateResponse = new MessageTemplate()
            {
                Body = "Test Body Content",
                Subject = "Test Email Subject Line"
            };
            _communicationService.Setup(mocked => mocked.GetTemplate(It.IsAny<int>())).Returns(getTemplateResponse);


            var response = _fixture.CreateDonationAndDistributionRecord(donationAmt, feeAmt, donorId, programId, chargeId, pymtType, processorId, setupDate, true, checkScannerBatchName);

            // Explicitly verify each expectation...
            _communicationService.Verify(mocked => mocked.SendMessage(It.IsAny<Communication>()));
            _programService.Verify(mocked => mocked.GetProgramById(3));
            _ministryPlatformService.Verify(mocked => mocked.CreateRecord(donationPageId, expectedDonationValues, It.IsAny<string>(), true));
            _ministryPlatformService.Verify(mocked => mocked.CreateRecord(donationDistPageId, expectedDistributionValues, It.IsAny<string>(), true));

            _ministryPlatformService.VerifyAll();
            _programService.VerifyAll();
            _communicationService.VerifyAll();
            Assert.IsNotNull(response);
            Assert.AreEqual(response, expectedDonationId);
        }

        [Test]
        public void ShouldUpdatePaymentProcessorCustomerId()
        {
            _ministryPlatformService.Setup(mocked => mocked.UpdateRecord(299, It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()));

            var response = _fixture.UpdatePaymentProcessorCustomerId(123, "456");

            _ministryPlatformService.Verify(mocked => mocked.UpdateRecord(
                299,
                It.Is<Dictionary<string, object>>(
                    d => ((int)d["Donor_ID"]) == 123
                        && ((string)d[DonorService.DonorProcessorId]).Equals("456")),
                It.IsAny<string>()));
            Assert.AreEqual(123, response);
        }

        [Test]
        public void ShouldThrowApplicationExceptionWhenMinistryPlatformUpdateFails()
        {
            var ex = new Exception("Oh no!!!");
            _ministryPlatformService.Setup(mocked => mocked.UpdateRecord(299, It.IsAny<Dictionary<string, object>>(), It.IsAny<string>())).Throws(ex);

            try
            {
                _fixture.UpdatePaymentProcessorCustomerId(123, "456");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOf(typeof(ApplicationException), e);
                Assert.AreSame(ex, e.InnerException);
            }
        }

        [Test]
        public void TestGetPossibleGuestDonorContact()
        {
            var donorId = 1234567;
            var processorId = "cus_431234";
            var contactId = 565656;
            var email = "cross@crossroads.net";
            var guestDonorPageViewId = "PossibleGuestDonorContact";
            var expectedDonorValues = new List<Dictionary<string, object>>();
            expectedDonorValues.Add(new Dictionary<string, object>
            {
                {"Donor_Record", donorId},
                {"Processor_ID", processorId},
                {"Contact_ID", contactId},
                {"Email_Address", email}
            });
            var donor = new ContactDonor()
            {
                DonorId = donorId,
                ProcessorId = processorId,
                ContactId = contactId,
                Email = email
            };

            _ministryPlatformService.Setup(mocked => mocked.GetPageViewRecords(
              guestDonorPageViewId, It.IsAny<string>(),
              It.IsAny<string>(),  "",0)).Returns(expectedDonorValues);

            var response = _fixture.GetPossibleGuestContactDonor(email);

            _ministryPlatformService.Verify(mocked => mocked.GetPageViewRecords(guestDonorPageViewId,It.IsAny<string>(), ","+email,"", 0));

            _ministryPlatformService.VerifyAll();
            Assert.AreEqual(response.DonorId, donor.DonorId);
            Assert.AreEqual(response.ContactId, donor.ContactId);
            Assert.AreEqual(response.Email, donor.Email);
            Assert.AreEqual(response.ProcessorId, donor.ProcessorId);
        }


        [Test]
        public void TestGetDonor()
        {
            var donorId = 1234567;
            var processorId = "cus_431234";
            var contactId = 565656;
            var email = "cross@crossroads.net";
            var guestDonorPageViewId = "DonorByContactId";
            var expectedDonorValues = new List<Dictionary<string, object>>();
            expectedDonorValues.Add(new Dictionary<string, object>
            {
                {"Donor_ID", donorId},
                {"Processor_ID", processorId},
                {"Contact_ID", contactId},
                {"Email", email}
            });
            var donor = new ContactDonor()
            {
                DonorId = donorId,
                ProcessorId = processorId,
                ContactId = contactId,
                Email = email
            };

            _ministryPlatformService.Setup(mocked => mocked.GetPageViewRecords(
                guestDonorPageViewId, It.IsAny<string>(),
                It.IsAny<string>(), "", 0)).Returns(expectedDonorValues);

            var response = _fixture.GetContactDonor(contactId);

            _ministryPlatformService.Verify(
                mocked => mocked.GetPageViewRecords(guestDonorPageViewId, It.IsAny<string>(), contactId+",", "", 0));

            _ministryPlatformService.VerifyAll();
            Assert.AreEqual(response.DonorId, donor.DonorId);
            Assert.AreEqual(response.ContactId, donor.ContactId);
            Assert.AreEqual(response.ProcessorId, donor.ProcessorId);
        }

        [Test]
        public void TestGetDonorNoExistingDonor()
        {
            var contactId = 565656;
            var guestDonorPageViewId = "DonorByContactId";

            _ministryPlatformService.Setup(mocked => mocked.GetPageViewRecords(
                guestDonorPageViewId, It.IsAny<string>(),
                It.IsAny<string>(), "", 0)).Returns((List<Dictionary<string, object>>)null);

            var response = _fixture.GetContactDonor(contactId);

            _ministryPlatformService.Verify(
                mocked => mocked.GetPageViewRecords(guestDonorPageViewId, It.IsAny<string>(), contactId + ",", "", 0));

            _ministryPlatformService.VerifyAll();
            Assert.IsNotNull(response);
            Assert.AreEqual(contactId, response.ContactId);
            Assert.AreEqual(0, response.DonorId);
            Assert.IsNull(response.ProcessorId);
        }

        [Test]
        public void TestSendEmail()
        {
            const string program = "Crossroads";
            const int declineEmailTemplate = 11940;
            var donationDate = DateTime.Now;
            const string emailReason = "rejected: lack of funds";
            const int donorId = 9876;
            const int donationAmt = 4343;
            const string paymentType = "Bank";

            var getTemplateResponse = new MessageTemplate()
            {
                Body = "Your payment was rejected.  Darn.",
                Subject = "Test Decline Email"
            };
            _communicationService.Setup(mocked => mocked.GetTemplate(It.IsAny<int>())).Returns(getTemplateResponse);

            _fixture.SendEmail(declineEmailTemplate, donorId, donationAmt, paymentType, donationDate, program,
                emailReason);

            _ministryPlatformService.VerifyAll();
            _communicationService.VerifyAll();
 
        }

        [Test]
        public void TestGetContactDonorForDonorAccount()
        {
            const int donorId = 1234567;
            const string processorId = "cus_431234";
            const int contactId = 565656;
            const string email = "cross@crossroads.net";
            const string guestDonorPageViewId = "DonorByContactId";

            var acctBytes = Encoding.UTF8.GetBytes("acctNum");
            var rtnBytes = Encoding.UTF8.GetBytes("rtn");
            var expectedEncAcct = Convert.ToBase64String(acctBytes.Concat(rtnBytes).ToArray());

            _crypto.Setup(mocked => mocked.EncryptValue("123")).Returns(acctBytes);
            _crypto.Setup(mocked => mocked.EncryptValue("456")).Returns(rtnBytes);

            var queryResults = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "Contact_ID", contactId }
                }
            };

            _ministryPlatformService.Setup(mocked => mocked.GetPageViewRecords(2015, It.IsAny<string>(), "," + expectedEncAcct, "", 0)).Returns(queryResults);

            var expectedDonorValues = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"Donor_ID", donorId},
                    {"Processor_ID", processorId},
                    {"Contact_ID", contactId},
                    {"Email", email}
                }
            };
            var donor = new ContactDonor()
            {
                DonorId = donorId,
                ProcessorId = processorId,
                ContactId = contactId,
                Email = email
            };

            _ministryPlatformService.Setup(mocked => mocked.GetPageViewRecords(
                guestDonorPageViewId, It.IsAny<string>(),
                contactId+",", "", 0)).Returns(expectedDonorValues);

            var result = _fixture.GetContactDonorForDonorAccount("123", "456");
            _ministryPlatformService.VerifyAll();
            _crypto.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.DonorId, donor.DonorId);
            Assert.AreEqual(result.ContactId, donor.ContactId);
            Assert.AreEqual(result.ProcessorId, donor.ProcessorId);
        }

    }
}
