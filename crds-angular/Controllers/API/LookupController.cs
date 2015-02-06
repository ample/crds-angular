using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using crds_angular.Security;
using crds_angular.Services;
using MinistryPlatform.Translation.Services;

namespace crds_angular.Controllers.API
{
    public class LookupController : CookieAuth
    {
        [ResponseType(typeof (List<Dictionary<string, object>>))]
        [Route("api/lookup/{table?}")]
        [HttpGet]
        public IHttpActionResult Lookup(string table)
        {
            return Authorized(t =>
            {
                var ret = new List<Dictionary<string, object>>();
                switch (table)
                {
                    case "genders":
                        ret = LookupService.Genders(t);
                        break;
                    case "maritalstatus":
                        ret = LookupService.MaritalStatus(t);
                        break;
                    case "serviceproviders":
                        ret = LookupService.ServiceProviders(t);
                        break;
                    case "countries":
                        ret = LookupService.Countries(t);
                        break;
                    case "states":
                        var json = TranslationService.GetStates(t);
                        ret = DecodeJson(json);
                        break;
                    case "crossroadslocations":
                        ret = LookupService.CrossroadsLocations(t);
                        break;
                    default:
                        break;
                }
                if (ret.Count == 0)
                {
                    return this.BadRequest(string.Format("table: {0}", table));
                }
                return Ok(ret);
            });
        }

        [ResponseType(typeof (DynamicJsonArray))]
        [HttpGet]
        [Route("api/lookup/{lookup?}")]
        public IHttpActionResult Get(string lookup)
        {
            return Authorized(t =>
            {
                if (lookup == "states")
                {
                    var states = TranslationService.GetStates(t);
                    var json = DecodeJson(states.ToString());

                    return this.Ok(json);
                }
                return this.BadRequest();
            });
        }


        [HttpGet]
        [Route("api/lookup/{email?}")]
        public IHttpActionResult EmailExists(string email)
        {
            //TODO let's clean this up
            var authorizedWithCookie = AuthorizedWithCookie(t =>
            {
                var exists = LookupService.EmailSearch(email, t.SessionId);
                if (exists.Count == 0 || Convert.ToInt32(exists["dp_RecordID"]) == t.UserId)
                {
                    return Ok();
                }
                return BadRequest();
            });

            if (authorizedWithCookie is UnauthorizedResult)
            {
                var token = AuthenticationService.authenticate(ConfigurationManager.AppSettings["ApiUser"],
                    ConfigurationManager.AppSettings["ApiPass"]);
                var exists = LookupService.EmailSearch(email, token);
                if (exists.Count == 0)
                {
                    return Ok();
                }
                return BadRequest();
            }
            return authorizedWithCookie;
        }

        protected static dynamic DecodeJson(string json)
        {
            var obj = System.Web.Helpers.Json.Decode(json);
            if (obj.GetType() != typeof (DynamicJsonArray)) return null;
            dynamic[] array = obj;
            return array;
        }
    }
}