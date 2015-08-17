﻿using System.Collections.Generic;
using crds_angular.Models.Crossroads.Stewardship;
using Newtonsoft.Json;

namespace crds_angular.Models.Crossroads.Trip
{
    public class TripFormResponseDto
    {
        [JsonProperty(PropertyName = "applicants")]
        public List<TripApplicant> Applicants { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public List<TripGroupDto> Groups { get; set; }
    }

    public class TripApplicant
    {
        [JsonProperty(PropertyName = "contactId")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "participantId")]
        public int ParticipantId { get; set; }

        [JsonProperty(PropertyName = "donorId")]
        public int? DonorId { get; set; }
    }

    public class SaveTripParticipantsDto
    {
        [JsonProperty(PropertyName = "applicants")]
        public List<TripApplicant> Applicants { get; set; }

        [JsonProperty(PropertyName = "group")]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "pledgeCampaign")]
        public PledgeCampaign Campaign { get; set; }
    }
}