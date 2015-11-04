using System;
using System.Collections.Generic;
using AutoMapper;
using crds_angular.Models.Crossroads.Profile;
using crds_angular.Services.Interfaces;
using MinistryPlatform.Models.DTO;
using MinistryPlatform.Translation.Services;
using MPServices = MinistryPlatform.Translation.Services.Interfaces;


namespace crds_angular.Services
{
    public class PersonService : MinistryPlatformBaseService, IPersonService
    {
        private readonly MPServices.IContactService _contactService;
        private readonly IContactAttributeService _contactAttributeService;
        private readonly MPServices.IApiUserService _apiUserService;

        public PersonService(MPServices.IContactService contactService, IContactAttributeService contactAttributeService, MPServices.IApiUserService apiUserService)
        {
            _contactService = contactService;
            _contactAttributeService = contactAttributeService;
            _apiUserService = apiUserService;
        }

        public void SetProfile(String token, Person person)
        {
            var contactDictionary = getDictionary(person.GetContact());
            var householdDictionary = getDictionary(person.GetHousehold());
            var addressDictionary = getDictionary(person.GetAddress());
            addressDictionary.Add("State/Region", addressDictionary["State"]);
            _contactService.UpdateContact(person.ContactId, contactDictionary, householdDictionary, addressDictionary);
            _contactAttributeService.SaveContactAttributes(person.ContactId, person.AttributeTypes, person.SingleAttributes);
        }

        public Person GetPerson(int contactId)
        {
            var contact = _contactService.GetContactById(contactId);
            var person = Mapper.Map<Person>(contact);

            var family = _contactService.GetHouseholdFamilyMembers(person.HouseholdId);
            person.HouseholdMembers = family;

            // TODO: Should this move to _contactService or should update move it's call out to this service?
            var apiUser = _apiUserService.GetToken();
            var attributesTypes = _contactAttributeService.GetContactAttributes(apiUser, contactId);
            person.AttributeTypes = attributesTypes.MultiSelect;
            person.SingleAttributes = attributesTypes.SingleSelect;

            return person;
        }

        public List<RoleDto> GetLoggedInUserRoles(string token)
        {
            return GetMyRecords.GetMyRoles(token);
        }

        public Person GetLoggedInUserProfile(String token)
        {
            var contact = _contactService.GetMyProfile(token);
            var person = Mapper.Map<Person>(contact);

            var family = _contactService.GetHouseholdFamilyMembers(person.HouseholdId);
            person.HouseholdMembers = family;

            return person;
        }

        public bool ResetPassword(string email)
        {


            return true;
        }
    }
}