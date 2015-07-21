﻿using System;
using crds_angular.Exceptions;
using crds_angular.Services;
using Moq;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using crds_angular.Models.Crossroads.Stewardship;
using Crossroads.Utilities.Interfaces;

namespace crds_angular.test.Services
{
    class StripeServiceTest
    {
        private Mock<IRestClient> _restClient;
        private Mock<IConfigurationWrapper> _configuration;
        private StripeService _fixture;

        [SetUp]
        public void Setup()
        {
            _restClient = new Mock<IRestClient>(MockBehavior.Strict);
            _configuration = new Mock<IConfigurationWrapper>();
            _configuration.Setup(mocked => mocked.GetConfigIntValue("MaxStripeQueryResultsPerPage")).Returns(42);

            _fixture = new StripeService(_restClient.Object, _configuration.Object);
        }

        [Test]
        public void ShouldGetChargesForTransfer()
        {
            var q = new Queue<StripeCharges>();
            q.Enqueue(new StripeCharges
            {
                HasMore = true,
                Data = new List<StripeCharge>
                {
                    new StripeCharge
                    {
                        Id = "123",
                    },
                    new StripeCharge
                    {
                        Id = "last_one_in_first_page",
                    }
                }
            });
            q.Enqueue(new StripeCharges
            {
                HasMore = false,
                Data = new List<StripeCharge>
                {
                    new StripeCharge
                    {
                        Id = "789",
                    },
                    new StripeCharge
                    {
                        Id = "90210",
                    }
                }
            });
            
            var response = new Mock<IRestResponse<StripeCharges>>();
            response.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            response.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            response.SetupGet(mocked => mocked.Data).Returns(() => (q.Dequeue())).Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCharges>(It.IsAny<IRestRequest>())).Returns(response.Object);

            var charges = _fixture.GetChargesForTransfer("tx123");
            _restClient.Verify(mocked => mocked.Execute<StripeCharges>(
                It.Is<RestRequest>(o =>
                    o.Method == Method.GET
                    && o.Resource.Equals("transfers/tx123/transactions")
                    && ParameterMatches("count", 42, o.Parameters)
            )));
            _restClient.Verify(mocked => mocked.Execute<StripeCharges>(
                It.Is<RestRequest>(o =>
                    o.Method == Method.GET
                    && o.Resource.Equals("transfers/tx123/transactions")
                    && ParameterMatches("count", 42, o.Parameters)
                    && ParameterMatches("starting_after", "last_one_in_first_page", o.Parameters)
            )));
            _restClient.VerifyAll();
            response.VerifyAll();

            Assert.IsNotNull(charges);
            Assert.AreEqual(4, charges.Count);
            Assert.AreEqual("123", charges[0].Id);
            Assert.AreEqual("last_one_in_first_page", charges[1].Id);
            Assert.AreEqual("789", charges[2].Id);
            Assert.AreEqual("90210", charges[3].Id);
        }

        [Test]
        public void ShouldGetDefaultSource()
        {
            var cust = new StripeCustomer
            {
                sources = new Sources
                {
                    data = new List<SourceData>
                    {
                        new SourceData
                        {
                            id = "456",
                            @object = "bank_account",
                            last4 = "9876",
                            routing_number = "5432",
                        },
                        new SourceData
                        {
                            id = "123",
                            @object = "bank_account",
                            last4 = "1234",
                            routing_number = "5678",
                        },
                        new SourceData
                        {
                            id = "789",
                            @object = "credit_card",
                            brand = "visa",
                            last4 = "0001",
                            exp_month = "01",
                            exp_year = "2023",
                            address_zip = "20202"
                        },
                        new SourceData
                        {
                            id = "123",
                            @object = "credit_card",
                            brand = "mcc",
                            last4 = "0002",
                            exp_month = "2",
                            exp_year = "2024",
                            address_zip = "10101"
                        },
                    }
                },
                default_source = "123",
            };
            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Data).Returns(cust).Verifiable();
            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            var defaultSource = _fixture.GetDefaultSource("token");
            Assert.IsNotNull(defaultSource);

