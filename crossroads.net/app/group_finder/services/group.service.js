(function () {
  'use strict';

  module.exports = GroupService;

  GroupService.$inject= ['$resource', '$log', 'GROUP_TYPE_ID'];

  function GroupService($resource, $log, GROUP_TYPE_ID) {
    return {
      Type: $resource(__API_ENDPOINT__ +  'api/group/groupType/:groupTypeId', {groupTypeId: GROUP_TYPE_ID})
    };
  }
})();
