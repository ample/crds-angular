﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using AutoMapper;
using crds_angular.Models;
using crds_angular.Models.Crossroads;
using Microsoft.Ajax.Utilities;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services;
using Attribute = MinistryPlatform.Models.Attribute;
using Event = MinistryPlatform.Models.Event;
using Response = crds_angular.Models.Crossroads.Response;

namespace crds_angular.Services
{
    public class PersonService : MinistryPlatformBaseService
    {
        public void setProfile(String token, Person person)
        {
            var contactDictionary = getDictionary(person.GetContact());
            var householdDictionary = getDictionary(person.GetHousehold());
            var addressDictionary = getDictionary(person.GetAddress());
            addressDictionary.Add("State/Region", addressDictionary["State"]);

            MinistryPlatformService.UpdateRecord(AppSetting("MyContact"), contactDictionary, token);

            if (addressDictionary["Address_ID"] != null)
            {
                //address exists, update it
                MinistryPlatformService.UpdateRecord(AppSetting("MyAddresses"), addressDictionary, token);
            }
            else
            {
                //address does not exist, create it, then attach to household
                var addressId = MinistryPlatformService.CreateRecord(AppSetting("MyAddresses"), addressDictionary, token);
                householdDictionary.Add("Address_ID", addressId);
            }
            MinistryPlatformService.UpdateRecord(AppSetting("MyHousehold"), householdDictionary, token);
        }

        public List<Skill> getLoggedInUserSkills(int contactId, string token)
        {
            return GetSkills(contactId, token);
        }

        public Person getLoggedInUserProfile(String token)
        {
            var contact = MinistryPlatformService.GetRecordsArr(AppSetting("MyProfile"), token);
            if (contact.Count == 0)
            {
                throw new InvalidOperationException("getLoggedInUserProfile - no data returned.");
            }
            var contactJson = TranslationService.DecodeJson(contact.ToString());

            var person = new Person
            {
                Contact_Id = contactJson.Contact_Id,
                Email_Address = contactJson.Email_Address,
                NickName = contactJson.Nickname,
                First_Name = contactJson.First_Name,
                Middle_Name = contactJson.Middle_Name,
                Last_Name = contactJson.Last_Name,
                Maiden_Name = contactJson.Maiden_Name,
                Mobile_Phone = contactJson.Mobile_Phone,
                Mobile_Carrier = contactJson.Mobile_Carrier_ID,
                Date_of_Birth = contactJson.Date_of_Birth,
                Marital_Status_Id = contactJson.Marital_Status_ID,
                Gender_Id = contactJson.Gender_ID,
                Employer_Name = contactJson.Employer_Name,
                Address_Line_1 = contactJson.Address_Line_1,
                Address_Line_2 = contactJson.Address_Line_2,
                City = contactJson.City,
                State = contactJson.State,
                Postal_Code = contactJson.Postal_Code,
                Anniversary_Date = contactJson.Anniversary_Date,
                Foreign_Country = contactJson.Foreign_Country,
                County = contactJson.County,
                Home_Phone = contactJson.Home_Phone,
                Congregation_ID = contactJson.Congregation_ID,
                Household_ID = contactJson.Household_ID,
                Address_Id = contactJson.Address_ID
            };

            return person;
        }

        private List<Skill> GetSkills(int recordId, string token)
        {
            var attributes = GetMyRecords.GetMyAttributes(recordId, token);

            var skills =
                Mapper.Map<List<Attribute>, List<Skill>>(attributes);

            return skills;
        }

        public List<FamilyMember> GetMyFamily(int recordId, string token)
        {
            var contactRelationships = GetMyRecords.GetMyFamily(recordId, token).ToList();
            var familyMembers = Mapper.Map<List<Contact_Relationship>, List<FamilyMember>>(contactRelationships);
            return familyMembers;
        }

