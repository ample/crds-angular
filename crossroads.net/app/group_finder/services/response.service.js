(function(){
  'use strict';

  module.exports = ResponseService;

  ResponseService.$inject = ['$log', 'Group', 'GroupMember'];

  function ResponseService($log, Group, GroupMember) {

    this.data = {
      group: new Group(),
      member: new GroupMember()
    };

    this.clear = function(){
      this.data = {};
    };
  }

})();
