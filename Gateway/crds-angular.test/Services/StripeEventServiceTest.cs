﻿using System;
using System.Collections.Generic;
using System.Linq;
using crds_angular.Models.Crossroads.Stewardship;
using crds_angular.Services;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities;
using Crossroads.Utilities.Interfaces;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp.Extensions;

namespace crds_angular.test.Services
{
    public class StripeEventServiceTest
    {
        private StripeEventService _fixture;
        private Mock<IPaymentService> _paymentService;
        private Mock<IDonationService> _donationService;

        [SetUp]
        public void SetUp()
        {
            var configuration = new Mock<IConfigurationWrapper>();
            configuration.Setup(mocked => mocked.GetConfigIntValue("DonationStatusDeposited")).Returns(999);
            configuration.Setup(mocked => mocked.GetConfigIntValue("DonationStatusSucceeded")).Returns(888);
            configuration.Setup(mocked => mocked.GetConfigIntValue("DonationStatusDeclined")).Returns(777);
            configuration.Setup(mocked => mocked.GetConfigIntValue("BatchEntryTypePaymentProcessor")).Returns(555);

            _paymentService = new Mock<IPaymentService>(MockBehavior.Strict);
            _donationService = new Mock<IDonationService>(MockBehavior.Strict);
            _fixture = new StripeEventService(_paymentService.Object, _donationService.Object, configuration.Object);
        }

        [Test]
        public void TestProcessStripeEventNoMatchingEventHandler()
        {
            var e = new StripeEvent
            {
                LiveMode = true,
                Type = "not.this.one"
            };

            Assert.IsNull(_fixture.ProcessStripeEvent(e));
            _paymentService.VerifyAll();
            _donationService.VerifyAll();
        }

        [Test]
        public void TestChargeSucceeded()
        {
            var e = new StripeEvent
            {
                LiveMode = true,
                Type = "charge.succeeded",
                Created = DateTime.Now.AddDays(-1),
                Data = new StripeEventData
                {
                    Object = JObject.FromObject(new StripeCharge
                    {
                        Id = "9876"
                    })
                }
            };

            _donationService.Setup(mocked => mocked.UpdateDonationStatus("9876", 888, e.Created, null)).Returns(123);
            Assert.IsNull(_fixture.ProcessStripeEvent(e));
            _paymentService.VerifyAll();
            _donationService.VerifyAll();
        }

        [Test]
        public void TestChargeFailed()
        {
            var e = new StripeEvent
            {
                LiveMode = true,
                Type = "charge.failed",
                Created = DateTime.Now.AddDays(-1),
                Data = new StripeEventData
                {
                    Object = JObject.FromObject(new StripeCharge
                    {
                        Id = "9876",
                        FailureCode = "invalid_routing_number",
                        FailureMessage = "description from stripe"
                    })
                }
            };

            _donationService.Setup(mocked => mocked.UpdateDonationStatus("9876", 777, e.Created, "invalid_routing_number: description from stripe")).Returns(123);
            _donationService.Setup(mocked => mocked.ProcessDeclineEmail("9876"));
            Assert.IsNull(_fixture.ProcessStripeEvent(e));
            _paymentService.VerifyAll();
            _donationService.VerifyAll();
        }

        [Test]
        public void TestTransferPaidNoChargesFound()
        {
            var e = new StripeEvent
            {
                LiveMode = true,
                Type = "transfer.paid",
                Created = DateTime.Now.AddDays(-1),
                Data = new StripeEventData
                {
                    Object = JObject.FromObject(new StripeTransfer
                    {
                        Id = "tx9876",
                    })
                }
            };

            _donationService.Setup(mocked => mocked.GetDonationBatchByProcessorTransferId("tx9876")).Returns((DonationBatchDTO)null);
            _paymentService.Setup(mocked => mocked.GetChargesForTransfer("tx9876")).Returns(new List<StripeCharge>());
            var result = _fixture.ProcessStripeEvent(e);
            Assert.IsNotNull(_fixture.ProcessStripeEvent(e));
            Assert.IsInstanceOf<TransferPaidResponseDTO>(result);
            var tp = (TransferPaidResponseDTO)result;
            Assert.AreEqual(0, tp.TotalTransactionCount);
            Assert.AreEqual(0, tp.SuccessfulUpdates.Count);
            Assert.AreEqual(0, tp.FailedUpdates.Count);

            _paymentService.VerifyAll();
            _donationService.VerifyAll();
        }

