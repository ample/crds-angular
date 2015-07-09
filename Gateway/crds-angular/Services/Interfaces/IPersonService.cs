using System;
using System.Collections.Generic;
using crds_angular.Models;
using crds_angular.Models.Crossroads;
using MinistryPlatform.Models.DTO;

namespace crds_angular.Services.Interfaces
{
    public interface IPersonService
    {
        void SetProfile(String token, Person person);
        List<Skill> GetLoggedInUserSkills(int contactId, string token);
        Person GetLoggedInUserProfile(String token);
        Person GetPerson(int contactId);
        List<RoleDto> GetLoggedInUserRoles(string token);
    }
}