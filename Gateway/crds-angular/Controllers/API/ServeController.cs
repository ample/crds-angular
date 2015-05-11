﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Exceptions.Models;
using crds_angular.Extenstions;
using crds_angular.Models.Crossroads;
using crds_angular.Models.Crossroads.Serve;
using crds_angular.Security;
using crds_angular.Services.Interfaces;
using log4net;
using MinistryPlatform.Translation.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class ServeController : MPAuth
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IServeService _serveService;

        public ServeController(IServeService serveService, IAuthenticationService authenticationService)
        {
            _serveService = serveService;
            _authenticationService = authenticationService;
        }

        [Route("api/serve/testlist/{parms}")]
        public IHttpActionResult Get(string parms)
        {
            var array = parms.Split(Convert.ToChar(","));

            return this.Ok();
        }

        [ResponseType(typeof (List<ServingDay>))]
        [Route("api/serve/family-serve-days")]
        public IHttpActionResult GetFamilyServeDays()
        {
            return Authorized(token =>
            {
                try
                {
                    var servingDays = _serveService.GetServingDaysFaster(token);
                    if (servingDays == null)
                    {
                        return Unauthorized();
                    }
                    return Ok(servingDays);
                }
                catch (Exception exception)
                {
                    var apiError = new ApiErrorDto("Get Family Serve Days Failed", exception);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [ResponseType(typeof (List<FamilyMemberDto>))]
        [Route("api/serve/family/{contactId}")]
        public IHttpActionResult GetFamily(int contactId)
        {
            return Authorized(token =>
            {
                try
                {
                    var list = _serveService.GetImmediateFamilyParticipants(contactId, token);
                    return Ok(list);
                }
                catch (Exception ex)
                {
                    var apiError = new ApiErrorDto("Save RSVP Failed", ex);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
                
            });
        }

        [Route("api/serve/save-rsvp")]
        public IHttpActionResult SaveRsvp([FromBody] SaveRsvpDto saveRsvp)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(val => val.Errors).Aggregate("", (current, err) => current + err.Exception.Message);
                var dataError = new ApiErrorDto("RSVP Data Invalid", new InvalidOperationException("Invalid RSVP Data" + errors));
                throw new HttpResponseException(dataError.HttpResponseMessage);
            }
            //validate request
            if (saveRsvp.StartDateUnix <= 0)
            {
                var dateError = new ApiErrorDto("StartDate Invalid", new InvalidOperationException("Invalid Date"));
                throw new HttpResponseException(dateError.HttpResponseMessage);
            }

            return Authorized(token =>
            {
                try
                {
                    _serveService.SaveServeRsvp(token, saveRsvp.ContactId, saveRsvp.OpportunityId,
                        saveRsvp.EventTypeId, saveRsvp.StartDateUnix.FromUnixTime(),
                        saveRsvp.EndDateUnix.FromUnixTime(), saveRsvp.SignUp, saveRsvp.AlternateWeeks);
                }
                catch (Exception exception)
                {
                    var apiError = new ApiErrorDto("Save RSVP Failed", exception);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
                return Ok();
            });
        }
    }
}
