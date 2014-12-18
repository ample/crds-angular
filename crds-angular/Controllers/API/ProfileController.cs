using crds_angular.Models;
using crds_angular.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.SessionState;

namespace crds_angular.Controllers.API
{
    public class ProfileController : ApiController
    {

        [ResponseType(typeof (Person))]
        [Route("api/profile")]
        public IHttpActionResult GetProfile()
        {

            CookieHeaderValue cookie = Request.Headers.GetCookies("sessionId").FirstOrDefault();
            if (cookie != null && (cookie["sessionId"].Value != "null" || cookie["sessionId"].Value != null))
            {

                string token = cookie["sessionId"].Value;
                var person = ProfileService.getLoggedInUserProfile(token);
                if (person == null)
                {
                    return this.Unauthorized();
                }
                return this.Ok(person);
            }
            else
            {
                return this.Unauthorized();
            } 
        }

        [Route("api/profile")]
        public IHttpActionResult Post([FromBody] Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CookieHeaderValue cookie = Request.Headers.GetCookies("sessionId").FirstOrDefault();
            if (cookie.ToString() != null)
            {

                string token = cookie["sessionId"].Value;
                ProfileService.setProfile(token, profile);
                return this.Ok();
            }
            else
            {
                return this.Unauthorized();
            }
        }

       
    }

    

   

   
    
}
