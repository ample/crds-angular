(function() {

  module.exports = GroupService;

  GroupService.$inject = ['$resource', '$log'];

  function GroupService($resource, $log) {
    return {
      Participant: $resource(__API_ENDPOINT__ +  'api/group/:groupId/participants', {groupId: '@groupId'},
        {save: {method:'POST', isArray:true}}),
      Type: $resource(__API_ENDPOINT__ +  'api/group/groupType/:groupTypeId', {groupTypeId: '@groupTypeId'}),
      Detail: $resource(__API_ENDPOINT__ +  'api/group/:groupId', {groupId: '@groupId'}),
      Events: $resource(__API_ENDPOINT__ + 'api/group/:groupId/events'),
      Participants: $resource(__API_ENDPOINT__ + 'api/group/:groupId/event/:eventId')
    };
  }

})();
