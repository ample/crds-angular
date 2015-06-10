﻿using crds_angular.Exceptions.Models;
using crds_angular.Models.Crossroads;
using crds_angular.Security;
using crds_angular.Services.Interfaces;
using crds_angular.test.controllers;
using log4net;
using MinistryPlatform.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Exceptions;

namespace crds_angular.Controllers.API
{
    public class DonorController : MPAuth
    {
        private IDonorService gatewayDonorService;
        private IPaymentService stripePaymentService;

        private static readonly ILog logger = LogManager.GetLogger(typeof(DonorController));

        public DonorController(IDonorService gatewayDonorService, IPaymentService stripePaymentService)
        {
            this.gatewayDonorService = gatewayDonorService;
            this.stripePaymentService = stripePaymentService;
        }

        [ResponseType(typeof(DonorDTO))]
        [Route("api/donor/{email?}")]
        public IHttpActionResult Get(string email="")
        {
            return (Authorized(token => GetDonorForAuthenticatedUser(token), () => GetDonorForUnauthenticatedUser(email)));
        }

        [ResponseType(typeof (DonorDTO))]
        [Route("api/donor")]
        public IHttpActionResult Post([FromBody] CreateDonorDTO dto)
        {
            return (Authorized(token => CreateDonorForAuthenticatedUser(token, dto), () => CreateDonorForUnauthenticatedUser(dto)));
        }

        private IHttpActionResult CreateDonorForUnauthenticatedUser(CreateDonorDTO dto)
        {
            ContactDonor donor;
            try
            {
                donor = gatewayDonorService.GetContactDonorForEmail(dto.email_address);
            }
            catch (Exception e)
            {
                var msg = "Error getting donor for email " + dto.email_address;
                logger.Error(msg, e);
                var apiError = new ApiErrorDto(msg, e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
            int existingDonorId = 
                (donor == null) ? 
                    0 : 
                    donor.DonorId;

            try
            {
                donor = gatewayDonorService.CreateOrUpdateContactDonor(donor, dto.email_address, dto.stripe_token_id, DateTime.Now);
            }
            catch (Exception e)
            {
                var msg = "Error creating donor for email " + dto.email_address;
                logger.Error(msg, e);
                var apiError = new ApiErrorDto(msg, e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }

            var responseBody = new DonorDTO
            {
                id = donor.DonorId,
                Processor_ID = donor.ProcessorId,
            };

            // HTTP StatusCode should be 201 (Created) if we created a donor, or 200 (Ok) if returning an existing donor
            var statusCode =
                (existingDonorId == donor.DonorId) ?
                    HttpStatusCode.OK :
                    HttpStatusCode.Created;
            return (ResponseMessage(Request.CreateResponse<DonorDTO>(statusCode, responseBody)));
        }

        private IHttpActionResult CreateDonorForAuthenticatedUser(String authToken, CreateDonorDTO dto)
        {
            try
            {
                var donor = gatewayDonorService.GetContactDonorForAuthenticatedUser(authToken);
                donor = gatewayDonorService.CreateOrUpdateContactDonor(donor, string.Empty, dto.stripe_token_id, DateTime.Now);

                var response = new DonorDTO
                {
                    id = donor.DonorId,
                    Processor_ID = donor.ProcessorId
                };

                return Ok(response);
            }
            catch (Exception exception)
            {
                var apiError = new ApiErrorDto("Donor Post Failed", exception);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        private IHttpActionResult GetDonorForAuthenticatedUser(string token)
        {
            try
            {
                var donor = gatewayDonorService.GetContactDonorForAuthenticatedUser(token);

                if (donor == null || !donor.HasPaymentProcessorRecord)
                {
                    return (NotFound());
                }
                else
                {
                    var default_source = stripePaymentService.getDefaultSource(donor.ProcessorId);
                    
                    var response = new DonorDTO()   
                    {
                        id = donor.DonorId,
                        Processor_ID = donor.ProcessorId,
                        default_source = new DefaultSourceDTO
                        {
                            credit_card = new CreditCardDTO
                            {
                               last4 = default_source.credit_card.last4,
                               name = default_source.credit_card.name,
                               brand = default_source.credit_card.brand,
                               address_zip = default_source.credit_card.address_zip,
                               exp_date = default_source.credit_card.exp_date
                            },
                            bank_account = new BankAccountDTO
                            {
                               last4 = default_source.bank_account.last4,
                               routing = default_source.bank_account.routing
                            }
                         }
                    };

                    return Ok(response);
                }
            }
            catch (Exception exception)
            {
                var apiError = new ApiErrorDto("Donor Get Failed", exception);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        private IHttpActionResult GetDonorForUnauthenticatedUser(string email)
        {
            try
            {
                var donor = gatewayDonorService.GetContactDonorForEmail(email);
                if (donor == null || !donor.HasPaymentProcessorRecord)
                {
                    return NotFound();
                }
                else
                {
                    var response = new DonorDTO
                    {
                        id = donor.DonorId,
                        Processor_ID = donor.ProcessorId
                    };

                    return Ok(response); 
                }
            }
            catch (Exception exception)
            {
                var apiError = new ApiErrorDto("Donor Get Failed", exception);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [ResponseType(typeof (DonorDTO))]
        [Route("api/donor")]
        public IHttpActionResult Put([FromBody] UpdateDonorDTO dto)
        {
            return Authorized(token =>
            {
                ContactDonor contactDonor;
                DefaultSourceDTO defaultSource;
     
                try
                {
                    contactDonor = gatewayDonorService.GetContactDonorForAuthenticatedUser(token);

                    //Post apistripe/customer/{custID}/sources pass in the dto.stripe_token_id
                    defaultSource = stripePaymentService.updateCustomerSource(contactDonor.ProcessorId, dto.stripe_token_id);

                }
                catch (StripeException stripeException)
                {
                    var apiError = new ApiErrorDto("Error calling payment processor:"+ stripeException.Message, stripeException);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
                catch (ApplicationException applicationException)
                {
                    var apiError = new ApiErrorDto("Error calling Ministry Platform" + applicationException.Message, applicationException);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
                //return donor
                var donor = new DonorDTO
                {
                    id = contactDonor.DonorId,
                    Processor_ID = contactDonor.ProcessorId,
                    default_source = new DefaultSourceDTO
                    {
                        credit_card = new CreditCardDTO
                        {
                            brand = defaultSource.credit_card.brand,
                            last4 = defaultSource.credit_card.last4,
                            name = defaultSource.credit_card.name,
                            address_zip = defaultSource.credit_card.address_zip,
                            exp_date = defaultSource.credit_card.exp_date
                        },
                        bank_account = new BankAccountDTO
                        {
                            last4 = defaultSource.bank_account.last4,
                            routing = defaultSource.bank_account.routing
                        }
                    }
                };

                return Ok(donor);
            });
            

        }
    }
}

