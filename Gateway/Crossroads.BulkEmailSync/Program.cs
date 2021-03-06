﻿using System;
using System.Configuration;
using System.Reflection;
using crds_angular.App_Start;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities.Services;
using log4net;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Crossroads.BulkEmailSync
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main()
        {
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            var container = new UnityContainer();
            section.Configure(container);

            TlsHelper.AllowTls12();

            AutoMapperConfig.RegisterMappings();

            try
            {
                Log.Info("Starting Bulk Email Synchronization");

                var syncService = container.Resolve<IBulkEmailSyncService>();
                syncService.RunService();

                Log.Info("Finished Bulk Email Synchronization successfully");
            }
            catch (Exception ex)
            {
                Log.Error("Bulk Email Synchronization Process failed.", ex);
                Environment.Exit(9999);
            }
        }
    }
}
