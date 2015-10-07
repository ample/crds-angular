using System;
using System.Collections.Generic;
using crds_angular.Models.Crossroads.Serve;

namespace crds_angular.Services.Interfaces
{
    public interface IServeService
    {
        List<int> GetUpdatedOpportunities(string token, SaveRsvpDto dto, Func<MinistryPlatform.Models.Participant, MinistryPlatform.Models.Event, Boolean> saveFunc = null);
        List<FamilyMember> GetImmediateFamilyParticipants(int contactId, string token);
        DateTime GetLastServingDate(int opportunityId, string token);
        List<QualifiedServerDto> GetQualifiedServers(int groupId, int contactId, string token);
        List<ServingDay> GetServingDays(string token, int contactId, long from, long to);
        Capacity OpportunityCapacity(int opportunityId, int eventId, int? minNeeded, int? maxNeeded, string token);
        List<int> SaveServeRsvp(string token, SaveRsvpDto dto);
    }
}