using Newtonsoft.Json;

namespace crds_angular.Models.Crossroads.Trip
{
    public class TripDto
    {
        [JsonProperty(PropertyName = "tripEnd")]
        public long EventEndDate { get; set; }

        [JsonIgnore]
        public int EventId { get; set; }

        [JsonProperty(PropertyName = "tripParticipantId")]
        public int EventParticipantId { get; set; }

        [JsonProperty(PropertyName = "tripStart")]
        public long EventStartDate { get; set; }

        [JsonProperty(PropertyName = "tripName")]
        public string EventTitle { get; set; }

        [JsonIgnore]
        public string EventType { get; set; }
    }
}