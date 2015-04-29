﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using crds_angular.Extenstions;
using crds_angular.Models.Crossroads;
using crds_angular.Models.Crossroads.Opportunity;
using crds_angular.Security;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services;
using MinistryPlatform.Translation.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class OpportunityController : MPAuth
    {

        private IOpportunityService _opportunityService;

        public OpportunityController(IOpportunityService opportunityService)
        {
            this._opportunityService = opportunityService;
        }

        [ResponseType(typeof (int))]
        [Route("api/opportunity/{id}")]
        public IHttpActionResult Post(int id, [FromBody] string stuff)
        {            
            var comments = string.Format("Request on {0}", DateTime.Now.ToString(CultureInfo.CurrentCulture));

            return Authorized(token =>
            {
                try
                {
                    var opportunityId = OpportunityService.RespondToOpportunity(token, id, comments);
                    return this.Ok(opportunityId);
                }
                catch (Exception ex)
                {
                    return this.InternalServerError(ex);
                }

            });
        }

        [ResponseType(typeof (Dictionary<string, long>))]
        [Route("api/opportunity/getLastOpportunityDate/{id}")]
        public IHttpActionResult GetLastOpportunityDate(int id)
        {
            return Authorized(token => this.Ok(new Dictionary<string, long> { {"date", _opportunityService.GetLastOpportunityDate(id, token).ToUnixTime()}}));
        }

        [ResponseType(typeof (OpportunityGroup))]
        [Route("api/opportunity/getGroupParticipantsForOpportunity/{id}")]
        public IHttpActionResult GetGroupParticipantsForOpportunity(int id)
        {
            return Authorized(token =>
            {
                var group = _opportunityService.GetGroupParticipantsForOpportunity(id, token);
                var oppGrp = Mapper.Map<Group, OpportunityGroup>(group);
                return this.Ok(oppGrp);
            });
        }
    }
}