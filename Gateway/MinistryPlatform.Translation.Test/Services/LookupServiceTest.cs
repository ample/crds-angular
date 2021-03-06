﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Utilities.Interfaces;
using FsCheck;
using MinistryPlatform.Translation.Models.Lookups;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Services
{
    [TestFixture]
    class LookupServiceTest
    {
        private LookupRepository _fixture;
        private Mock<IMinistryPlatformService> _ministryPlatformService;
        private Mock<IConfigurationWrapper> _configurationWrapper;
        private Mock<IAuthenticationRepository> _authenticationService;
      
        [SetUp]
        public void Setup()
        {
            _ministryPlatformService = new Mock<IMinistryPlatformService>();
            _authenticationService = new Mock<IAuthenticationRepository>();
            _configurationWrapper = new Mock<IConfigurationWrapper>();
            _fixture = new LookupRepository(_authenticationService.Object, _configurationWrapper.Object,  _ministryPlatformService.Object);
            
        }

        [Test]
        public void ShouldReturnWorkTeamList()
        {

            Prop.ForAll<int, string, string>((config, token, st) =>
            {
                var wt = WorkTeams();    
                _configurationWrapper.Setup(m => m.GetConfigIntValue("WorkTeams")).Returns(config);
                _ministryPlatformService.Setup(m => m.GetLookupRecords(config, token)).Returns(wt);
                var returnVal = _fixture.GetList<MpWorkTeams>(token);
                Assert.IsInstanceOf<IEnumerable<MpWorkTeams>>(returnVal);
                Assert.AreEqual(wt.Count, returnVal.Count());
            }).QuickCheckThrowOnFailure();
        }

        [Test]
        public void ShouldReturnOtherOrgList()
        {
            Prop.ForAll<int, string, string>((config, token, st) =>
            {
                var oo = OtherOrgs();
                _configurationWrapper.Setup(m => m.GetConfigIntValue("OtherOrgs")).Returns(config);
                _ministryPlatformService.Setup(m => m.GetLookupRecords(config, token)).Returns(oo);
                var returnVal = _fixture.GetList<MpOtherOrganization>(token);
                Assert.IsInstanceOf<IEnumerable<MpOtherOrganization>>(returnVal);
                Assert.AreEqual(oo.Count, returnVal.Count());

            }).QuickCheckThrowOnFailure();

        }

        private List<Dictionary<string, object>> OtherOrgs()
        {
            return new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"dp_RecordID", 15},
                    {"dp_RecordName", "namworkteam"}
                },
                new Dictionary<string, object>()
                {
                    {"dp_RecordID", 12},
                    {"dp_RecordName", "name or workteam"}
                }
            };
        }

        private List<Dictionary<string, object>> WorkTeams()
        {
            return new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"dp_RecordID", 15},
                    {"dp_RecordName", "namworkteam"}
                },
                new Dictionary<string, object>()
                {
                    {"dp_RecordID", 12},
                    {"dp_RecordName", "name or workteam"}
                }
            };
        } 

    }
}
