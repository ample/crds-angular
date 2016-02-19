(function(){
  'use strict';

  module.exports = GroupInfoService;

  GroupInfoService.$inject = ['$http', '$cookies', '$q', 'Group'];

  function GroupInfoService($http, $cookies, $q, Group) {
    var groupInfo = {};
    var groups = {
      hosting: [],
      participating: []
    };

    var requestComplete = false;
    var GroupType = Group.Type.query(function(data) {
      var cid = $cookies.get('userId');
      if (cid) {
        _.each(data, function(group) {

          if (group.contactId === parseInt(cid)) {
            group.isHost = true;
            groups.hosting.push(group);
          } else {
            group.isHost = false;
            groups.participating.push(group);
          }
        });
      }
      requestComplete = true;
      return groups;
    });

    groupInfo.getHosting = function() {
      return groups.hosting;
    };

    groupInfo.getParticipating = function() {
      return groups.participating;
    };

    groupInfo.findHosting = function(id) {
      return _.find(groups.hosting, function(group) {
        return group.id === parseInt(id);
      });
    };

    return groupInfo;

  }

})();
