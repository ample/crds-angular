(function(){
  'use strict';

  module.exports = GroupInvitationCtrl;

  GroupInvitationCtrl.$inject = ['$scope', '$log', '$cookies', 'Group', 'INVITE_EMAIL_ID'];

  function GroupInvitationCtrl($scope, $log, $cookies, Group, INVITE_EMAIL_ID) {
    var vm = this;
    vm.inviteMember = inviteMember;
    vm.inviteSuccess = false;
    vm.inviteError = false;

    //
    // Controller implementation
    //

    function inviteMember() {
      vm.inviteSuccess = false;
      vm.inviteError = false;

      var contactId =  $cookies.get('userId');
      var toSend = {
        groupId: $scope.groupId,
        fromContactId: contactId,
        templateId: INVITE_EMAIL_ID,
        emailAddress: vm.invitee
      };

      Group.EmailInvite.save(toSend).$promise.then(function inviteEmailSuccess() {
        vm.inviteSuccess = true;
        vm.invitee = null;
      }, function inviteEmailError(error) {
        vm.inviteError = true;
      });
    }
  }

})();
