using Newtonsoft.Json;

namespace crds_angular.Models.Crossroads.Serve
{
    public class ServeRole
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "capacity")]
        public Capacity Capacity { get; set; }

        //[JsonProperty(PropertyName = "slotsTaken")]
        //public int SlotsTaken { get; set; }

        [JsonProperty(PropertyName = "roleId")]
        public int RoleId { get; set; }
    }

    public class Capacity
    {
        public int Available { get; set; }
        public string BadgeType { get; set; }
        public bool Display { get; set; }
        public int Maximum { get; set; }
        public string Message { get; set; }
        public int Minimum { get; set; }
        public int Taken { get; set; }
        
    }
}