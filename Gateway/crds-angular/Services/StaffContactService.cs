﻿using System.Collections.Generic;
using crds_angular.Services.Interfaces;
using MinistryPlatform.Translation.Services.Interfaces;

namespace crds_angular.Services
{
    public class StaffContactService : IStaffContactService
    {
        private readonly IContactService _contactService;

        public StaffContactService(IContactService contactService)
        {
            _contactService = contactService;
        }

        public List<Dictionary<string, object>> GetStaffContacts(string token)
        {
            var records = _contactService.StaffContacts(token);
            return records;
        }
    }
}