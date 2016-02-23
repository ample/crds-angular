(function(){
  'use strict';

  module.exports = GroupInvitationService;

  GroupInvitationService.$inject = ['Group'];

  function GroupInvitationService(Group) {
    var service = {};
    service.acceptInvitation = acceptInvitation;

    function acceptInvitation(groupId, options) {

      var data = _.defaults(options, {
        'childCareNeeded': false,
        'sendConfirmationEmail': false
      });
      return Group.Participant.save({
        groupId: groupId
      }, [data]).$promise;
    }

    return service;
  }

})();
