﻿using System;
using System.Configuration;
using System.Reflection;
using crds_angular.Services;
using log4net;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Crossroads.ChildcareRsvp
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main()
        {
            var section = (UnityConfigurationSection) ConfigurationManager.GetSection("unity");
            var container = new UnityContainer();
            section.Configure(container);

            try
            {
                var childcareService = container.Resolve<ChildcareService>();
                childcareService.SendRequestForRsvp();
            }
            catch (Exception ex)
            {
                Log.Error("Childcare RSVP Email Process failed.", ex);
                Environment.Exit(9999);
            }
        }
    }
}