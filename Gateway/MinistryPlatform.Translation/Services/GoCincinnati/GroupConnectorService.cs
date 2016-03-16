﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Utilities.Interfaces;
using log4net;
using MinistryPlatform.Translation.Services.Interfaces;
using MinistryPlatform.Translation.Services.Interfaces.GoCincinnati;

namespace MinistryPlatform.Translation.Services.GoCincinnati
{
    public class GroupConnectorService : BaseService, IGroupConnectorService
    {
        private readonly IMinistryPlatformService _ministryPlatformService;


        private readonly ILog _logger = LogManager.GetLogger(typeof(RoomService));

        public GroupConnectorService(IMinistryPlatformService ministryPlatformService, IAuthenticationService authenticationService, IConfigurationWrapper configuration)
            : base(authenticationService, configuration)
        {
            _ministryPlatformService = ministryPlatformService;
        }

        public int CreateGroupConnector(int registrationId)
        {
            var t = ApiLogin();
            var pageId = _configurationWrapper.GetConfigIntValue("GroupConnectorPageId");
            var dictionary = new Dictionary<string, object>
            {
                {"Primary_Registration", registrationId}
            };

            try
            {
                var groupConnectorId = _ministryPlatformService.CreateRecord(pageId, dictionary, t, true);
                CreateGroupConnectorRegistration(groupConnectorId, registrationId);
                return groupConnectorId;
            }
            catch (Exception e)
            {
                var msg = string.Format("Error creating Go Cincinnati Group Connector, registration: {0}", registrationId);
                _logger.Error(msg, e);
                throw (new ApplicationException(msg, e));
            }         

        }

        public int CreateGroupConnectorRegistration(int groupConnectorId, int registrationId)
        {
            var t = ApiLogin();
            var pageId = _configurationWrapper.GetConfigIntValue("GroupConnectorRegistrationPageId");
            var dictionary = new Dictionary<string, object>
            {
                {"GroupConnector_ID", groupConnectorId},
                {"Registration_ID", registrationId}
            };

            try
            {
                return (_ministryPlatformService.CreateRecord(pageId, dictionary, t, true));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error creating Go Cincinnati Group Connector Registration, group connector: {0}, registration: {1}", groupConnectorId, registrationId);
                _logger.Error(msg, e);
                throw (new ApplicationException(msg, e));
            }
        }
    }
}
