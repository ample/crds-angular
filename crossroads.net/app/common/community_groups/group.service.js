(function() {

  module.exports = GroupService;

  GroupService.$inject = ['$resource', '$log'];

  function GroupService($resource, $log) {
    return {
      Participant: $resource(__API_ENDPOINT__ +  'api/group/:groupId/participants', {groupId: '@groupId'},
        {save: {method:'POST', isArray:true}}),
      Detail: $resource(__API_ENDPOINT__ +  'api/group/:groupId', {groupId: '@groupId'})
    };
  }

})();
