(function () {
  'use strict';

  module.exports = GroupInvitationCtrl;

  GroupInvitationCtrl.$inject = [
    '$cookies',
    '$log',
    '$state',
    '$stateParams',
    'DAYS',
    'Email',
    'EMAIL_TEMPLATES',
    'GroupInfo',
    'GroupInvitationService',
    'GROUP_ROLE_ID_PARTICIPANT',
    'Responses'
  ];

  function GroupInvitationCtrl ($cookies,
                                $log,
                                $state,
                                $stateParams,
                                DAYS,
                                Email,
                                EMAIL_TEMPLATES,
                                GroupInfo,
                                GroupInvitationService,
                                GROUP_ROLE_ID_PARTICIPANT,
                                Responses) {

    var vm = this;

    vm.groupId = $stateParams.groupId;
    vm.requestPending = true;
    vm.showInvite = false;
    vm.capacity = 0;
    vm.goToDashboard = goToDashboard;
    vm.initialize = initialize;
    vm.groupId = parseInt($stateParams.groupId);
    vm.alreadyJoined = false;

    // if there are responses, then the user came through QA flow
    function initialize() {
      //if (GroupInfo.isParticipatingOrHost(vm.groupId)) {
      //  vm.requestPending = false;
      //  vm.alreadyJoined = true;
      //}

      if (vm.alreadyJoined === false) {
        if (_.has(Responses.data , 'completedQa')) {
          vm.capacity = 1;

          // Set capacity to account for invited spouse
          if (parseInt(Responses.data.relationship_status) === 2) {
            vm.capacity = 2;
            vm.showInvite = true;
          }
        }

        GroupInvitationService.acceptInvitation(vm.groupId,
          {capacity: vm.capacity, groupRoleId: GROUP_ROLE_ID_PARTICIPANT}
          )
          .then(function invitationAccepted() {
            // Invitation acceptance was successful
            vm.accepted = true;

            // Force the group info cache to reload to pick up the invited user
            return GroupInfo.loadGroupInfo(true);
          })
          .then(function groupInfoLoadCompleted() {
            // Send a public or private invitation complete email
            var cid = $cookies.get('userId');
            var group = GroupInfo.findParticipatingOrHost(vm.groupId);
            if (cid && group) {
              var email = {
                groupId: group.groupId,
                fromContactId: cid,
                toContactId: cid
              };

              if (!group.isPrivate) {
                email.templateId = EMAIL_TEMPLATES.PARTICIPANT_PUBLIC_CONFIRM_EMAIL_ID;
                email.mergeData = {
                  HostName: group.contact ? group.contact.firstName : null,
                  AddressLine1: group.address.addressLine1,
                  MeetingDay: group.meetingDay,
                  MeetingTime: group.meetingHour
                };
              } else {
                email.templateId = EMAIL_TEMPLATES.PARTICIPANT_PRIVATE_CONFIRM_EMAIL_ID;
              }

              Email.Mail.save(email).$promise.catch(function emailError(error) {
                $log.error("Email confirmation failed", error);
              });

            } else {
              $log.error("Could not find the participant's group to send confirmation email");
            }
          })
          .catch(function(error) {
            // An error happened accepting the invitation
            vm.rejected = true;
            $log.error("An error happened while accepting a group invitation", error);
          })
          .finally(function() {
            vm.requestPending = false;
          });
      }
    }

    function goToDashboard() {
      $state.go('group_finder.dashboard');
    }

    vm.initialize();
  }
})();
