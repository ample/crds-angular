(function(){
  'use strict';

  module.exports = GroupMemberService;

  GroupMemberService.$inject = [];

  function GroupMemberService() {

    function GroupMember(userData) {
      this.setData(userData);
    }

    GroupMember.prototype = {
      setData: function(data) {
        angular.extend(this, data);
      },
      clear: function() {
        console.log('clear');
      }
    };

    return GroupMember;
  }

})();
