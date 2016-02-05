(function(){
  'use strict';

  module.exports = GroupFinderCtrl;

  GroupFinderCtrl.$inject = ['$log', '$state', '$cookies', 'Responses', 'AuthService'];

  function GroupFinderCtrl($log, $state, $cookies, Responses, AuthService) {
    if (AuthService.isAuthenticated() === true) {
      $log.debug('you are logged in - going to summary');
      $state.go('brave.summary');
    } else {
      $log.debug('not logged in');
      $state.go('brave.welcome');
    }
  }

})();
