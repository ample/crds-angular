﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using crds_angular.Exceptions.Models;
using crds_angular.Security;
using crds_angular.Services.Interfaces;
using crds_angular.test.controllers;
using MinistryPlatform.Translation.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class DonorController : MPAuth
    {
        private IDonorService donorService;
        private IPaymentService stripeService;
        private IAuthenticationService authenticationService;

        public DonorController(IDonorService donorService, IPaymentService stripeService,
            IAuthenticationService authenticationService)
        {
            this.donorService = donorService;
            this.stripeService = stripeService;
            this.authenticationService = authenticationService;

        }

        [ResponseType(typeof (DonorDTO))]
        [Route("api/donor")]
        public IHttpActionResult Post([FromBody] CreateDonorDTO dto)
        {
            return Authorized(token =>
            {
                try
                {
                    var contactId = authenticationService.GetContactId(token);

                    var customerId = stripeService.createCustomer(dto.stripe_token_id);

                    var donorId = donorService.CreateDonorRecord(contactId, customerId);

                    var response = new DonorDTO
                    {
                        id = donorId.ToString(),
                        stripe_customer_id = customerId
                    };

                    return Ok(response);
                }
                catch (Exception exception)
                {
                    var apiError = new ApiErrorDto("Donor Post Failed", exception);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });


        }

        [ResponseType(typeof (DonorDTO))]
        [Route("api/donation")]
        public IHttpActionResult Post([FromBody] CreateDonationDTO dto)
        {
            return Authorized(token =>
            {
                var donationId = donorService.CreateDonationRecord(dto.donationAmt, dto.donorId);

                var donationDistributionId = donorService.CreateDonationDistributionRecord(donationId, dto.donationAmt,
                    dto.programId);

                var response =  "you did it";                                                            
                return Ok(response);
            });

        }
    }
}
