﻿using System;
using crds_angular.Controllers.API;
using crds_angular.Models.MP;
using MinistryPlatform.Translation.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace crds_angular.test.controllers
{

    [TestFixture]
    public class GroupControllerTest
    {
        private MockRepository mocks;
        private IGroupService _groupService;
        private GroupController controller;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            _groupService = mocks.StrictMock<IGroupService>();
            controller = new GroupController(_groupService);
        }

        [Test]
        public void ShouldUseGroupService()
        {
            // Arrange
            var contact = new ContactDTO();
            // "Old" Record/Replay syntax, neither of these work
            //Expect.Call(
            //    delegate
            //    {
            //        //_groupService.addContactToGroup("1", "2", Arg<String>.Is.Anything, Arg<DateTime>.Is.Anything,
            //        //    Arg<DateTime>.Is.Anything, Arg<bool>.Is.Anything);
            //        _groupService.addContactToGroup("1", "2");
            //    }).IgnoreArguments();
            
            // Act
            controller.Post("1", "2", contact);
            //mocks.ReplayAll();
            
            // Assert
            _groupService.Replay();

            // "New" AAA syntax, none of these work
            //_groupService.AssertWasCalled(p => p.addContactToGroup("1", "2"));
            //_groupService.AssertWasCalled(p => p.addContactToGroup(Arg<string>.Is.Equal("1"), Arg<string>.Is.Equal("2")));
            //_groupService.AssertWasCalled(p => p.addContactToGroup(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            _groupService.VerifyAllExpectations();
        }
    }
}
