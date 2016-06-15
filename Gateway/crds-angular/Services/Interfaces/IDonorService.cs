﻿using System;
using System.Collections.Generic;
using crds_angular.Models.Crossroads.Stewardship;
using MinistryPlatform.Translation.Models;
using MinistryPlatform.Translation.Models.DTO;

namespace crds_angular.Services.Interfaces
{
    public interface IDonorService
    {
        ContactDonor GetContactDonorForEmail(string emailAddress);

        ContactDonor GetContactDonorForAuthenticatedUser(string authToken);

        ContactDonor GetContactDonorForDonorAccount(string accountNumber, string routingNumber);

        ContactDonor GetContactDonorForDonorId(int donorId);
            
        ContactDonor GetContactDonorForCheckAccount(string encryptedKey);

        ContactDonor CreateOrUpdateContactDonor(ContactDonor existingDonor,  string encryptedKey, string emailAddress, string paymentProcessorToken = null, DateTime? setupDate = null);

        string DecryptValues(string value);

        int CreateRecurringGift(string authorizedUserToken, RecurringGiftDto recurringGiftDto, ContactDonor contact);

        RecurringGiftDto EditRecurringGift(string authorizedUserToken, RecurringGiftDto editGift, ContactDonor donor);

        Boolean CancelRecurringGift(string authorizedUserToken, int recurringGiftId);

        MpCreateDonationDistDto GetRecurringGiftForSubscription(string subscriptionId);

        List<RecurringGiftDto> GetRecurringGiftsForAuthenticatedUser(string userToken);

        List<PledgeDto> GetCapitalCampaignPledgesForAuthenticatedUser(string userToken);
    }
}
