(function(){
  'use strict';

  module.exports = GroupService;

  GroupService.$inject = [];

  function GroupService() {

    function Group(data) {
      this.setData(data);
    }

    Group.prototype = {
      setData: function(data) {
        angular.extend(this, data);
      },
      clear: function() {
        console.log('clear');
      }
    };

    return Group;
  }

})();
