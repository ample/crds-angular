(function () {
  'use strict';

  module.exports = HostCtrl;

  HostCtrl.$inject = ['$timeout', '$scope', '$state', 'AuthenticatedPerson'];

  function HostCtrl ($timeout, $scope, $state, AuthenticatedPerson) {
    $scope.currentStep = 1;
    $scope.person = AuthenticatedPerson;
  }

})();
