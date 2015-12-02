﻿using System;
using System.Configuration;
using System.Reflection;
using crds_angular.Services.Interfaces;
using log4net;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

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