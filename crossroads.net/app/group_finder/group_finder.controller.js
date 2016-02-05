(function(){
  'use strict';

  module.exports = GroupFinderCtrl;

  GroupFinderCtrl.$inject = ['$log', '$location', '$cookies', 'Responses'];

  function GroupFinderCtrl($log, $location, $cookies, Responses) {
    $location.path('/brave/welcome');
  }

})();
