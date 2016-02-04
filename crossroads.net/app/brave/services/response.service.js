(function(){
  'use strict';

  module.exports = ResponseService;

  ResponseService.$inject = ['$log', 'User', 'Group'];

  function ResponseService($log, User, Group) {

    this.data = {
      user: new User(),
      group: new Group()
    };

    this.clear = function(){
      this.data = {};
    };
  }

})();
