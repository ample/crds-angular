﻿using System;
using System.Collections.Generic;
using System.Linq;
using crds_angular.Models.Crossroads.GoVolunteer;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities.Interfaces;
using log4net;
using MinistryPlatform.Translation.Services.Interfaces.GoCincinnati;
using IGroupConnectorService = MinistryPlatform.Translation.Services.Interfaces.GoCincinnati.IGroupConnectorService;
using MPInterfaces = MinistryPlatform.Translation.Services.Interfaces;

namespace crds_angular.Services
{
    public class GoVolunteerService : IGoVolunteerService

    {
        private readonly IAttributeService _attributeService;
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly MPInterfaces.IContactRelationshipService _contactRelationshipService;
        private readonly MPInterfaces.IContactService _contactService;
        private readonly IGroupConnectorService _groupConnectorService;
        private readonly ILog _logger = LogManager.GetLogger(typeof (GoVolunteerService));
        private readonly int _otherEquipmentId;
        private readonly MPInterfaces.IParticipantService _participantService;
        private readonly MPInterfaces.IProjectTypeService _projectTypeService;
        private readonly IRegistrationService _registrationService;
        private readonly IGoSkillsService _skillsService;

        public GoVolunteerService(MPInterfaces.IParticipantService participantService,
                                  IRegistrationService registrationService,
                                  MPInterfaces.IContactService contactService,
                                  IGroupConnectorService groupConnectorService,
                                  IConfigurationWrapper configurationWrapper,
                                  MPInterfaces.IContactRelationshipService contactRelationshipService,
                                  MPInterfaces.IProjectTypeService projectTypeService,
                                  IAttributeService attributeService,
                                  IGoSkillsService skillsService)
        {
            _participantService = participantService;
            _registrationService = registrationService;
            _contactService = contactService;
            _groupConnectorService = groupConnectorService;
            _configurationWrapper = configurationWrapper;
            _contactRelationshipService = contactRelationshipService;
            _projectTypeService = projectTypeService;
            _attributeService = attributeService;
            _otherEquipmentId = _configurationWrapper.GetConfigIntValue("GoCincinnatiOtherEquipmentAttributeId");
            _skillsService = skillsService;
        }

        public List<ChildrenOptions> ChildrenOptions()
        {
            var attributeTypeId = _configurationWrapper.GetConfigIntValue("GoCincinnatiRegistrationChildrenAttributeType");
            var attributeTypes = _attributeService.GetAttributeTypes(attributeTypeId);
            var attributes = attributeTypes.Single().Attributes;
            return attributes.Select(attribute => new ChildrenOptions
            {
                AttributeId = attribute.AttributeId,
                PluralLabel = string.Concat("Children Ages ", attribute.Name),
                SingularLabel = string.Concat("Child Age ", attribute.Name),
                Value = 0
            }).ToList();
        }

        public Registration CreateRegistration(Registration registration, string token)
        {
            var registrationDto = new MinistryPlatform.Translation.Models.GoCincinnati.Registration();
            try
            {
                registrationDto.ParticipantId = RegistrationContact(registration, token, registrationDto);
                var registrationId = CreateRegistration(registration, registrationDto);
                GroupConnector(registration, registrationId);
                SpouseInformation(registration);
                _skillsService.UpdateSkills(registrationDto.ParticipantId, registration.Skills, token);               
                Attributes(registration, registrationId);
            }
            catch (Exception ex)
            {
                const string msg = "Go Volunteer Service: CreateRegistration";
                _logger.Error(msg, ex);
                throw new Exception(msg, ex);
            }
            return registration;
        }

        public List<ProjectType> GetProjectTypes()
        {
            var pTypes = _projectTypeService.GetProjectTypes();
            return pTypes.Select(pt =>
            {
                var projType = new ProjectType();
                return projType.FromMpProjectType(pt);
            }).ToList();
        }

        private void Attributes(Registration registration, int registrationId)
        {
            ChildAgeGroups(registration, registrationId);
            PrepWork(registration, registrationId);
            Equipment(registration, registrationId);
            ProjectPreferences(registration, registrationId);
        }

        private void ProjectPreferences(Registration registration, int registrationId)
        {
            foreach (var projectPreference in registration.ProjectPreferences.Where(pref => pref.Id != 0))
            {
                _registrationService.AddProjectPreferences(registrationId, projectPreference.Id, projectPreference.Priority);
            }
        }

        private void Equipment(Registration registration, int registrationId)
        {
            foreach (var equipment in registration.Equipment)
            {
                var id = equipment.Id != 0 ? equipment.Id : _otherEquipmentId;
                _registrationService.AddEquipment(registrationId, id, equipment.Notes);
            }
        }

