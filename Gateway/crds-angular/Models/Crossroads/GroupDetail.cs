﻿using System.Collections.Generic;
using crds_angular.Models.Json;
using Newtonsoft.Json;


namespace crds_angular.Models.Crossroads
{
    
    public class GroupDetail
    {
        [JsonProperty(PropertyName = "groupId")]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "groupFullInd")]
        public bool GroupFullInd { get; set; }

        [JsonProperty(PropertyName = "waitListInd")]
        public bool WaitListInd { get; set; }

        [JsonProperty(PropertyName = "waitListGroupId")]
        public int WaitListGroupId { get; set; }
    }
}