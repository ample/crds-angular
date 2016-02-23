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
<<<<<<< HEAD
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
=======
    var dataPromise = $http.get('/app/group_finder/data/user.group.json');
    dataPromise.then(function(res) {
      var cid = $cookies.get('userId');
      if (cid) {
        _.each(res.data.groups, function(group, i, list) {
          groups.byId[group.id] = group;
          _.each(group.members, function(member, i, list) {

            if (member.groupRoleId === 22) {
              if (member.contactId === parseInt(cid)) {
                group.isHost = true;
                group.host = member;
                groups.hosting.push(group);
              } else {
                group.isHost = false;
                group.host = member;
                groups.participating.push(group);
              }
            }
          });
        });
      }
      return groups;
    })
    .finally(function() {
      requestComplete = true;
>>>>>>> feature/group-finder
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

<<<<<<< HEAD
=======
    groupInfo.findGroupById = function(id) {
      return dataPromise.then(function() {
        return groups.byId[id];
      });
    };

>>>>>>> feature/group-finder
    return groupInfo;

  }

})();