        private void PrepWork(Registration registration, int registrationId)
        {
            foreach (var prepWork in registration.PrepWork)
            {
                _registrationService.AddPrepWork(registrationId, prepWork.Id, prepWork.Spouse);
            }
        }

        private void ChildAgeGroups(Registration registration, int registrationId)
        {
            foreach (var ageGroup in registration.ChildAgeGroup)
            {
                _registrationService.AddAgeGroup(registrationId, ageGroup.Id, ageGroup.Count);
            }
        }

        private void SpouseInformation(Registration registration)
        {
            if (!AddSpouse(registration))
            {
                return;
            }
            var contactId = _contactService.CreateSimpleContact(registration.Spouse.FirstName,
                                                                registration.Spouse.LastName,
                                                                registration.Spouse.EmailAddress,
                                                                registration.Spouse.DateOfBirth,
                                                                registration.Spouse.MobilePhone);

            CreateRelationship(registration, contactId);
        }

        private void CreateRelationship(Registration registration, int contactId)
        {
            var relationship = new MinistryPlatform.Models.Relationship
            {
                RelationshipID = _configurationWrapper.GetConfigIntValue("MarriedTo"),
                RelatedContactID = contactId,
                StartDate = DateTime.Today
            };
            _contactRelationshipService.AddRelationship(relationship, registration.Self.ContactId);
        }

        private static bool AddSpouse(Registration registration)
        {
            if (!registration.SpouseParticipation)
            {
                return false;
            }
            return registration.Spouse.ContactId == 0;
        }

        private void GroupConnector(Registration registration, int registrationId)
        {
            if (registration.CreateGroupConnector)
            {
                _groupConnectorService.CreateGroupConnector(registrationId, registration.PrivateGroup);
            }
            else if (registration.GroupConnectorId != 0)
            {
                _groupConnectorService.CreateGroupConnectorRegistration(registration.GroupConnectorId, registrationId);
            }
        }

        private int CreateRegistration(Registration registration, MinistryPlatform.Translation.Models.GoCincinnati.Registration registrationDto)
        {
            var preferredLaunchSiteId = PreferredLaunchSite(registration);
            registrationDto.AdditionalInformation = registration.AdditionalInformation;
            registrationDto.InitiativeId = registration.InitiativeId;
            registrationDto.OrganizationId = registration.OrganizationId;
            registrationDto.PreferredLaunchSiteId = preferredLaunchSiteId;
            registrationDto.RoleId = registration.RoleId;
            registrationDto.SpouseParticipation = registration.SpouseParticipation;
            return Registration(registrationDto);
        }

        private int Registration(MinistryPlatform.Translation.Models.GoCincinnati.Registration registrationDto)
        {
            int registrationId;
            try
            {
                registrationId = _registrationService.CreateRegistration(registrationDto);
            }
            catch (Exception ex)
            {
                _logger.Error("GO Volunteer Service - Create Registration (Create Registration)", ex);
                throw;
            }
            return registrationId;
        }

        private int PreferredLaunchSite(Registration registration)
        {
            int preferredLaunchSiteId;
            if (registration.PreferredLaunchSiteId == 0)
            {
                // use group connector
                var groupConnector = _groupConnectorService.GetGroupConnectorById(registration.GroupConnectorId);
                preferredLaunchSiteId = groupConnector.PreferredLaunchSiteId;
            }
            else
            {
                // use preferred id
                preferredLaunchSiteId = registration.PreferredLaunchSiteId;
            }
            return preferredLaunchSiteId;
        }

        private int RegistrationContact(Registration registration, string token, MinistryPlatform.Translation.Models.GoCincinnati.Registration registrationDto)
        {
            // Create or Update Contact
            var participantId = registration.Self.ContactId != 0 ? ExistingParticipant(registration, token) : CreateParticipant(registration);

            if (participantId == 0)
            {
                throw new ApplicationException("Registration Participant Not Found");
            }
            return participantId;
        }

        private int CreateParticipant(Registration registration)
        {
            //create contact & participant
            var contactId = _contactService.CreateSimpleContact(registration.Self.FirstName,
                                                                registration.Self.LastName,
                                                                registration.Self.EmailAddress,
                                                                registration.Self.DateOfBirth,
                                                                registration.Self.MobilePhone);
            registration.Self.ContactId = contactId;
            var participantId = _participantService.CreateParticipantRecord(contactId);
            return participantId;
        }

        private int ExistingParticipant(Registration registration, string token)
        {
            // update name/email/dob/mobile
            var dict = registration.Self.GetDictionary();
            _contactService.UpdateContact(registration.Self.ContactId, dict);

            //get participant
            var participant = _participantService.GetParticipantRecord(token);
            var participantId = participant.ParticipantId;
            return participantId;
        }
    }
}