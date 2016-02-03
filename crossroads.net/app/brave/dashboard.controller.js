(function(){
  'use strict';

  module.exports = DashboardCtrl;

  DashboardCtrl.$inject = ['$scope', '$log', '$http'];

  function DashboardCtrl($scope, $log, $http) {
    $log.debug('inside dashboard ctrl.');
  }

})();
