﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MinistryPlatform.Translation.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace crds_angular.Models.Crossroads.GoVolunteer
{
    public class GoSkills
    {

        public GoSkills()
        {
        }

        public GoSkills(int id, string label, string name)
        {
            SkillId = id;
            Label = label;
            Name = name;
        }

        [JsonProperty(PropertyName = "skillId")]
        public int SkillId { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }        

        public List<GoSkills> ToGoSkills(List<MPGoVolunteerSkill> skills)
        {
            return skills.Select(skill => new GoSkills(skill.GoVolunteerSkillId, skill.Label, skill.AttributeName)).ToList();
        } 

    }
}