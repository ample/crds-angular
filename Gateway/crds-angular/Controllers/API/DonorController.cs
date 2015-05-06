﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
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
                var contactId = authenticationService.GetContactId(token);
               
                var customerId = stripeService.createCustomer(dto.stripe_token_id);

                var donorId = donorService.CreateDonorRecord(contactId, customerId);

                var response = new DonorDTO
                {
                    id = donorId.ToString(),
                    stripe_customer_id = customerId
                };

                return Ok(response);
            });


        }

    }
}