            Assert.AreEqual("5678", defaultSource.routing_number);
            Assert.AreEqual("1234", defaultSource.bank_last4);

            Assert.AreEqual("mcc", defaultSource.brand);
            Assert.AreEqual("0002", defaultSource.last4);
            Assert.AreEqual("02", defaultSource.exp_month);
            Assert.AreEqual("24", defaultSource.exp_year);
            Assert.AreEqual("10101", defaultSource.address_zip);

            _restClient.VerifyAll();
            stripeResponse.VerifyAll();
        }

        [Test]
        public void ShouldThrowExceptionWhenTokenIsInvalid()
        {
            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.BadRequest).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Content).Returns("{error: {message:'Bad Request'}}").Verifiable();
            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            Assert.Throws<StripeException>(() => _fixture.CreateCustomer("token"));

            _restClient.Verify(mocked => mocked.Execute<StripeCustomer>(
                It.Is<RestRequest>(o =>
                    o.Method == Method.POST
                    && o.Resource.Equals("customers")
                    && ParameterMatches("description", "Crossroads Donor #pending", o.Parameters)
                    && ParameterMatches("source", "token", o.Parameters)
                    )));
            _restClient.VerifyAll();
            stripeResponse.VerifyAll();
        }

        [Test]
        public void ShouldThrowAbortExceptionWhenStripeConnectionFails()
        {
            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.BadRequest).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Content).Returns("{}").Verifiable();
            stripeResponse.SetupGet(mocked => mocked.ErrorException).Returns(new Exception("Doh!")).Verifiable();
            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            try
            {
                _fixture.CreateCustomer("token");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (StripeException e)
            {
                Assert.AreEqual("abort", e.Type);
                Assert.AreEqual("Doh!", e.DetailMessage);
                Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
            }


        }

        [Test]
        public void ShouldReturnSuccessfulCustomerId()
        {
            var customer = new StripeCustomer
            {
                id = "12345"
            };

            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Data).Returns(customer).Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            var response = _fixture.CreateCustomer("token");
            _restClient.Verify(mocked => mocked.Execute<StripeCustomer>(
                It.Is<IRestRequest>(o =>
                    o.Method == Method.POST
                    && o.Resource.Equals("customers")
                    && ParameterMatches("description", "Crossroads Donor #pending", o.Parameters)
                    && ParameterMatches("source", "token", o.Parameters)
                    )));
            _restClient.VerifyAll();
            stripeResponse.VerifyAll();

            Assert.AreEqual("12345", response);
        }

        [Test]
        public void ShouldUpdateCustomerDescription()
        {
            var customer = new StripeCustomer
            {
                id = "12345"
            };

            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Data).Returns(customer).Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            var response = _fixture.UpdateCustomerDescription("token", 102030);
            _restClient.Verify(mocked => mocked.Execute<StripeCustomer>(
                It.Is<IRestRequest>(o =>
                    o.Method == Method.POST
                    && o.Resource.Equals("customers/token")
                    && ParameterMatches("description", "Crossroads Donor #102030", o.Parameters)
                    )));
            _restClient.VerifyAll();
            stripeResponse.VerifyAll();

            Assert.AreEqual("12345", response);
        }

        [Test]
        public void ShouldThrowExceptionWhenCustomerUpdateFails()
        {
            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.BadRequest).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Content).Returns("{error: {message:'Invalid Request'}}").Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            try
            {
                _fixture.UpdateCustomerDescription("token", 102030);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (StripeException e)
            {
                Assert.AreEqual("Customer update failed", e.Message);
                Assert.IsNotNull(e.DetailMessage);
                Assert.AreEqual("Invalid Request", e.DetailMessage);
            }
        }

        [Test]
        public void ShouldChargeCustomer()
        {
            var charge = new StripeCharge
            {
                Id = "90210",
                BalanceTransaction = new StripeBalanceTransaction
                {
                    Id = "txn_123",
                    Fee = 145
                }
            };
            

            var stripeResponse = new Mock<IRestResponse<StripeCharge>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Data).Returns(charge).Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCharge>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            var response = _fixture.ChargeCustomer("cust_token", 9090, 98765, "cc");

            _restClient.Verify(mocked => mocked.Execute<StripeCharge>(
                It.Is<IRestRequest>(o =>
                    o.Method == Method.POST
                    && o.Resource.Equals("charges")
                    && ParameterMatches("amount", 9090 * 100, o.Parameters)
                    && ParameterMatches("currency", "usd", o.Parameters)
                    && ParameterMatches("customer", "cust_token", o.Parameters)
                    && ParameterMatches("description", "Donor ID #98765", o.Parameters)
                    && ParameterMatches("expand[]", "balance_transaction", o.Parameters)
                    )));

            _restClient.VerifyAll();
            stripeResponse.VerifyAll();

            Assert.AreSame(charge, response);
        }

        private bool ParameterMatches(string name, object value, List<Parameter> parms)
        {
            return(parms.Find(p => p.Name.Equals(name) && p.Value.Equals(value)) != null);
        }
        
        [Test]
        public void ShouldNotChargeCustomerIfAmountIsInvalid()
        {
            var customer = new StripeCustomer
            {
                id = "12345",
                default_source = "some card"
            };
            
            var getCustomerResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);

            getCustomerResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            getCustomerResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            getCustomerResponse.SetupGet(mocked => mocked.Data).Returns(customer).Verifiable();
            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(getCustomerResponse.Object);


            var chargeResponse = new Mock<IRestResponse<StripeCharge>>(MockBehavior.Strict);
            chargeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            chargeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.BadRequest).Verifiable();
            chargeResponse.SetupGet(mocked => mocked.Content).Returns("{error: {message:'Invalid Integer Amount'}}").Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCharge>(It.IsAny<IRestRequest>())).Returns(chargeResponse.Object);
            try
            {
                _fixture.ChargeCustomer("token", -900, 98765, "cc");
                Assert.Fail("Should have thrown exception");
            }
            catch (StripeException e)
            {
                Assert.AreEqual("Invalid charge request", e.Message);
                Assert.IsNotNull(e.DetailMessage);
                Assert.AreEqual("Invalid Integer Amount", e.DetailMessage);
            }

        }

        [Test]
        public void ShouldUpdateCustomerSource()
        {
            var customer = new StripeCustomer
            {
                id = "cus_test0618",
                default_source = "platinum card",
                sources = new Sources()
                {
                    data = new List<SourceData>()
                    {
                        new SourceData()
                        {
                            last4 = "8585",
                            brand = "Visa",
                            address_zip = "45454",
                            id = "platinum card",
                            exp_month = "01",
                            exp_year = "2020"
                        }
                    }
                }
            };


            var stripeResponse = new Mock<IRestResponse<StripeCustomer>>(MockBehavior.Strict);
            stripeResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            stripeResponse.SetupGet(mocked => mocked.Data).Returns(customer).Verifiable();

            _restClient.Setup(mocked => mocked.Execute<StripeCustomer>(It.IsAny<IRestRequest>())).Returns(stripeResponse.Object);

            var defaultSource = _fixture.UpdateCustomerSource("customerToken", "cardToken");
            _restClient.Verify(mocked => mocked.Execute<StripeCustomer>(
                It.Is<IRestRequest>(o =>
                    o.Method == Method.POST
                    && o.Resource.Equals("customers/customerToken")
                    && ParameterMatches("source", "cardToken",o.Parameters)
                    )));
            _restClient.VerifyAll();
            stripeResponse.VerifyAll();
          
            Assert.AreEqual("Visa", defaultSource.brand);
            Assert.AreEqual("8585", defaultSource.last4);
            Assert.AreEqual("45454", defaultSource.address_zip);
        }

    }

}
