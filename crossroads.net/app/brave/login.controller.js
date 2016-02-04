(function(){
  'use strict';

  module.exports = LoginCtrl;

  LoginCtrl.$inject = ['$rootScope', '$scope', '$log', '$state', '$cookies'];

  // TODO This needs to go away when we implement authentication.
  function LoginCtrl($rootScope, $scope, $log, $state, $cookies) {
    $log.debug('login.controller.js');

    var vm = this;

    vm.authState = 'sign-in';

    vm.authenticate = function(){
      $cookies.put('userAuthenticated', true);
      $state.go('brave.summary');
    };

  }

})();
