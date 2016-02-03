(function(){
  'use strict';

  module.exports = BraveCtrl;

  BraveCtrl.$inject = ['$log', '$location', '$cookies', 'ResponseService'];

  function BraveCtrl($log, $location, $cookies, ResponseService) {

    // TODO This needs to go away when we implement authentication.
    if(!$cookies.get('userAuthenticated')) {
      $location.path('/brave/login');
    } else {
      ResponseService.clear();
      $location.path('/brave/welcome');
    }
  }

})();
