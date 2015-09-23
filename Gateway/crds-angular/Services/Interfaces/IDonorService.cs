﻿using MinistryPlatform.Models;
using System;

namespace crds_angular.Services.Interfaces
{
    public interface IDonorService
    {
        ContactDonor GetContactDonorForEmail(string emailAddress);

        ContactDonor GetContactDonorForAuthenticatedUser(string authToken);

        ContactDonor GetContactDonorForDonorAccount(string accountNumber, string routingNumber);

        ContactDetails GetContactDonorForCheckAccount(string encryptedKey);

        ContactDonor CreateOrUpdateContactDonor(ContactDonor existingDonor,  string encryptedKey, string emailAddress, string paymentProcessorToken, DateTime setupDate);

        string DecryptValues(string accountNumber, string routingNumber);
    }
}
