﻿using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Models.Crossroads.GoVolunteer;
using crds_angular.Security;
using crds_angular.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class GoVolunteerController : MPAuth
    {
        private readonly IOrganizationService _organizationService;

        public GoVolunteerController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet]
        [ResponseType(typeof(Organization))]
        [Route("api/organization/{name}")]
        public IHttpActionResult GetOrganization(string name)
        {
            var org = _organizationService.GetOrganizationByName(name);
            if (org == null)
            {
                return NotFound();
            }
            return Ok(org);
        }
    }
}