        public List<tmServingTeam> GetMeMyFamilysServingStuff(string token)
        {
            var personService = new PersonService();
            //probaby a better way, this is just a test
            var loggedInUserProfile = personService.getLoggedInUserProfile(token);

            var servingTeams = new List<tmServingTeam>();
            var myTeams = GetMyRecords.GetMyServingTeams(loggedInUserProfile.Contact_Id, token);
            
            foreach (var team in myTeams)
            {
                //if team already in servingTeams, just add role
                if (servingTeams.Any(s => s.Name == team.GroupName))
                {
                    var tmp = team;
                    //why this?
                    var s = servingTeams.Single(t => t.Name == tmp.GroupName);
                    s.Members[0].Roles.Add(new tmRoles{Name = tmp.GroupRole});
                }
                else
                {



                    var servingTeam = new tmServingTeam();
                    servingTeam.Name = team.GroupName;

                    //if person already in servingTeam, just add role


                    var groupMembers = new List<TmTeamMember>();
                    var groupMember = new TmTeamMember();
                    groupMember.ContactId = loggedInUserProfile.Contact_Id;
                    groupMember.Name = loggedInUserProfile.First_Name;


                    var role = new tmRoles();
                    role.Name = team.GroupRole;

                    groupMembers.Add(groupMember);
                    servingTeam.Members = groupMembers;
                    servingTeams.Add(servingTeam);
                }

                //get all catch all roles for team
            }

            


            //now go get family
            var familyMembers = personService.GetMyFamily(loggedInUserProfile.Contact_Id, token);
            foreach (var familyMember in familyMembers)
            {
                var groups = GetMyRecords.GetMyServingTeams(familyMember.ContactId, token);
                foreach (var group in groups)
                {
                    if (servingTeams.Any(s => s.Name == group.GroupName))
                    {
                        var tmp = group;
                        //why this?
                        var s = servingTeams.Single(t => t.Name == tmp.GroupName);

                        //is this person already on team?
                        if (s.Members.Any(x => x.ContactId == familyMember.ContactId))
                        {
                            //found a match
                            var member = s.Members.Single(q => q.ContactId == familyMember.ContactId);
                            var role = new tmRoles {Name = tmp.GroupRole};
                            member.Roles.Add(role);
                        }
                        else
                        {

                            var groupMember = new TmTeamMember();
                            groupMember.ContactId = familyMember.ContactId;
                            groupMember.Name = familyMember.PreferredName;

                            s.Members.Add(groupMember);
                        }
                    }
                    else
                    {
                        var servingTeam = new tmServingTeam();
                        servingTeam.Name = group.GroupName;
                        var groupMembers = new List<TmTeamMember>();
                        var groupMember = new TmTeamMember();
                        groupMember.ContactId = familyMember.ContactId;
                        groupMember.Name = familyMember.PreferredName;
                        groupMembers.Add(groupMember);
                        servingTeam.Members = groupMembers;
                        servingTeams.Add(servingTeam);
                    }
                    
                }

            }
            return servingTeams;
        }

        public List<ServingTeam> GetServingOpportunities(int contactId, string token)
        {
            var groups = GetMyRecords.GetMyServingTeams(contactId, token);
            var teams = new List<ServingTeam>();
            foreach (var group in groups)
            {
                var team = new ServingTeam();
                team.TeamName = group.GroupName;
                var opportunities = OpportunityService.GetOpportunitiesForGroup(group.GroupId, token);
                foreach (var opp in opportunities)
                {
                    var opportunity = new ServingOpportunity();
                    opportunity.OpportunityName = opp.OpportunityName;
                    opportunity.ServeOccurances = ParseEvents(opp.Events);
                    //opportunity.OpportunityDateTime = opp.Opportunity_Date;
                    var response = OpportunityService.GetMyOpportunityResponses(contactId, opp.OpportunityId, token);
                    if (response != null)
                    {
                        opportunity.Rsvp = new ServingRSVP
                        {
                            //Occurrence = response.Opportunity_Date,
                            Response = ParseResponseResult(response)
                        };
                    }

                    team.Opportunities.Add(opportunity);
                }
                teams.Add(team);
            }
            return teams;
        }

        private static List<ServeOccurance> ParseEvents(IEnumerable<Event> events)
        {
            return
                events.Select(e => new ServeOccurance {Name = e.EventTitle, StarDateTime = e.EventStartDate}).ToList();
        }

        private static Response ParseResponseResult(MinistryPlatform.Models.Response response)
        {
            switch (response.Response_Result_ID)
            {
                case 1:
                    //Placed
                    return Response.Yes;
                case 2:
                    //Not Placed
                    return Response.No;
                case null:
                    //Maybe?
                    return Response.Maybe;
                default:
                    throw new ApplicationException("Invalid Opportunity Response Result.");
            }
        }
    }
}

public class tmServingTeam
{
    public string Name { get; set; }
    public List<TmTeamMember> Members { get; set; }
}

public class TmTeamMember
{
    public string Name { get; set; }
    public int ContactId { get; set; }
    public List<tmRoles> Roles { get; set; }
}

public class tmRoles
{
    public string Name { get; set; }
}