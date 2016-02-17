﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using crds_angular.Models.Crossroads.Subscription;
using crds_angular.Models.MailChimp;
using crds_angular.Services;
using crds_angular.Services.Interfaces;
using crds_angular.Util;
using Crossroads.Utilities.Interfaces;
using log4net;
using MinistryPlatform.Models;
using MPInterfaces = MinistryPlatform.Translation.Services.Interfaces;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace crds_angular.test.Services
{
    internal class ListHandlerTest
    {
        //private ILoginService _loginService;

        //private Mock<ILog> _logger;

        //private Mock<MPInterfaces.IAuthenticationService> _authenticationService;
        //private Mock<IConfigurationWrapper> _configurationWrapper;
        //private Mock<MPInterfaces.IContactService> _contactService;
        //private Mock<IEmailCommunication> _emailCommunication;
        //private Mock<MPInterfaces.IUserService> _userService;

        //private readonly ILog _logger = LogManager.GetLogger(typeof(MailchimpListHandler));
        //private readonly IConfigurationWrapper _configWrapper;
        //private readonly MPInterfaces.IBulkEmailRepository _bulkEmailRepository;
        //private readonly IRestClient _restClient;

        private Mock<ILog> _logger;
        private Mock<IConfigurationWrapper> _configurationWrapper;
        private Mock<MPInterfaces.IBulkEmailRepository> _bulkEmailRepository;
        private Mock<IRestClient> _restClient;
        private MailchimpListHandler _fixture;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILog>();
            _configurationWrapper = new Mock<IConfigurationWrapper>();
            _bulkEmailRepository = new Mock<MPInterfaces.IBulkEmailRepository>();
            _restClient = new Mock<IRestClient>();

            _bulkEmailRepository.Setup(mocked => mocked.GetPublications(It.IsAny<string>())).Returns(GetPublications());

            _fixture = new MailchimpListHandler(_configurationWrapper.Object, _bulkEmailRepository.Object, _restClient.Object);
        }

        [Test]
        public void ShouldHandleResubscribe()
        {
            string email = "test@test.com";
            string status = "unsubscribed";

            string listName = "test";
            string token = "testToken";

            string responseContent = "{ \"id\" : \"12345678\", \"email_address\" : \"" + email + "\", \"status\" : \"" + status + "\" }";

            OptInResponse newResponse = new OptInResponse
            {
                ErrorInSignupProcess = false,
                UserAlreadySubscribed = false
            };

            var subStatusResponse = new Mock<IRestResponse<BulkEmailSubscriberOptDTO>>(MockBehavior.Strict);
            subStatusResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            subStatusResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            subStatusResponse.SetupGet(mocked => mocked.Content).Returns(responseContent).Verifiable();

            var subSubmitResponse = new Mock<IRestResponse<BulkEmailSubscriberOptDTO>>(MockBehavior.Strict);
            subSubmitResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            subSubmitResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();

            RestRequest getRequest = new RestRequest("test", Method.GET);
            _restClient.Setup(mocked => mocked.Execute(It.Is<IRestRequest>(r => r.Method == Method.GET))).Returns(subStatusResponse.Object);
            _restClient.Setup(mocked => mocked.Execute(It.Is<IRestRequest>(r => r.Method == Method.PUT))).Returns(subSubmitResponse.Object);

            var response = _fixture.AddListSubscriber(email, listName, token);

            Assert.AreEqual(newResponse.UserAlreadySubscribed, response.UserAlreadySubscribed);
            Assert.AreEqual(newResponse.ErrorInSignupProcess, response.ErrorInSignupProcess);
        }

        [Test]
        public void ShouldHandleAlreadySubscribed()
        {
            string email = "test@test.com";
            string status = "subscribed";

            string listName = "test";
            string token = "testToken";

            string responseContent = "{ \"id\" : \"12345678\", \"email_address\" : \"" + email + "\", \"status\" : \"" + status + "\" }";

            OptInResponse newResponse = new OptInResponse
            {
                ErrorInSignupProcess = false,
                UserAlreadySubscribed = true
            };

            var subStatusResponse = new Mock<IRestResponse<BulkEmailSubscriberOptDTO>>(MockBehavior.Strict);
            subStatusResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            subStatusResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            subStatusResponse.SetupGet(mocked => mocked.Content).Returns(responseContent).Verifiable();

            var subSubmitResponse = new Mock<IRestResponse<BulkEmailSubscriberOptDTO>>(MockBehavior.Strict);
            subSubmitResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            subSubmitResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();

            RestRequest getRequest = new RestRequest("test", Method.GET);
            _restClient.Setup(mocked => mocked.Execute(It.Is<IRestRequest>(r => r.Method == Method.GET))).Returns(subStatusResponse.Object);
            _restClient.Setup(mocked => mocked.Execute(It.Is<IRestRequest>(r => r.Method == Method.PUT))).Returns(subSubmitResponse.Object);

            var response = _fixture.AddListSubscriber(email, listName, token);

            Assert.AreEqual(newResponse.UserAlreadySubscribed, response.UserAlreadySubscribed);
            Assert.AreEqual(newResponse.ErrorInSignupProcess, response.ErrorInSignupProcess);
        }

        [Test]
        public void ShouldHandleNewSubscribe()
        {
            string email = "test@test.com";
            string status = "unsubscribed";

            string listName = "test";
            string token = "testToken";

            OptInResponse newResponse = new OptInResponse
            {
                ErrorInSignupProcess = false,
                UserAlreadySubscribed = false
            };

            var subStatusResponse = new Mock<IRestResponse<BulkEmailSubscriberOptDTO>>(MockBehavior.Strict);
            subStatusResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            subStatusResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.NotFound).Verifiable();

            var subSubmitResponse = new Mock<IRestResponse<BulkEmailSubscriberOptDTO>>(MockBehavior.Strict);
            subSubmitResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            subSubmitResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();

            RestRequest getRequest = new RestRequest("test", Method.GET);
            _restClient.Setup(mocked => mocked.Execute(It.Is<IRestRequest>(r => r.Method == Method.GET))).Returns(subStatusResponse.Object);
            _restClient.Setup(mocked => mocked.Execute(It.Is<IRestRequest>(r => r.Method == Method.PUT))).Returns(subSubmitResponse.Object);

            var response = _fixture.AddListSubscriber(email, listName, token);

            Assert.AreEqual(newResponse.UserAlreadySubscribed, response.UserAlreadySubscribed);
            Assert.AreEqual(newResponse.ErrorInSignupProcess, response.ErrorInSignupProcess);
        }

        private List<BulkEmailPublication> GetPublications()
        {
            List<BulkEmailPublication> publications = new List<BulkEmailPublication>();
            publications.Add(new BulkEmailPublication
            {
                PublicationId = 1,
                Title = "test",
                Description = "test",
                ThirdPartyPublicationId = "12345678",
                LastSuccessfulSync = System.DateTime.Now
            });

            return publications;
        }
    }
}
