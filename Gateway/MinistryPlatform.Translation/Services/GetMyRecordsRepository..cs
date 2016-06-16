﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Translation.Services.Interfaces;
using RoleDTO = MinistryPlatform.Translation.Models.DTO.MpRoleDto;

namespace MinistryPlatform.Translation.Services
{
    public class GetMyRecordsRepository : BaseRepository
    {
        public GetMyRecordsRepository(IAuthenticationService authenticationService, IConfigurationWrapper configurationWrapper)
            : base(authenticationService, configurationWrapper)
        {
            
        }

        public static List<RoleDTO> GetMyRoles(string token)
        {
            var pageId = Convert.ToInt32(ConfigurationManager.AppSettings["MyRoles"]);
            var pageRecords = MinistryPlatformService.GetRecordsDict(pageId, token);

            return pageRecords.Select(record => new RoleDTO
            {
                Id = (int) record["Role_ID"], Name = (string) record["Role_Name"]
            }).ToList();
        }
    }
}