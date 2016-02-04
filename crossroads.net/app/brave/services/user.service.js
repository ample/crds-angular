(function(){
  'use strict';

  module.exports = UserService;

  UserService.$inject = [];

  function UserService() {

    function User(userData) {
      this.setData(userData);
    }

    User.prototype = {
      setData: function(userData) {
        angular.extend(this, userData);
      },
      clear: function() {
        console.log('clear');
      }
    };

    return User;
  }

})();
