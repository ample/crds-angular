(function(){
  'use strict';

  module.exports = LoginCtrl;

  LoginCtrl.$inject = ['$rootScope', '$scope', '$log', '$location', '$cookies'];

  // TODO This needs to go away when we implement authentication.
  function LoginCtrl($rootScope, $scope, $log, $location, $cookies) {
    $log.debug('login.controller.js');

    $scope.authState = 'sign-in';

    this.authenticate = function(){
      $cookies.put('userAuthenticated', true);
      $location.path('/brave/welcome');
    };

  }

})();
