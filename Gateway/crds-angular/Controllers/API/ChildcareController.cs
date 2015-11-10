﻿using System;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Exceptions.Models;
using crds_angular.Models.Crossroads;
using crds_angular.Security;
using crds_angular.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class ChildcareController : MPAuth
    {
        private readonly IChildcareService _childcareService;

        public ChildcareController(IChildcareService childcareService)
        {
            _childcareService = childcareService;
        }

        [ResponseType(typeof(Event))]
        [Route("api/childcare/event/{eventid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult ChildcareEventById(int eventid)
        {
            return Authorized(token =>
            {
                try
                {
                    return Ok(_childcareService.GetMyChildcareEvent(eventid, token));
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Get Event by Id failed", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }
    }
}