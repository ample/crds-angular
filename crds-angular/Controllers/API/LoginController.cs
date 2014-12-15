﻿using crds_angular.Models;
using crds_angular.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.SessionState;

namespace crds_angular.Controllers.API
{
    public class LoginController : ApiController
    {
        [ResponseType(typeof(LoginReturn))]
        public IHttpActionResult Post([FromBody]Credentials cred)
        {
            SessionStateItemCollection Session = new SessionStateItemCollection();
            // try to login 
            var token = TranslationService.Login(cred.username, cred.password);
            if (token == null)
            {
                return this.Unauthorized();
            } 
            var p = PersonService.getLoggedInUserProfile(token);
            Session["token"] = token;
            var r = new LoginReturn
            {
                userToken = token,
                userId = p.Id
            };
            return this.Ok(r);
        }
    }

    public class LoginReturn
    {
        public string userToken { get; set; }
        public int userId { get; set; }
    }

    public class Credentials
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
