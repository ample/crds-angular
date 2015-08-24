﻿using Newtonsoft.Json;

namespace crds_angular.Models.Crossroads.Trip
{
    public class TripGift
    {
        [JsonProperty(PropertyName = "donorNickname")]
        public string DonorNickname { get; set; }
        
        [JsonProperty(PropertyName = "donorLastName")]
        public string DonorLastName { get; set; }

        [JsonProperty(PropertyName = "donorEmail")]
        public string DonorEmail { get; set; }

        [JsonProperty(PropertyName = "donationAmount")]
        public int DonationAmount { get; set; }

        [JsonProperty(PropertyName = "donationDate")]
        public string DonationDate { get; set; }

        [JsonProperty(PropertyName = "registeredDonor")]
        public bool RegisteredDonor { get; set; }
    }
}