using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace crds_angular.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        { 
            return View("Index");
        }        
    }
}
