﻿using System.Web.Hosting;
using System.Web.Http;
using Crossroads.AsyncJobs.Application;
using Microsoft.Practices.Unity;

namespace Crossroads.AsyncJobs
{
    public class ApplicationPreload : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            StartJobProcessor();
        }

        public static void StartJobProcessor()
        {
            GetJobProcessorInstance().Start();
        }

        public static void StopJobProcessor()
        {
            GetJobProcessorInstance().Stop();
        }

        public static JobProcessor GetJobProcessorInstance()
        {
            var uc = (IUnityContainer)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUnityContainer));
            var processor = (JobProcessor)uc.Resolve(typeof(JobProcessor), "JobProcessor");
            return (processor);
        }

    }
}