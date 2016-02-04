(function(){
  'use strict';

  module.exports = BraveCtrl;

  BraveCtrl.$inject = ['$log', '$location', '$cookies', 'Responses'];

  function BraveCtrl($log, $location, $cookies, Responses) {

    // TODO This needs to go away when we implement authentication.
    if(!$cookies.get('userAuthenticated')) {
      $location.path('/brave/login');
    } else {
      Responses.clear();
      $location.path('/brave/welcome');
    }
  }

})();