        [Test]
        public void TestTransferPaid()
        {
            var e = new StripeEvent
            {
                LiveMode = true,
                Type = "transfer.paid",
                Created = DateTime.Now.AddDays(-1),
                Data = new StripeEventData
                {
                    Object = JObject.FromObject(new StripeTransfer
                    {
                        Id = "tx9876",
                        Amount = 50000,
                        Fee = 1500
                    })
                }
            };

            var charges = new List<StripeCharge>
            {
                new StripeCharge
                {
                    Id = "ch111",
                    Amount = 111,
                    Fee = 1
                },
                new StripeCharge
                {
                    Id = "ch222",
                    Amount = 222,
                    Fee = 2
                },
                new StripeCharge
                {
                    Id = "ch333",
                    Amount = 333,
                    Fee = 3
                },
                new StripeCharge
                {
                    Id = "ch777",
                    Amount = 777, 
                    Fee = 7
                },
                new StripeCharge
                {
                    Id = "ch444",
                    Amount = 444,
                    Fee = 4
                },
                new StripeCharge
                {
                    Id = "ch555",
                    Amount = 555, 
                    Fee = 5
                }
            };

            _donationService.Setup(mocked => mocked.GetDonationByProcessorPaymentId("ch111")).Returns(new DonationDTO
            {
                donation_id = "1111",
                batch_id = null
            });

            _donationService.Setup(mocked => mocked.GetDonationByProcessorPaymentId("ch222")).Returns(new DonationDTO
            {
                donation_id = "2222",
                batch_id = null
            });

            _donationService.Setup(mocked => mocked.GetDonationByProcessorPaymentId("ch333")).Returns(new DonationDTO
            {
                donation_id = "3333",
                batch_id = null
            });

            _donationService.Setup(mocked => mocked.GetDonationByProcessorPaymentId("ch444")).Throws(new Exception("Not gonna do it, wouldn't be prudent."));

            _donationService.Setup(mocked => mocked.GetDonationByProcessorPaymentId("ch555")).Returns(new DonationDTO
            {
                donation_id = "5555",
                batch_id = 1984
            });
            _donationService.Setup(mocked => mocked.GetDonationBatch(1984)).Returns(new DonationBatchDTO
            {
                Id = 5150,
                ProcessorTransferId = "OU812"
            });

            _donationService.Setup(mocked => mocked.GetDonationByProcessorPaymentId("ch777")).Returns(new DonationDTO
            {
                donation_id = "7777",
                batch_id = 2112
            });
            _donationService.Setup(mocked => mocked.GetDonationBatch(2112)).Returns(new DonationBatchDTO
            {
                Id = 2112,
                ProcessorTransferId = null
            });

            _donationService.Setup(mocked => mocked.GetDonationBatchByProcessorTransferId("tx9876")).Returns((DonationBatchDTO)null);
            _paymentService.Setup(mocked => mocked.GetChargesForTransfer("tx9876")).Returns(charges);
            _donationService.Setup(
                mocked => mocked.CreatePaymentProcessorEventError(e, It.IsAny<StripeEventResponseDTO>()));
            _donationService.Setup(mocked => mocked.UpdateDonationStatus(1111, 999, e.Created, null)).Returns(1111);
            _donationService.Setup(mocked => mocked.UpdateDonationStatus(2222, 999, e.Created, null)).Returns(2222);
            _donationService.Setup(mocked => mocked.UpdateDonationStatus(3333, 999, e.Created, null)).Returns(3333);
            _donationService.Setup(mocked => mocked.UpdateDonationStatus(7777, 999, e.Created, null)).Returns(7777);
            _donationService.Setup(mocked => mocked.CreateDeposit(It.IsAny<DepositDTO>())).Returns(
                (DepositDTO o) =>
                {
                    o.Id = 98765;
                    return (o);
                });
            _donationService.Setup(mocked => mocked.CreateDonationBatch(It.IsAny<DonationBatchDTO>())).Returns((DonationBatchDTO o) => o);

            var result = _fixture.ProcessStripeEvent(e);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TransferPaidResponseDTO>(result);
            var tp = (TransferPaidResponseDTO)result;
            Assert.AreEqual(6, tp.TotalTransactionCount);
            Assert.AreEqual(4, tp.SuccessfulUpdates.Count);
            Assert.AreEqual(charges.GetRange(0, 4).Select(charge => charge.Id), tp.SuccessfulUpdates);
            Assert.AreEqual(2, tp.FailedUpdates.Count);
            Assert.AreEqual("ch444", tp.FailedUpdates[0].Key);
            Assert.AreEqual("Not gonna do it, wouldn't be prudent.", tp.FailedUpdates[0].Value);
            Assert.AreEqual("ch555", tp.FailedUpdates[1].Key);
            Assert.IsNotNull(tp.Batch);
            Assert.IsNotNull(tp.Deposit);
            Assert.IsNotNull(tp.Exception);

            _donationService.Verify(mocked => mocked.CreateDonationBatch(It.Is<DonationBatchDTO>(o =>
                o.BatchName.Matches(@"MP\d{12}")
                && o.SetupDateTime == o.FinalizedDateTime
                && o.BatchEntryType == 555
                && o.ItemCount == 4
                && o.BatchTotalAmount == ((111 + 222 + 333 + 777) / Constants.StripeDecimalConversionValue)
                && o.Donations != null
                && o.Donations.Count == 4
                && o.DepositId == 98765
                && o.ProcessorTransferId.Equals("tx9876")
            )));

            _donationService.Verify(mocked => mocked.CreateDeposit(It.Is<DepositDTO>(o =>
                o.DepositName.Matches(@"MP\d{12}")
                && !o.Exported
                && o.AccountNumber.Equals(" ")
                && o.BatchCount == 1
                && o.DepositDateTime != null
                && o.DepositTotalAmount == 500M
                &&o.ProcessorFeeTotal == 15M
                &&o.DepositAmount == 485M
                && o.Notes == null
                && o.ProcessorTransferId.Equals("tx9876")
            )));

            _paymentService.VerifyAll();
            _donationService.VerifyAll();
        }
    }
}
