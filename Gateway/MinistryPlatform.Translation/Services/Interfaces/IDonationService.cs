﻿using System;

namespace MinistryPlatform.Translation.Services.Interfaces
{
    public interface IDonationService
    {
        int UpdateDonationStatus(int donationId, int statusId, DateTime statusDate, string statusNote = null);
        int UpdateDonationStatus(string processorPaymentId, int statusId, DateTime statusDate, string statusNote = null);
        int CreateDonationBatch(string batchName, DateTime setupDateTime, decimal batchTotalAmount, int itemCount,
            int batchEntryType, int? depositId, DateTime finalizedDateTime);
        void AddDonationToBatch(int batchId, int donationId);
        int CreateDeposit(string depositName, decimal depositTotalAmount, DateTime depositDateTime, string accountNumber, int batchCount, bool exported, string notes);
    }
}