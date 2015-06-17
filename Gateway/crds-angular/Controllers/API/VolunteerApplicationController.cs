﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using crds_angular.Exceptions.Models;
using crds_angular.Models.Crossroads.VolunteerApplication;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Exceptions.Models;
using crds_angular.Models.Crossroads.Serve;
using crds_angular.Security;
using crds_angular.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class VolunteerApplicationController : MPAuth
    {
        private readonly IVolunteerApplicationService _volunteerApplicationService;


         public VolunteerApplicationController(IVolunteerApplicationService volunteerApplicationService)
        {
            _volunteerApplicationService = volunteerApplicationService;
        }

        [Route("api/volunteer-application/adult")]
        public IHttpActionResult SaveAdult([FromBody] AdultApplicationDto application)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(val => val.Errors).Aggregate("", (current, err) => current + err.ErrorMessage + " ");
                var dataError = new ApiErrorDto("SaveAdult Data Invalid", new InvalidOperationException("Invalid SaveAdult Data" + errors));
                throw new HttpResponseException(dataError.HttpResponseMessage);
            }

            try
            {
                _volunteerApplicationService.SaveAdult(application);
            }
            catch (Exception exception)
            {
                var apiError = new ApiErrorDto("Volunteer Application POST Failed", exception);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
            return Ok();
        }

        [Route("api/volunteer-application/student")]
        public IHttpActionResult SaveStudent([FromBody] StudentApplicationDto application)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(val => val.Errors)
                    .Aggregate("", (current, err) => current + err.ErrorMessage + " ");
                var dataError = new ApiErrorDto("SaveStudent Data Invalid",
                    new InvalidOperationException("Invalid SaveStudent Data" + errors));
                throw new HttpResponseException(dataError.HttpResponseMessage);
            }

            try
            {
                _volunteerApplicationService.SaveStudent(application);
            }
            catch (Exception exception)
            {
                var apiError = new ApiErrorDto("Volunteer Application POST Failed", exception);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
            return Ok();
        }

        [ResponseType(typeof(List<FamilyMember>))]
        [Route("api/volunteer-application/family/{contactId}")]
        public IHttpActionResult GetFamily(int contactId)
        {
            return Authorized(token =>
            {
                try
                {
                    var family = _volunteerApplicationService.FamilyThatUserCanSubmitFor(contactId, token);
                    return Ok(family);
                }
                catch (Exception ex)
                {
                    var apiError = new ApiErrorDto("Volunteer Application GET Family", ex);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }

            });
        }
    }
}
