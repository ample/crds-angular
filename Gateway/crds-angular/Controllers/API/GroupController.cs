﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Models.MP;
using MinistryPlatform.Translation.Services;

namespace crds_angular.Controllers.API
{
    public class GroupController : ApiController
    {
        IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [ResponseType(typeof(GroupDTO))]
        [Route("api/group/{groupId}/contact/{contactId}")]
        public IHttpActionResult Post(String groupId, String contactId, [FromBody] ContactDTO contact)
        {
            //_groupService.addContactToGroup(groupId, contactId);
            // TODO Need to determine groupRoleId to use, and either put in config or hardcode
            // TODO Can we get employee role from somewhere?
           _groupService.addContactToGroup(groupId, contactId, groupRoleId: "1", startDateTime: System.DateTime.Now, endDateTime:null, employeeRole: false);
            return this.Ok();
            
        }

        // TODO: implement later
        [ResponseType(typeof(GroupDTO))]
        [Route("api/group/{groupId}/contact")]
        public IHttpActionResult Post(String groupId, [FromBody] List<ContactDTO> contact)
        {
            throw new NotImplementedException();
            return this.Ok();
        }

        // TODO: implement later
        [ResponseType(typeof(ContactDTO))]
        [Route("api/group/{groupId}/contact/{contactId}")]
        public IHttpActionResult Get(String groupId, String contactId)
        {
            throw new NotImplementedException();
            return this.Ok();

        }

        // TODO: implement later
        [ResponseType(typeof(List<ContactDTO>))]
        [Route("api/group/{groupId}/contact")]
        public IHttpActionResult Get(String groupId)
        {
            throw new NotImplementedException();
            return this.Ok();

        }

        // TODO: implement later
        [ResponseType(typeof(GroupDTO))]
        [Route("api/group/{groupId}/contact/{contactId}")]
        public IHttpActionResult Delete(String groupId, String contactId)
        {
            throw new NotImplementedException();
            return this.Ok();

        }


    }

    public class ContactDTO
    {
    }

    public class GroupDTO
    {
    }
}
