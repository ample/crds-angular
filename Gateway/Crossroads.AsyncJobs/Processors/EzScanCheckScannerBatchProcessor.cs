﻿using crds_angular.Models.Crossroads.Stewardship;
using crds_angular.Services.Interfaces;
using Crossroads.AsyncJobs.Interfaces;
using Crossroads.AsyncJobs.Models;
using log4net;
using System;
using System.Collections.Generic;
using crds_angular.Models.Crossroads;
using Crossroads.Utilities.Interfaces;
using Newtonsoft.Json;

namespace Crossroads.AsyncJobs.Processors
{
    public class EzScanCheckScannerBatchProcessor : IJobExecutor<CheckScannerBatch>
    {
        private readonly ICheckScannerService _checkScannerService;
        private readonly IEmailCommunication _emailService;
        private readonly int _checkScannerBatchSuccessTemplateId;
        private readonly int _checkScannerBatchFailureTemplateId;

        private readonly ILog _logger = LogManager.GetLogger(typeof(CheckScannerBatch));

        public EzScanCheckScannerBatchProcessor(ICheckScannerService checkScannerService, IEmailCommunication emailService, IConfigurationWrapper configuration)
        {
            _checkScannerService = checkScannerService;
            _emailService = emailService;
            _checkScannerBatchSuccessTemplateId = configuration.GetConfigIntValue("CheckScannerBatchSuccessTemplateId");
            _checkScannerBatchFailureTemplateId = configuration.GetConfigIntValue("CheckScannerBatchFailureTemplateId");
        }

        public void Execute(JobDetails<CheckScannerBatch> details)
        {
            var batch = details.Data;
            try
            {
                _logger.Debug(string.Format("Received check scanner batch {0} at {1} (queued at {2})", batch.Name, details.RetrievedDateTime, details.EnqueuedDateTime));
                var result = _checkScannerService.CreateDonationsForBatch(batch);
                SendEmail(result, null);
            }
            catch (Exception e)
            {
                var msg = "Unexpected error processing check scanner batch " + batch.Name;
                _logger.Error(msg, e);

                SendEmail(batch, e);
            }
        }

        private void SendEmail(CheckScannerBatch batch, Exception error)
        {
            if (batch.MinistryPlatformContactId == null || batch.MinistryPlatformUserId == null)
            {
                return;
            }

            var email = new EmailCommunicationDTO
            {
                FromContactId = batch.MinistryPlatformContactId.Value,
                TemplateId = error == null ? _checkScannerBatchSuccessTemplateId : _checkScannerBatchFailureTemplateId,
                ToContactId = batch.MinistryPlatformContactId.Value,
                MergeData = new Dictionary<string, object>
                    {
                        { "batchName", batch.Name },
                        { "programId", batch.ProgramId },
                    }
            };

            if (error != null)
            {
                email.MergeData["exception"] = error;
            }
            else
            {
                email.MergeData["result"] = JsonConvert.SerializeObject(batch, Formatting.Indented);
            }


            _emailService.SendEmail(email);
        }
    }
}